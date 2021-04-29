using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	public class Project : IProject
	{
		public ConversionDictionaryString IdMapping { get; private set; } = new ConversionDictionaryString();

		private readonly Dictionary<int, IIdeaObject> _iomIdToBimObject = new Dictionary<int, IIdeaObject>();

		private int _id = 1;

		public int GetIomId(string bimId)
		{
			return IdMapping[bimId];
		}

		public int GetIomId(IIdeaObject obj)
		{
			if (IdMapping.TryGetValue(obj.Id, out int id))
			{
				return id;
			}

			id = _id++;
			IdMapping.Add(obj.Id, id);
			_iomIdToBimObject.Add(id, obj);

			return id;
		}

		public IIdeaObject GetBimObject(int id)
		{
			return _iomIdToBimObject[id];
		}

		public void Load(IGeometry geometry, ConversionDictionaryString conversionTable)
		{
			IEnumerable<IIdeaObject> objects = geometry.GetMembers().OfType<IIdeaObject>().Concat(geometry.GetNodes());

			foreach (IIdeaObject obj in objects)
			{
				string bimId = obj.Id;
				if (conversionTable.TryGetValue(bimId, out int id))
				{
					_iomIdToBimObject.Add(id, obj);
				}
			}

			IdMapping = conversionTable;
		}
	}
}