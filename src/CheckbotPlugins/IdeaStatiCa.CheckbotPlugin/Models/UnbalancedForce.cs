namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public class UnbalancedForce
	{
		public double X { get; set; }

		public double Y { get; set; }

		public double Z { get; set; }

		public double Mx { get; set; }

		public double My { get; set; }

		public double Mz { get; set; }

		public string LoadName { get; set; }

		public UnbalancedForce(string loadName, double x, double y, double z, double mx, double my, double mz)
		{
			LoadName = loadName;
			X = x;
			Y = y;
			Z = z;
			Mx = mx;
			My = my;
			Mz = mz;
		}

	}
}
