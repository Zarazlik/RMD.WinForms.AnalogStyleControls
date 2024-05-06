using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

				NeedleAngle = GetNeedleAngel(MinNeedleAngle, MaxNeedleAngle, MinValue, MaxValue);
				Refresh();
			}
		}
		float value;

		public float NeedleAngle { get; private set; }

		[Category("Behavior")]
		public float MinNeedleAngle { get; set; }
		[Category("Behavior")]
		public float MaxNeedleAngle { get; set; }

		[Category("Behavior")]
		public float MinValue { get; set; }		
		[Category("Behavior")]
		public float MaxValue { get; set; }

		public Chronometer() 
		{ 
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			PointF center = new PointF(this.Width / 2, this.Height / 2);

			g.FillEllipse(new SolidBrush(Color.Red), center.X - 5, center.Y - 5,  10, 10);

			g.DrawLine(new Pen(Color.Blue), center, GetPointOnCercle(center, 100, NeedleAngle));
			g.DrawLine(new Pen(Color.Black), center, GetPointOnCercle(center, 100, MinNeedleAngle));
			g.DrawLine(new Pen(Color.Black), center, GetPointOnCercle(center, 100, MaxNeedleAngle));

			DrawArcLine(new Pen(Color.Green), center, 120, MinNeedleAngle, 180);

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
		}

		private float GetNeedleAngel(float minAngle, float maxAngle, float minValue, float maxValue)
		{
			float step = (minAngle - maxAngle) / (maxValue - minValue);

			return minAngle - (step * (maxValue - Value));
		}
	}
}
