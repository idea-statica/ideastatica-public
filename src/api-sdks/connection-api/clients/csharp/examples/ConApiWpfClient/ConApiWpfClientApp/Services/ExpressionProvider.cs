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

		/// <summary>
		/// Asynchronously modifies the expression.
		/// </summary>
		/// <remarks>If the connection contains members, the default expression is generated based on the first member's
		/// cross-section height. If the connection has no members, a default expression is used. The generated expression is
		/// then modified using a text editor service. Look at https://developer.ideastatica.com/docs/api/api_parameter_reference_guide.html for more details about expressions.</remarks>
		/// <param name="projectId">The unique identifier of the project containing the connection.</param>
		/// <param name="connectionId">The identifier of the connection whose members are used to generate the expression.</param>
		/// <param name="cts">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
		/// <returns>A task that represents the asynchronous operation. The task result is an <see cref="ExpressionModel"/> containing
		/// the modified expression, or <see langword="null"/> if the expression could not be modified.</returns>
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
				// there are no members in the connection - return some arithmetic expression as the example.
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
