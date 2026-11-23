using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.IOM.VersioningService.Exceptions;
using IdeaStatiCa.IOM.VersioningService.Extension;
using IdeaStatiCa.IOM.VersioningService.Tools;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.VersionSteps.Steps
{
	/// <summary>
	/// 3.3.1 - PlateData.Material: name string &lt;-&gt; ReferenceElement (MatSteel). The up step throws
	/// <see cref="UnresolvedMaterialNameException"/> for the plates naming a grade MatSteel does not list, negative
	/// volumes excepted.
	/// </summary>
	internal class Step331 : BaseStep
	{
		public Step331(IPluginLogger logger) : base(logger)
		{
		}

		public static Version Version => Version.Parse("3.3.1");

		public override Version GetVersion() => Step331.Version;

		public override void DoUpStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. UpStep {Version} was skipped");
				return;
			}

			var nameToId = ConnectionRefTool.BuildNameToId(openModel, "MatSteel");
			var unresolved = new List<UnresolvedMaterialName>();
			foreach (var plate in ConnectionRefTool.FindAll(openModel, "PlateData").ToList())
			{
				var unresolvedName = ConnectionRefTool.StringToReference(plate, "Material", "MatSteel", nameToId);
				if (unresolvedName == null)
				{
					continue;
				}

				var item = new UnresolvedMaterialName(ConnectionElementKind.Plate, plate.TryGetElementValue("Id"), plate.TryGetElementValue("Name"), plate.TryGetElementValue("OriginalModelId"), unresolvedName);
				// A negative volume only cuts, so its steel decides nothing.
				if (IsNegativeObject(plate))
				{
					_logger.LogWarning($"UpStep {Version}: negative plate '{item.ElementName}' (Id {item.ElementId}) names steel '{unresolvedName}', which is not in MatSteel; left without material");
				}
				else
				{
					unresolved.Add(item);
				}
			}

			if (unresolved.Count > 0)
			{
				throw new UnresolvedMaterialNameException(unresolved);
			}
		}

		public override void DoDownStep(SModel _model)
		{
			ISIntermediate openModel = _model.GetModelElement();
			if (openModel == null)
			{
				_logger.LogInformation($"OpenModel not found. DownStep {Version} was skipped");
				return;
			}

			var idToName = ConnectionRefTool.BuildIdToName(openModel, "MatSteel");
			foreach (var plate in ConnectionRefTool.FindAll(openModel, "PlateData").ToList())
			{
				ConnectionRefTool.ReferenceToString(plate, "Material", idToName, _logger);
			}
		}

		private static bool IsNegativeObject(SObject plate)
		{
			var value = plate.TryGetElementValue("IsNegativeObject")?.Trim();
			return value == "true" || value == "1";
		}
	}
}
