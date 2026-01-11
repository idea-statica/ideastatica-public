using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;

namespace ConnectionIomGenerator.Service
{
	public class IomGenerator : IIomGenerator
	{
		public IomGenerator()
		{
		}

		public async Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input)
		{
			var res = new OpenModelContainer();

			var generator = new FeaGenerator();
			var feaModel =generator.Generate(input);

			return await Task.FromResult(res);
		}
	}
}
