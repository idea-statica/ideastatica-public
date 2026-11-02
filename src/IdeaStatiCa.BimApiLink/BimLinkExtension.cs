using IdeaStatiCa.BimApiLink.Plugin;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink
{
	public static class BimLinkExtension
	{
		public static Task Run(this BimLink bimLink, IModel model)
			=> bimLink.Create(model).Run();
	}
}