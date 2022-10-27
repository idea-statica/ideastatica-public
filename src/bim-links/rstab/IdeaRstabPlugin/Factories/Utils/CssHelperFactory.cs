using Dlubal.RSTAB6;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry2D;
using IdeaRstabPlugin.Model;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

using IOM = IdeaRS.OpenModel.CrossSection;

namespace IdeaRstabPlugin.Factories.RstabPluginUtils
{
	/// <summary>
	/// RSTAB to IOM cross sections parameters helper factory
	/// </summary>
	internal static class CssHelperFactory
	{
		private const double kX = 0.001;
		private const double kY = -0.001;

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssIw(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);
			double b = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_b);
			double tf = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g);
			double tw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s);

			IOM.CrossSectionFactory.FillWeldedI(crossSectionParameter, b, h, tw, tf);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssIwn(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);
			double btw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_bu);
			double tfw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_bo);
			double tfThickness = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_to);
			double bfThickness = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_tu);
			double tw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_s);

			IOM.CrossSectionFactory.FillWeldedAsymI(crossSectionParameter, btw, tfw, h - tfThickness - bfThickness, tw, tfThickness, bfThickness);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssBoxFl(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);
			double btw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_bu);
			double tfw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_bo);
			double tfThickness = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_to);
			double bfThickness = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_tu);
			double tw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_s);
			double bi = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_bi);

			IOM.CrossSectionFactory.FillWeldedBoxFlange(crossSectionParameter, btw, tfw, h - tfThickness - bfThickness, bi, tw, tfThickness, bfThickness);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void CreateWeldedT(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);
			double b = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_b);
			double tf = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_f);
			double tw = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_w);

			IOM.CrossSectionFactory.FillWeldedT(crossSectionParameter, b, h, tw, tf);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssI(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);
			double b = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_b);
			double flangeTh = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g);
			double webTh = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s);
			double r = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r);
			double r1 = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1);

			CURVE_2D[] shape = cssDB.rsGetShape();
			IList<PolyLine2D> outlines = GetPolyLineShape(shape, BOUND_TYPE.BT_OUTER);

			double taperAngle = 0.0;
			if (outlines[0].Segments.Count == 20)
			{
				double x1 = outlines[0].Segments[2].EndPoint.X;
				double x2 = outlines[0].Segments[1].EndPoint.X;
				double y1 = outlines[0].Segments[2].EndPoint.Y;
				double y2 = outlines[0].Segments[1].EndPoint.Y;
				taperAngle = Math.Abs(Math.Atan(Math.Abs(y1 - y2) / Math.Abs(x1 - x2)));
			}

			IOM.CrossSectionFactory.FillRolledI(crossSectionParameter, b, h, webTh, flangeTh, r, taperAngle, r1);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssU(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);
			double b = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_b);
			double flangeTh = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g);
			double webTh = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s);
			double r = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r);
			double r1 = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1);

			CURVE_2D[] shape = cssDB.rsGetShape();
			IList<PolyLine2D> outlines = GetPolyLineShape(shape, BOUND_TYPE.BT_OUTER);

			double taperAngle = 0.0;
			if (outlines[0].Segments.Count == 20)
			{
				double x1 = outlines[0].Segments[2].EndPoint.X;
				double x2 = outlines[0].Segments[3].EndPoint.X;
				double y1 = outlines[0].Segments[2].EndPoint.Y;
				double y2 = outlines[0].Segments[3].EndPoint.Y;
				taperAngle = Math.Abs(Math.Atan(Math.Abs(y1 - y2) / Math.Abs(x1 - x2)));
			}

			IOM.CrossSectionFactory.FillRolledChannel(crossSectionParameter, b, h, webTh, flangeTh, r, r1, taperAngle);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssL(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);
			double b = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_b);
			double webTh = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_s);
			double r = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r);
			double r1 = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1);

			IOM.CrossSectionFactory.FillRolledAngle(crossSectionParameter, b, h, webTh, r, r1, 0.0);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssRHS(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double b = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_b);
			double h = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_h);

			if (h == 0)
			{
				h = b;
			}

			double webTh = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s);

			if (webTh == 0)
			{
				webTh = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_s);
			}

			double ri = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r);
			double ro = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_r_o);

			if (ro == 0.0)
			{
				ro = 1e-6;
			}

			IOM.CrossSectionFactory.FillRolledRHS(crossSectionParameter, h, b, webTh, ri, ro, 0.0);
		}

		/// <summary>
		/// Filles instance of CrossSectionParameter
		/// </summary>
		/// <param name="crossSectionParameter">Instance of CrossSectionParameter</param>
		/// <param name="css">RSTAB cross-section</param>
		internal static void FillCssCHS(CrossSectionParameter crossSectionParameter, Dlubal.RSTAB8.ICrossSection css)
		{
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();

			double b = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_D);
			double t = cssDB.rsGetProperty(DB_CRSC_PROPERTY_ID.CRSC_PROP_s);

			IOM.CrossSectionFactory.FillRolledCHS(crossSectionParameter, b * 0.5, t);
		}

		/// <summary>
		/// Creates cross-section made of multiple components
		/// </summary>
		/// <param name="css">RSTAB cross-section</param>
		/// <param name="objectFactory">IObjectFactory instance</param>
		/// <returns>Instance of IIdeaCrossSection</returns>
		internal static IIdeaCrossSection CreateCrossSectionByComponents(Dlubal.RSTAB8.ICrossSection css, IObjectFactory objectFactory)
		{
			Dlubal.RSTAB8.CrossSection cssData = css.GetData();
			IrsCrossSectionDB2 cssDB = (IrsCrossSectionDB2)css.GetDatabaseCrossSection();
			CURVE_2D[] shape = cssDB.rsGetShape();

			List<PolyLine2D> outlines = GetPolyLineShape(shape, BOUND_TYPE.BT_OUTER);
			List<PolyLine2D> openings = GetPolyLineShape(shape, BOUND_TYPE.BT_INNER);

			IIdeaCrossSectionByComponents crossSection = new IdeaCrossSectionByComponents()
			{
				Id = $"css-{cssData.No}",
				Name = cssData.Description
			};

			for (int i = 0; i < outlines.Count; i++)
			{
				//in connection we don't support complex shape only rectangle
				if (outlines[i].Segments.Count > 4)
				{
					return null;
				}
				Region2D region = new Region2D
				{
					Outline = outlines[i],
					Openings = openings.Count == 0 ? null : openings
				};

				IIdeaCrossSectionComponent component = new IdeaCrossSectionComponent
				{
					Geometry = region,
					Material = objectFactory.GetMaterial(cssData.MaterialNo),
				};
				crossSection.Components.Add(component);
			}

			return crossSection;
		}

		/// <summary>
		/// Gets polyline from RSTAB cross-section shape
		/// </summary>
		/// <param name="shape">RSTAB CURVE_2D[] shape</param>
		/// <param name="maybound">RSTAB BOUND_TYPE enum</param>
		/// <returns>RSTAB cross-section polyline</returns>
		internal static List<PolyLine2D> GetPolyLineShape(CURVE_2D[] shape, BOUND_TYPE maybound)
		{
			List<PolyLine2D> retList = new List<PolyLine2D>();

			PolyLine2D polyline = null;
			bool firstRun = true;
			int numCurves = shape.Length;

			for (int i = 0; i < numCurves; i++)
			{
				if (shape[i].bound == maybound)
				{
					IPOINT_2D[] points = shape[i].arrPoints;
					int numPoints = points.Length;

					if (firstRun && (shape[i].type == CURVE_TYPE.CT_CIRCLE))
					{
						polyline = new PolyLine2D
						{
							StartPoint = new Point2D { X = points[1].x * kX, Y = points[1].y * kY }
						};

						Point2D center = new Point2D { X = points[0].x * kX, Y = points[0].y * kY };
						double radius = GetCircleRadius(points[0], points[1]);

						polyline.Segments.Add(new ArcSegment2D
						{
							Point = new Point2D { X = center.X, Y = center.Y + radius },
							EndPoint = new Point2D { X = center.X - radius, Y = center.Y }
						});

						polyline.Segments.Add(new ArcSegment2D
						{
							Point = new Point2D { X = center.X, Y = center.Y - radius },
							EndPoint = new Point2D { X = center.X + radius, Y = center.Y }
						});

						retList.Add(polyline);

						continue;
					}

					if (firstRun)
					{
						polyline = new PolyLine2D { StartPoint = new Point2D { X = points[0].x * kX, Y = points[0].y * kY } };
						firstRun = false;
						retList.Add(polyline);
					}

					if (polyline != null)
					{
						double ex = points[numPoints - 1].x * kX;
						double ey = points[numPoints - 1].y * kY;
						if ((Math.Abs(ex - polyline.StartPoint.X) < 1e-3)
							&& (Math.Abs(ey - polyline.StartPoint.Y) < 1e-3))
						{
							firstRun = true;
						}
					}

					switch (shape[i].type)
					{
						case CURVE_TYPE.CT_LINE:
							if (numPoints == 2)
							{
								Segment2D seg = new LineSegment2D { EndPoint = new Point2D { X = points[1].x * kX, Y = points[1].y * kY } };
								polyline.Segments.Add(seg);
							}
							break;

						case CURVE_TYPE.CT_ARC:
							if (numPoints == 3)
							{
								Segment2D seg = new ArcSegment2D
								{
									Point = new Point2D
									{
										X = points[1].x * kX,
										Y = points[1].y * kY
									},
									EndPoint = new Point2D
									{
										X = points[2].x * kX,
										Y = points[2].y * kY
									}
								};
								polyline.Segments.Add(seg);
							}
							break;
					}
				}
			}

			return retList;
		}

		private static double GetCircleRadius(IPOINT_2D centre, IPOINT_2D point)
		{
			double dx = (point.x - centre.x) * kX;
			double dy = (point.y - centre.y) * kY;
			return Math.Sqrt(dx * dx + dy * dy);
		}
	}
}