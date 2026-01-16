using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;

namespace ConnectionIomGenerator.Service
{
	public interface IIomGenerator
	{
		Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input, LoadingInput? loadingInput);
	}
}
