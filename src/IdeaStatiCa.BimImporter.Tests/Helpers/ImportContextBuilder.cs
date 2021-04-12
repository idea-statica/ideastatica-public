using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using NSubstitute;

namespace IdeaStatiCa.BimImporter.Tests.Helpers
{
	internal class ImportContextBuilder
	{
		public IImportContext Context { get; }

		public ImportContextBuilder()
		{
			Context = Substitute.For<IImportContext>();
		}

		public (T, ReferenceElement) Add<T>() where T : class, IIdeaObject
		{
			T obj = Substitute.For<T>();
			ReferenceElement refElm = new ReferenceElement();
			Context.Import(obj).Returns(refElm);
			return (obj, refElm);
		}

		public ReferenceElement Add<T>(T obj) where T : IIdeaObject
		{
			ReferenceElement refElm = new ReferenceElement();
			Context.Import(obj).Returns(refElm);
			return refElm;
		}
	}
}