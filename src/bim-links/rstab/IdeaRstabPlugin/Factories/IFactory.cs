namespace IdeaRstabPlugin.Factories
{
	internal interface IFactory<Source, Target>
	{
		/// <summary>
		/// Creates an instance of <typeparamref name="Target"/> based on <typeparamref name="Source"/>.
		/// </summary>
		/// <param name="objectFactory">IObjectFactory instance</param>
		/// <param name="importSession"></param>
		/// <returns><typeparamref name="Target"/> instance</returns>
		/// <param name="source"><typeparamref name="Source"/> instance</param>
		
		Target Create(IObjectFactory objectFactory, IImportSession importSession, Source source);
	}
}