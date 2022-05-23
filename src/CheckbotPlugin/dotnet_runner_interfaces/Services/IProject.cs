class ProjectInfo
{
	public string Name { get; }
	public CountryCode CountryCode { get; }
}

enum Item
{
	ConnectionPoint,
	Substructure
}

interface IProjectService
{
	ProjectInfo Info { get; }
}