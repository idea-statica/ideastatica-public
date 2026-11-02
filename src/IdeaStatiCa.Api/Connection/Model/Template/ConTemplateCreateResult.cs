using System;

namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Result of creating a reusable connection template from an existing connection.
	/// Carries the template payload (contemp string) alongside structured metadata
	/// inherited from the source connection.
	/// </summary>
	public class ConTemplateCreateResult
	{
		/// <summary>
		/// Identifier of the produced template (newly generated for the export).
		/// </summary>
		public Guid TemplateId { get; set; }

		/// <summary>
		/// Name of the template, inherited from the connection header.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Design code of the source connection (e.g. "ECEN", "American", "AUS").
		/// </summary>
		public string DesignCode { get; set; }

		/// <summary>
		/// Data contract version embedded in the produced template.
		/// </summary>
		public int Version { get; set; }

		/// <summary>
		/// Manufacturing type of the connection (e.g. Welded, Bolted, Mixed).
		/// </summary>
		public string ManufacturingType { get; set; }

		/// <summary>
		/// Classified member typology of the connection (e.g. "X+;Y+").
		/// </summary>
		public string TypologyCode { get; set; }

		/// <summary>
		/// Classified member typology in V2 form (e.g. "C;EX+;EY+").
		/// </summary>
		public string TypologyCode_V2 { get; set; }

		/// <summary>
		/// Number of operations captured in the template.
		/// </summary>
		public int OperationCount { get; set; }

		/// <summary>
		/// Number of parameters captured in the template.
		/// </summary>
		public int ParameterCount { get; set; }

		/// <summary>
		/// Number of parametric links (parameter-to-model bindings) captured in the template.
		/// </summary>
		public int ParamModelLinkCount { get; set; }

		/// <summary>
		/// The serialized template payload ('contemp' string).
		/// </summary>
		public string Template { get; set; }
	}
}
