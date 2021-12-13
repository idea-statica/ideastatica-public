using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea.Providers
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

		private readonly IModel _modelData;
		private readonly Dictionary<(DataType, int), object> _data = new Dictionary<(DataType, int), object>();

		public ModelDataProvider(IModel model)
		{
			//using (new LicenceLock(model))
			//{
			_modelData = model;
			//}
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

		public IMember GetMember(int no)
		{
			return GetOrCreate(no, DataType.Member, () => IMemberUtils.GetMember(_modelData, no));	
		}

		//public IMember GetIMember(int no)
		//{
		//	return _modelData.GetMember(no, ItemAt.AtNo);
		//}


		//public IEnumerable<IMember> GetMembers()
		//{
		//	foreach (IMember member in _modelData.GetMembers())
		//	{
		//		_data[(DataType.Member, member.No)] = member;
		//		yield return member;
		//	}
		//}

		public INode GetNode(int no)
		{
			return GetOrCreate(no, DataType.Node, () => _modelData.GetFrameAnalysisNodes().Get(no));
		}

		//public IBeam GetBeam(int no)
		//{
		//	return GetOrCreate(no, DataType.Beam, () => _modelData.GetBeam(no));
		//}

		//public IColumn GetColumn(int no)
		//{
		//	return GetOrCreate(no, DataType.Column, () => _modelData.GetColumn(no));
		//}

		//public IHorizBrace GetHorizBrace(int no)
		//{
		//	return GetOrCreate(no, DataType.HorizBrace, () => _modelData.GetHorizBrace(no));
		//}

		//public IVerticalBrace GetVerticalBrace(int no)
		//{
		//	return GetOrCreate(no, DataType.VerticalBrace, () => _modelData.GetVerticalBrace(no));
		//}

		//public IEnumerable<IBeam> GetBeams()
		//{
		//	return GetOrCreate(no, DataType.VerticalBrace, () => _modelData.GetBeam);
		//}

		//public IEnumerable<IColumn> GetColumns()
		//{
		//	throw new NotImplementedException();
		//}

		//public IEnumerable<IHorizBrace> GetHorizantalBraces()
		//{
		//	throw new NotImplementedException();
		//}

		//public IEnumerable<IVerticalBrace> GetVerticalBraces()
		//{
		//	throw new NotImplementedException();
		//}
	}
}
