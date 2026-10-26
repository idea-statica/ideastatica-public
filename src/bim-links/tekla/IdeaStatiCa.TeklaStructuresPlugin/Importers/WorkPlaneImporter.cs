
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	public class WorkPlaneImporter : BaseImporter<IIdeaWorkPlane>
	{
		public WorkPlaneImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaWorkPlane Create(string id)
		{
			PlugInLogger.LogInformation($"WorkPlaneImporter create {id}");
			var teklaWorkPlaneItem = Model.GetItemByHandler(id);
			if (teklaWorkPlaneItem == null)
			{
				PlugInLogger.LogInformation($"WorkPlaneImporter not found member {id}");
				return null;
			}

			var ideaWorkPlane = new WorkPlane(id);

			if (teklaWorkPlaneItem is CutPlane cutPlane)
			{
				var plane = new GeometricPlane(cutPlane.Plane.Origin, cutPlane.Plane.AxisX, cutPlane.Plane.AxisY);
				var normalVector = plane.Normal.GetNormal();
				ideaWorkPlane.OriginNo = Model.GetPointId(cutPlane.Plane.Origin);
				ideaWorkPlane.Normal = new IdeaVector3D(normalVector.X, normalVector.Y, normalVector.Z);
				Model.CacheCreatedObject(new StringIdentifier<IIdeaWorkPlane>(id), ideaWorkPlane);
				return ideaWorkPlane;
			}
			else if (teklaWorkPlaneItem is Fitting fitting)
			{
				var plane = new GeometricPlane(fitting.Plane.Origin, fitting.Plane.AxisX, fitting.Plane.AxisY);
				var normalVector = plane.Normal.GetNormal();
				ideaWorkPlane.OriginNo = Model.GetPointId(fitting.Plane.Origin);
				ideaWorkPlane.Normal = new IdeaVector3D(normalVector.X, normalVector.Y, normalVector.Z);
				Model.CacheCreatedObject(new StringIdentifier<IIdeaWorkPlane>(id), ideaWorkPlane);
				return ideaWorkPlane;
			}
			else
			{
				PlugInLogger.LogInformation($"WorkPlaneImporter unknown workplane {teklaWorkPlaneItem.GetType()}");
				return null;
			}
		}
	}
}