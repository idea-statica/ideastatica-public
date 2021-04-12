using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	public class Project: IProject
	{
		private readonly Dictionary<string, int> _bimToIomId = new Dictionary<string, int>();
		private readonly Dictionary<int, IIdeaObject> _iomIdToBimObject = new Dictionary<int, IIdeaObject>();

		private int _id = 1;

		public int GetIomId(string bimId)
		{
			return _bimToIomId[bimId];
		}

		public int GetIomId(IIdeaObject obj)
		{
			if (_bimToIomId.TryGetValue(obj.Id, out int id))
			{
				return id;
			}

			id = _id++;
			_bimToIomId.Add(obj.Id, id);
			_iomIdToBimObject.Add(id, obj);

			return id;
		}

		public IIdeaObject GetBimObject(int id)
		{
			return _iomIdToBimObject[id];
		}
	}
}