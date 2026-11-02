namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// CrossSectionFactory
	/// </summary>
	public static partial class CrossSectionFactory
	{
		public static void FillLibraryShape(CrossSectionParameter css, string searchName)
		{
			css.CrossSectionType = CrossSectionType.UniqueName;
			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = searchName });
		}

		/// <summary>
		/// Rectangle shape - massive concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public static void FillRectangle(CrossSectionParameter css, double width, double height)
		{
			css.CrossSectionType = CrossSectionType.Rect;
			css.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
			css.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		}

		/// <summary>
		/// Circular shape - for steel or concrete shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="d">Diametrer of the shape</param>
		public static void FillCircle(CrossSectionParameter css, double d)
		{
			css.CrossSectionType = CrossSectionType.O;
			css.Parameters.Add(new ParameterDouble() { Name = "D", Value = d });
		}

		/// <summary>
		/// Fill general shape
		/// </summary>
		/// <param name="css">Parameters of CrossSectionParameter will be filled</param>
		/// <param name="pt0">Cross-section point 0</param>
		/// <param name="pt1">Cross-section point 1</param>
		/// <param name="pt2">Cross-section point 2</param>
		/// <param name="pt3">Cross-section point 3</param>
		/// <param name="pt4">Cross-section point 4</param>
		/// <param name="pt5">Cross-section point 5</param>
		public static void FillGeneralShape(CrossSectionParameter css, double pt0, double pt1, double pt2, double pt3, double pt4, double pt5)
		{
			css.CrossSectionType = CrossSectionType.CHSg;
			css.Parameters.Add(new ParameterDouble() { Name = "Pt0", Value = pt0 });
			css.Parameters.Add(new ParameterDouble() { Name = "Pt1", Value = pt1 });
			css.Parameters.Add(new ParameterDouble() { Name = "Pt2", Value = pt2 });
			css.Parameters.Add(new ParameterDouble() { Name = "Pt3", Value = pt3 });
			css.Parameters.Add(new ParameterDouble() { Name = "Pt4", Value = pt4 });
			css.Parameters.Add(new ParameterDouble() { Name = "Pt5", Value = pt5 });
		}
	}
}