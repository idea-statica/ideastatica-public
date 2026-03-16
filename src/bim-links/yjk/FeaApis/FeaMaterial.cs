using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaStatiCa.BimApi;

namespace yjk.FeaApis
{
	public interface IFeaMaterial
	{
		int Id { get; }
		MaterialType MaterialType { get; }
		string Name { get; }
	}

	internal abstract class FeaMaterial : IFeaMaterial
	{
		public int Id { get; set; }
		public MaterialType MaterialType { get; set; }
		public string Name { get; set; }
	}

	internal class FeaMaterialSteel : FeaMaterial
	{
		public FeaMaterialSteel(int id, string name)
		{
			Id = id;
			MaterialType = MaterialType.Steel;
			Name = name;
		}
	}

	internal class FeaMaterialConcrete: FeaMaterial
	{
		public FeaMaterialConcrete(int id, string name, double fck)
		{
			Id = id;
			MaterialType = MaterialType.Concrete;
			Name = name;
			Fck = fck;
		}
		public double Fck { get; set; }
	}
}
