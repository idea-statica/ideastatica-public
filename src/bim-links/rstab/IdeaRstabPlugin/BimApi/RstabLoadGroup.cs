using Dlubal.RSTAB8;
using IdeaRstabPlugin.Factories;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using System;

namespace IdeaRstabPlugin.BimApi
{
	class RstabLoadGroup : IIdeaLoadGroup
	{
		private IModel _model;
		private ObjectFactory _objectFactory;
		private int _lgNo;
		private Relation _Relation = Relation.Standard;
		private LoadGroupType _GroupType = LoadGroupType.Variable;

		public RstabLoadGroup(IModel model, ObjectFactory objectFactory, int lgNo)
		{
			this._model = model;
			this._objectFactory = objectFactory;
			this._lgNo = lgNo;
			switch (_lgNo)
			{
				case 2:
					_GroupType = LoadGroupType.Permanent;
					_Relation = Relation.Standard;
					break;
				case 3:
					_GroupType = LoadGroupType.Variable;
					_Relation = Relation.Standard;
					break;
				case 4:
					_GroupType = LoadGroupType.Fatigue;
					_Relation = Relation.Standard;
					break;
			}
		}

		public Relation Relation { get { return _Relation; } set { _Relation = value; } }

		public LoadGroupType GroupType { get { return _GroupType; } set { _GroupType = value; } }

		public double GammaQ => 1.5;

		public double Dzeta => 0.85;

		public double GammaGInf => 1.0;

		public double GammaGSup => 1.35;

		public double Psi0 => 0.7;

		public double Psi1 => 0.5;

		public double Psi2 => 0.3;

		public string Id => "loadgroup-" + _lgNo.ToString();

		public string Name
		{
			get
			{
				switch (_lgNo)
				{
					case 1:
						return "Default";
					case 2:
						return "Permanent base";
					case 3:
						return "Variable base";
					case 4:
						return "Variable cyclic base";

					default:
						return "LG " + _lgNo.ToString();
				}
			}
		}
	}
}
