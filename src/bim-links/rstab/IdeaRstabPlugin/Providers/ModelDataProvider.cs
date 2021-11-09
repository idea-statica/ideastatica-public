using Dlubal.RSTAB8;
using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Providers
{
	internal class ModelDataProvider : IModelDataProvider, IDataCache
	{
		private enum DataType
		{
			Node,
			Member,
			Line,
			CrossSection,
			Material
		}

		private readonly IModelData _modelData;
		private readonly Dictionary<(DataType, int), object> _data = new Dictionary<(DataType, int), object>();

		public ModelDataProvider(IModel model)
		{
			using (new LicenceLock(model))
			{
				_modelData = model.GetModelData();
			}
		}

		public void Clear()
		{
			_data.Clear();
		}

		private T GetOrCreate<T>(int no, DataType dataType, Func<T> factory)
		{
			if (_data.TryGetValue((dataType, no), out object data))
			{
				return (T)data;
			}

			data = factory();
			_data[(dataType, no)] = data;

			return (T)data;
		}

		//public Line GetLine(int no)
		//{
		//	return GetOrCreate(no, DataType.Line, () => _modelData.GetLine(no, ItemAt.AtNo).GetData());
		//}

		//public ILine GetILine(int no)
		//{
		//	return _modelData.GetLine(no, ItemAt.AtNo);
		//}

		public Member GetMember(int no)
		{
			return GetOrCreate(no, DataType.Member, () => GetIMember(no).GetData());
		}

		public IMember GetIMember(int no)
		{
			return _modelData.GetMember(no, ItemAt.AtNo);
		}

		public IEnumerable<Member> GetMembers()
		{
			foreach (Member member in _modelData.GetMembers())
			{
				_data[(DataType.Member, member.No)] = member;
				yield return member;
			}
		}

		public Node GetNode(int no)
		{
			return GetOrCreate(no, DataType.Node, () => _modelData.GetNode(no, ItemAt.AtNo).GetData());
		}
	}
}