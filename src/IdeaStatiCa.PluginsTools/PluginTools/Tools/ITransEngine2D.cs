using System.Windows;

namespace CI.GiCL2D
{
	public enum Axis2D
	{
		X = 0,
		Y = 1
	}

	/// <summary>
	/// Interface pro provadeni transformace 2D
	/// </summary>
	public interface ITransEngine2D
	{
		/// <summary>
		/// Cteni transformacni matice
		/// </summary>
		double[,] Matrix { get; }

		/// <summary>
		/// Cteni prvku transformacni matice
		/// </summary>
		/// <param name="i"></param> radek matice
		/// <param name="j"></param> sloupec matice
		/// <returns></returns>
		double this[int i, int j] { get; }

		/// <summary>
		/// transformace bodu do souradneho systemu
		/// </summary>
		/// <param name="src"></param>zdrojovy bod
		/// <param name="dest"></param>cilovy bod
		void TransTo(Point src, ref Point dest);

		/// <summary>
		/// transformace bodu ze souradneho systemu
		/// </summary>
		/// <param name="src"></param>zdrojovy bod
		/// <param name="dest"></param>cilovy bod
		void TransFrom(Point src, ref Point dest);

		/// <summary>
		/// transformuje vektor do souradneho systemu
		/// </summary>
		/// <param name="src"></param>zdrojovy vektor
		/// <param name="dest"></param>cilovy vektor
		void TransTo(Vector src, ref Vector dest);

		/// <summary>
		/// transformuje vektor ze souradneho systemu
		/// </summary>
		/// <param name="src"></param>zdrojovy vektor
		/// <param name="dest"></param>cilovy vektor
		void TransFrom(Vector src, ref Vector dest);

		/// <summary>
		/// transformuje smer do souradneho systemu
		/// </summary>
		/// <param name="src"></param>zdrojovy smer
		/// <param name="dest"></param>cilovy smer
		void TransTo(Direction src, ref Direction dest);

		/// <summary>
		/// transformuje smer ze souradneho systemu
		/// </summary>
		/// <param name="src"></param>zdrojovy smer
		/// <param name="dest"></param>cilovy smer
		void TransFrom(Direction src, ref Direction dest);

		/// <summary>
		/// zrcadli bod podle zadane osy souradneho systemu
		/// </summary>
		/// <param name="axis"></param>osa zrcadleni
		/// <param name="src"></param>zdrojovy bod
		/// <param name="dest"></param>cilovy bod
		void Mirror(Axis2D axis, Point src, ref Point dest);
	}
}
