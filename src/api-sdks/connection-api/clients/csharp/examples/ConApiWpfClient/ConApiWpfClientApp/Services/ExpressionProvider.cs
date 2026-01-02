using ConApiWpfClientApp.Models;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	public class ExpressionProvider : IExpressionProvider
	{
		private readonly IConnectionApiClient _connectionApiClient;
		private readonly IPluginLogger _logger;

		public ExpressionProvider(IConnectionApiClient connectionApiClient, IPluginLogger logger)
		{
			_connectionApiClient = connectionApiClient;
			_logger = logger;
		}

		public async Task<ExpressionModel?> GetExpressionAsync(Guid projectId, int connectionId, CancellationToken cts)
		{
			string expression = string.Empty;

			// get all connected members in the connection
			var members = await _connectionApiClient.Member.GetMembersAsync(projectId, connectionId, 0, cts);
			if(members != null && members.Any())
			{
				// example - get height of the cross-section of the first member in the connection
				var firstMember = members.First();
				//firstMember.Id
				expression = $"GetValue('{firstMember.Name}', 'CrossSection.Bounds.Height')";
			}
			else
			{
				// there are no members in the connection - return som expression
				expression = "Abs(-1 - 5)";
			}

			var editor = new TextEditorService();
			var modifiedExpression = await editor.EditAsync(expression);
			if(string.IsNullOrEmpty(modifiedExpression))
			{
				return null;
			}


			var res = new ExpressionModel();
			res.Expression = modifiedExpression;

			return await Task.FromResult(res);
		}
	}
}
