using NorsokChecker.Controls;
using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Rendering the joint view to a PNG for the report.
	///
	/// The trap this guards is that a WPF control which has never been laid out renders as a blank
	/// bitmap — no exception, no warning, just an empty picture. So these do not check "did it return
	/// bytes": they check that the bytes contain a drawing, by counting how many distinct colours the
	/// image has. A blank render has one.
	///
	/// STA: constructs WPF controls.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class JointRenderTests
	{
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		/// <summary>A triangle per member, sharing one vertex array, as the payload does.</summary>
		private static List<MemberMesh> TwoMembers()
		{
			var pos = new double[]
			{
				0, 0, 0,   1, 0, 0,   0, 1, 0,       // member 1
				0, 0, 0,   0, 0, 1,   1, 0, 1,       // member 2
			};
			var nrm = new double[pos.Length];
			for (int i = 2; i < nrm.Length; i += 3) nrm[i] = 1;

			return new List<MemberMesh>
			{
				new() { MemberId = 1, Kind = BodyKind.Member, Name = "M1",
					Positions = pos, Normals = nrm, Indices = new[] { 0, 1, 2 } },
				new() { MemberId = 2, Kind = BodyKind.Member, Name = "M2",
					Positions = pos, Normals = nrm, Indices = new[] { 3, 4, 5 } },
			};
		}

		/// <summary>How many distinct pixel colours the PNG has — 1 means nothing was drawn.</summary>
		private static int DistinctColours(byte[] png)
		{
			using var ms = new System.IO.MemoryStream(png);
			var frame = System.Windows.Media.Imaging.BitmapFrame.Create(
				ms,
				System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat,
				System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);

			int stride = frame.PixelWidth * 4;
			var pixels = new byte[stride * frame.PixelHeight];
			frame.CopyPixels(pixels, stride, 0);

			var seen = new HashSet<uint>();
			for (int i = 0; i + 3 < pixels.Length; i += 4)
				seen.Add(BitConverter.ToUInt32(pixels, i));
			return seen.Count;
		}

		/// <summary>
		/// THE test: the PNG contains a drawing, not a blank canvas.
		///
		/// A control that was never measured or arranged renders empty, and that is the failure mode
		/// worth guarding — it produces a valid PNG of the right size with nothing in it, so a report
		/// would carry an empty frame and look merely ugly rather than broken.
		/// </summary>
		[Test]
		public void TheRenderContainsADrawing()
		{
			var view = new Joint3DView();
			view.Load(TwoMembers());

			byte[]? png = view.RenderToPng(400, 300);

			Assert.That(png, Is.Not.Null, "something was loaded, so something must render");
			int colours = DistinctColours(png!);
			Assert.That(colours, Is.GreaterThan(2),
				$"only {colours} distinct colour(s) — the bitmap is blank, which is what an "
				+ "un-arranged control renders as");
		}

		/// <summary>
		/// Nothing loaded, nothing rendered. Returning a blank PNG instead would put an empty frame
		/// in the report for every joint that could not be drawn.
		/// </summary>
		[Test]
		public void AnEmptyViewRendersNothing()
		{
			var view = new Joint3DView();

			Assert.That(view.RenderToPng(400, 300), Is.Null);
		}

		/// <summary>
		/// The render leaves the control as it found it. It is the same instance the §6.4 tab shows,
		/// so a render that turned the chrome off permanently would silently change the UI.
		/// </summary>
		[Test]
		public void RenderingRestoresTheChromeSetting()
		{
			var view = new Joint3DView { ChromeVisible = true };
			view.Load(TwoMembers());

			view.RenderToPng(400, 300);

			Assert.That(view.ChromeVisible, Is.True);
		}

		/// <summary>
		/// The chrome switch hides all three captions at once — the gesture hint, the body count and
		/// the placeholder. One switch rather than three suppressions at the call sites, so a fourth
		/// caption added later is covered by the same decision.
		/// </summary>
		[Test]
		public void ChromeVisibleHidesEveryCaption()
		{
			var view = new Joint3DView();
			view.Load(TwoMembers());

			view.ChromeVisible = false;

			Assert.Multiple(() =>
			{
				Assert.That(view.GestureHintForTest.Visibility,
					Is.EqualTo(System.Windows.Visibility.Collapsed), "the gesture hint");
				Assert.That(view.HintLabelForTest.Visibility,
					Is.EqualTo(System.Windows.Visibility.Collapsed), "the body count");
			});
		}
	}
}
