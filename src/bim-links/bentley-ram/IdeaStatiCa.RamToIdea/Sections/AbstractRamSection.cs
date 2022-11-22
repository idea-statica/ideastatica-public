using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal abstract class AbstractRamSection : IRamSection
	{
		public double Height { get; }

		public double Rotation => 0;

		public string Id => $"{_properties.SectionID}-{_properties.SectionLabel}";

		public string Name => string.IsNullOrEmpty(_properties.SectionLabel) ? "<empty>" : _properties.SectionLabel;

		public bool IsInPrincipal => false;

		private readonly IMaterialFactory _materialFactory;
		private readonly RamMemberProperties _properties;

		public AbstractRamSection(IMaterialFactory materialFactory, double height, RamMemberProperties props)
		{
			_materialFactory = materialFactory;

			Height = height;
			_properties = props;
		}

		protected IIdeaMaterial GetMaterial()
		{
			return _materialFactory.GetMaterial(_properties);
		}
	}
}