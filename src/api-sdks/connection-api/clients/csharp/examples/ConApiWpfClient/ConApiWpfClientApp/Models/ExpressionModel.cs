namespace ConApiWpfClientApp.Models
{
	/// <summary>
	/// Model containing a parametric expression string for evaluation against a connection.
	/// </summary>
	public class ExpressionModel
	{
		/// <summary>
		/// Gets or sets the expression to evaluate (e.g., "GetValue('B1', 'CrossSection.Bounds.Height')").
		/// </summary>
		public string? Expression { get; set; }
	}
}
