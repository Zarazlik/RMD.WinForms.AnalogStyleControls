using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace RMD.WinForms.AnalogStyleControls
{
	public class Chronometer : Control
	{
		#region Propertyes/Variable
		public float Value
		{
			get { return value; }
			set
			{
				if (value > MaxValue)
				{
					this.value = MaxValue;
				}
				else if (value < MinValue)
				{
					this.value = MinValue;
				}
				else
				{
					this.value = value;
				}

				NeedleAngle = GetNeedleAngel(StartNeedleAngle, EndNeedleAngle, MinValue, MaxValue);
				Refresh();
			}
		}
		float value;
		public float NeedleAngle { get; private set; }

		Arc needleArc;
		[Category("Positioning")]
		public float StartNeedleAngle 
		{ 
			get { return needleArc.StartRadian; }
			set { needleArc.StartRadian = value; }
		}
		[Category("Positioning")]
		public float EndNeedleAngle 
		{ 
			get { return needleArc.EndRadian; }
		}
		[Category("Positioning")]
		public float SweepNeedleAngle
		{
			get { return needleArc.SweepRadian; }
			set { needleArc.SweepRadian = value; }
		}

		[Category("Scale")]
		public float MinValue { get; set; }		
		[Category("Scale")]
		public float MaxValue { get; set; }

		[Category("Scale")]
		public float ScaleRodsStep { get; set; }
		[Category("Scale")]
		public int AccentScaleRodsStep { get; set; }
		[Category("Scale")]
		public int SubAccentScaleRodsStep { get; set; }

		[Category("Scale")]
		public Point[] ColorScaleLineValues { get; set; }
		[Category("Scale")]
		public Color[] ColorScaleLineColors { get; set; }

		[Category("Scale")]
		float IndentNeedle { get; set; } = 12;
		[Category("Scale")]
		float IndentRods { get; set; } = 10;
		[Category("Scale")]
		float IndentAccentRods { get; set; } = 15;
		[Category("Scale")]
		float IndentSubAccentRods { get; set; } = 15;

		#endregion

		public Chronometer() 
		{ 
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			#region Preparation
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;

			PointF center = new PointF(this.Width / 2, this.Height / 2);
			
			int maxRadius = Math.Min((this.Width / 2) - 3, (this.Height / 2) - 3);
			#endregion

			DrawColoredScaleLines();
			DrawScale();
			DrawNeedle();

			#region Methods

			PointF GetPointOnCercle(PointF center, float radius, float angle)
			{
				float alphaRadians = (float)(angle * Math.PI / 180);

				float x = (float)(center.X + radius * Math.Cos(alphaRadians));
				float y = (float)(center.Y + radius * Math.Sin(alphaRadians));

				return new PointF(x, y);
			}

			void DrawScale()
			{
				DrawRods();
				DrawArcLine(new Pen(Color.Black), center, maxRadius, StartNeedleAngle, SweepNeedleAngle);
				
				void DrawRods()
				{
					uint rodsCount = (uint)((MaxValue - MinValue) / ScaleRodsStep);
					float step = SweepNeedleAngle / rodsCount;

					for (int i = 0, accent = AccentScaleRodsStep, subAccent = SubAccentScaleRodsStep; i <= rodsCount; i++, accent++, subAccent++)
					{
						if (accent == AccentScaleRodsStep)
						{
							g.DrawLine(new Pen(Color.Black, 3), GetPointOnCercle(center, maxRadius, StartNeedleAngle + (step * i)), GetPointOnCercle(center, maxRadius - IndentAccentRods, StartNeedleAngle + (step * i)));
							accent = 0;

							if (subAccent == SubAccentScaleRodsStep)
							{
								subAccent = 0;
							}
						}
						else if (subAccent == SubAccentScaleRodsStep)
						{
							g.DrawLine(new Pen(Color.Black), GetPointOnCercle(center, maxRadius, StartNeedleAngle + (step * i)), GetPointOnCercle(center, maxRadius - IndentSubAccentRods, StartNeedleAngle + (step * i)));
							subAccent = 0;
						}
						else
						{
							g.DrawLine(new Pen(Color.Black), GetPointOnCercle(center, maxRadius, StartNeedleAngle + (step * i)), GetPointOnCercle(center, maxRadius - IndentRods, StartNeedleAngle + (step * i)));
						}
					}
				}
			}

			void DrawNeedle()
			{
				g.FillEllipse(new SolidBrush(Color.Red), center.X - 5, center.Y - 5, 10, 10);
				g.DrawLine(new Pen(Color.Red, 2.5f), center, GetPointOnCercle(center, maxRadius - IndentNeedle, NeedleAngle));
			}

			void DrawArcLine(Pen pen, PointF center, float radius, float startAngle, float sweepAngle)
			{
				RectangleF rect = new RectangleF(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);

				g.DrawArc(pen, rect, startAngle, sweepAngle);
			}

			void DrawColoredScaleLines()
			{
				DrawArcLine(new Pen(Color.Blue, 5), center, 175, StartNeedleAngle, 29);
				DrawArcLine(new Pen(Color.Green, 5), center, 175, StartNeedleAngle + 31, 90);
				DrawArcLine(new Pen(Color.Orange, 5), center, 175, StartNeedleAngle + 122, 28);
				DrawArcLine(new Pen(Color.Red, 5), center, 175, StartNeedleAngle + 151, 29);
			}

			#endregion
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Refresh();
		}

		private float GetNeedleAngel(float minAngle, float maxAngle, float minValue, float maxValue)
		{
			float step =  (maxAngle - minAngle) / (maxValue - minValue);

			return minAngle + (step * (minValue + Value));
		}
	}
}
