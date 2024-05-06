using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace RMD.WinForms.AnalogStyleControls
{
	public class Chronometer : Control
	{
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

		[Category("Behavior")]
		public float StartNeedleAngle { get; set; }
		[Category("Behavior")]
		public float EndNeedleAngle { get; private set; }
		[Category("Behavior")]
		public float SweepNeedleAngle
		{
			get { return sweepNeedleAngle; }
			set 
			{ 
				sweepNeedleAngle = value;
				EndNeedleAngle = StartNeedleAngle + sweepNeedleAngle;
			}
		}
		float sweepNeedleAngle;

		[Category("Behavior")]
		public float MinValue { get; set; }		
		[Category("Behavior")]
		public float MaxValue { get; set; }

		[Category("Behavior")]
		public float ScaleRodsStep { get; set; }
		[Category("Behavior")]
		public int AccentScaleRodsStep { get; set; }
		[Category("Behavior")]
		public int SubAccentScaleRodsStep { get; set; }

		public Chronometer() 
		{ 
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.HighQuality;

			PointF center = new PointF(this.Width / 2, this.Height / 2);

			DrawArcLine(new Pen(Color.Blue, 5), center, 175, StartNeedleAngle, 29);
			DrawArcLine(new Pen(Color.Green, 5), center, 175, StartNeedleAngle + 31, 90);
			DrawArcLine(new Pen(Color.Orange, 5), center, 175, StartNeedleAngle + 122, 28);
			DrawArcLine(new Pen(Color.Red, 5), center, 175, StartNeedleAngle + 151, 29);
			DrawRods();

			g.FillEllipse(new SolidBrush(Color.Red), center.X - 5, center.Y - 5,  10, 10);
			g.DrawLine(new Pen(Color.Red, 2.5f), center, GetPointOnCercle(center, 188, NeedleAngle));

			DrawArcLine(new Pen(Color.Black), center, 200, StartNeedleAngle, SweepNeedleAngle);


			PointF GetPointOnCercle(PointF center, float radius, float angle)
			{
				float alphaRadians = (float)(angle * Math.PI / 180);

				float x = (float)(center.X + radius * Math.Cos(alphaRadians));
				float y = (float)(center.Y + radius * Math.Sin(alphaRadians));

				return new PointF(x, y);
			}

			void DrawArcLine(Pen pen, PointF center, float radius, float startAngle, float sweepAngle)
			{
				RectangleF rect = new RectangleF(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);

				g.DrawArc(pen, rect, startAngle, sweepAngle);
			}

			void DrawRods()
			{
				uint rodsCount = (uint)((MaxValue - MinValue) / ScaleRodsStep);
				float step = SweepNeedleAngle / rodsCount;

				for (int i = 0, accent = AccentScaleRodsStep, subAccent = SubAccentScaleRodsStep; i <= rodsCount; i++, accent++, subAccent++)
				{
					if (accent == AccentScaleRodsStep) 
					{
						g.DrawLine(new Pen(Color.Black, 3), GetPointOnCercle(center, 200, StartNeedleAngle + (step * i)), GetPointOnCercle(center, 185, StartNeedleAngle + (step * i)));
						accent = 0;

						if (subAccent == SubAccentScaleRodsStep)
						{
							subAccent = 0;
						}
					}
					else if (subAccent == SubAccentScaleRodsStep)
					{
						g.DrawLine(new Pen(Color.Black), GetPointOnCercle(center, 200, StartNeedleAngle + (step * i)), GetPointOnCercle(center, 185, StartNeedleAngle + (step * i)));
						subAccent = 0;
					}
					else
					{
						g.DrawLine(new Pen(Color.Black), GetPointOnCercle(center, 200, StartNeedleAngle + (step * i)), GetPointOnCercle(center, 190, StartNeedleAngle + (step * i)));
					}
					
				}
			}
		}

		private float GetNeedleAngel(float minAngle, float maxAngle, float minValue, float maxValue)
		{
			float step =  (maxAngle - minAngle) / (maxValue - minValue);

			return minAngle + (step * (minValue + Value));
		}
	}
}
