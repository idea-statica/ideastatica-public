namespace CI.GiCL2D
{
	/// <summary>
	/// Zakladni interface pro geometricke entity
	/// </summary>
#if !SILVERLIGHT
	[System.Reflection.Obfuscation(Feature = "renaming")]
#endif
	public interface IGeomEntity2D
	{
		/// <summary>
		/// Funkce vynuti geometrickou transformaci entity
		/// </summary>
		/// <param name="inside"></param>smer transformace  true = transformuj do systemu, false = transformuj ze systemu
		/// <param name="te"></param>transformacni interface
		void Trans(bool inside, ITransEngine2D te);
	}
}
