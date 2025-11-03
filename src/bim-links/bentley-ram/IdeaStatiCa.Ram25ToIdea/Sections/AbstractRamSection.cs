using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Model;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal abstract class AbstractRamSection : IRamSection
	{
		public double Rotation => 0;

		public string Id => $"{_properties.SectionID}-{_properties.SectionLabel}";

		public string Name => string.IsNullOrEmpty(_properties.SectionLabel) ? "<empty>" : _properties.SectionLabel;

		public bool IsInPrincipal => false;

		private readonly IMaterialFactory _materialFactory;
		private readonly RamMemberProperties _properties;

		protected AbstractRamSection(IMaterialFactory materialFactory, RamMemberProperties props)
		{
			_materialFactory = materialFactory;

			_properties = props;
		}

		protected IIdeaMaterial GetMaterial()
		{
			return _materialFactory.GetMaterial(_properties);
		}
	}
}