using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConOperation : ConItem
	{
		public ConOperation() : base()
		{
		}

		[JsonConstructor]
		public ConOperation(int id) : base(id)
		{
		}

		public bool IsImported { get; set; }


		/// <summary>
		/// Type of the manufacturing operation
		/// </summary>
		public string OperationType { get; set; }
	}
}