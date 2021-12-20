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

		public string Name => _properties.SectionLabel;

		private readonly IObjectFactory _objectFactory;
		private readonly RamMemberProperties _properties;

		public AbstractRamSection(double height, RamMemberProperties props)
		{
			Height = height;
			_properties = props;
		}

		protected IIdeaMaterial GetMaterial()
		{
			return _objectFactory.GetMaterial(_properties.MaterialType, _properties.MaterialUID);
		}
	}
}