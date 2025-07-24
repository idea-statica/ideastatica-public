using System;
using System.Linq.Expressions;
using IdeaRS.OpenModel;
using IomToRcsExamples;

namespace IomToRcsExamples
{
	public interface IRcsExample
	{
		OpenModel BuildOpenModel();
	}

	public class RcsExampleBuilder
	{
		public enum Example { ReinforcedColumn, ReinforcedBeam, PrestressedBeam }

		public static OpenModel BuildExampleModel(Example example)
		{
			switch (example)
			{
				case Example.ReinforcedBeam:
					return new IomReinforcedBeam().BuildOpenModel();
				case Example.ReinforcedColumn:
					return new IomRcsColumn().BuildOpenModel();
				case Example.PrestressedBeam:
					return new IomRcsColumn().BuildOpenModel();
				default:
					return new IomReinforcedBeam().BuildOpenModel();
			}
		}
	}
}
