using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System;

// A C++/CLI class cannot derive from IntIdentifierImporter<T>, nor from an abstract C# class derived from it: the
// compiler reports error C3766 for the generic methods of IImporter. It can derive from a class that is not abstract,
// so each class here fixes IntIdentifierImporter<T> to one kind of object, and the C++/CLI importer overrides Create.
namespace BimApiLinkCppFeaExampleBases
{
	public class CombinationImporterBase : IntIdentifierImporter<IIdeaCombiInput>
	{
		public override IIdeaCombiInput Create(int id) => throw new NotImplementedException();
	}

	public class CrossSectionImporterBase : IntIdentifierImporter<IIdeaCrossSection>
	{
		public override IIdeaCrossSection Create(int id) => throw new NotImplementedException();
	}

	public class LoadCaseImporterBase : IntIdentifierImporter<IIdeaLoadCase>
	{
		public override IIdeaLoadCase Create(int id) => throw new NotImplementedException();
	}

	public class LoadGroupImporterBase : IntIdentifierImporter<IIdeaLoadGroup>
	{
		public override IIdeaLoadGroup Create(int id) => throw new NotImplementedException();
	}

	public class MaterialImporterBase : IntIdentifierImporter<IIdeaMaterial>
	{
		public override IIdeaMaterial Create(int id) => throw new NotImplementedException();
	}

	public class MemberImporterBase : IntIdentifierImporter<IIdeaMember1D>
	{
		public override IIdeaMember1D Create(int id) => throw new NotImplementedException();
	}

	public class NodeImporterBase : IntIdentifierImporter<IIdeaNode>
	{
		public override IIdeaNode Create(int id) => throw new NotImplementedException();
	}
}
