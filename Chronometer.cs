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

		#region Value
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

				NeedleAngle = GetAngelOfValue(value);
				Refresh();
			}
		}
		float value;
		public float NeedleAngle { get; private set; }
		float valueAngeleStep;
		#endregion

		#region Positioning
		Arc needleArc = new Arc(150, 180);
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
		#endregion

		#region Scale
		[Category("Scale")]
		public float MinValue { get; set; } = 0;
		[Category("Scale")]
		public float MaxValue { get; set; } = 100;

		[Category("Scale")]
		public Color ScaleColor {  get; set; } = Color.Black;
		[Category("Scale")]
		public Color NeedleColor { get; set; } = Color.Red;
		[Category("Scale")]
		public float  NeedleWidth { get; set; }= 2.5f;

		[Category("Scale")]
		public float ScaleRodsStep { get; set; } = 1;
		[Category("Scale")]
		public int AccentScaleRodsStep { get; set; } = 5;
		[Category("Scale")]
		public int SubAccentScaleRodsStep { get; set; } = 10;

		[Category("Scale")]
		public Point[] ColorScaleLineValues { get; set; } = new Point[]
		{
		new Point(0, 20),
		new Point(20, 75),
		new Point(75, 90),
		new Point(90, 100)
		};
		[Category("Scale")]
		public Color[] ColorScaleLineColors { get; set; } = new Color[]
		{
		Color.Blue,
		Color.Green,
		Color.Orange,
		Color.Red
		};
		[Category("Scale")]
		public float IndentColorScaleLine { get; set; } = 25;

		[Category("Scale")]
		public float IndentNeedle { get; set; } = 12;
		[Category("Scale")]
		public float IndentRods { get; set; } = 10;
		[Category("Scale")]
		public float IndentAccentRods { get; set; } = 15;
		[Category("Scale")]
		public float IndentSubAccentRods { get; set; } = 15;
		[Category("Scale")]
		public float ColorScaleLineWidth { get; set; } = 5;
		#endregion

		Bitmap scaleBackground;
		PointF center;
		int maxRadius;
		bool redrawBackground = true;

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

			SetValueAngeleStep();

			#endregion

			if (redrawBackground)
			{
				RedrawScaleBackground();
				redrawBackground = false;
			}

			g.DrawImage(scaleBackground, new Point(0,0));
			DrawNeedle(g, center, maxRadius);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			redrawBackground = true;
			Refresh();
		}

		#region Drawning

		protected void RedrawScaleBackground()
		{
			center = new PointF(this.Width / 2, this.Height / 2);
			maxRadius = Math.Min((this.Width / 2) - 3, (this.Height / 2) - 3);

			scaleBackground = new Bitmap(this.Width, this.Height);

			using (Graphics g = Graphics.FromImage(scaleBackground)) 
			{
				g.SmoothingMode = SmoothingMode.HighQuality;

				DrawColoredScaleLines(g, center, maxRadius);
				DrawScale(g, center, maxRadius);
			}
		}

		protected void DrawScale(Graphics g, PointF center, float maxRadius)
		{
			DrawRods();
			DrawArcLine(g, new Pen(ScaleColor), center, maxRadius, StartNeedleAngle, SweepNeedleAngle);

			void DrawRods()
			{
				uint rodsCount = (uint)((MaxValue - MinValue) / ScaleRodsStep);
				float step = SweepNeedleAngle / rodsCount;

				for (int i = 0, accent = AccentScaleRodsStep, subAccent = SubAccentScaleRodsStep; i <= rodsCount; i++, accent++, subAccent++)
				{
					if (accent == AccentScaleRodsStep)
					{
						g.DrawLine(new Pen(ScaleColor, 3), GetPointOnCercle(center, maxRadius, StartNeedleAngle + (step * i)), GetPointOnCercle(center, maxRadius - IndentAccentRods, StartNeedleAngle + (step * i)));
						accent = 0;

						if (subAccent == SubAccentScaleRodsStep)
						{
							subAccent = 0;
						}
					}
					else if (subAccent == SubAccentScaleRodsStep)
					{
						g.DrawLine(new Pen(ScaleColor), GetPointOnCercle(center, maxRadius, StartNeedleAngle + (step * i)), GetPointOnCercle(center, maxRadius - IndentSubAccentRods, StartNeedleAngle + (step * i)));
						subAccent = 0;
					}
					else
					{
						g.DrawLine(new Pen(ScaleColor), GetPointOnCercle(center, maxRadius, StartNeedleAngle + (step * i)), GetPointOnCercle(center, maxRadius - IndentRods, StartNeedleAngle + (step * i)));
					}
				}
			}
		}

		protected void DrawNeedle(Graphics g, PointF center, float maxRadius)
		{
			g.FillEllipse(new SolidBrush(NeedleColor), center.X - 5, center.Y - 5, 10, 10);
			g.DrawLine(new Pen(Color.Red, NeedleWidth), center, GetPointOnCercle(center, maxRadius - IndentNeedle, NeedleAngle));
		}

		protected void DrawColoredScaleLines(Graphics g, PointF center, float maxRadius)
		{
			for (int i = 0; i < ColorScaleLineColors.Length && i < ColorScaleLineValues.Length; i++)
			{
				DrawArcLine(
					g,
					new Pen(ColorScaleLineColors[i], ColorScaleLineWidth),
					center,
					maxRadius - IndentColorScaleLine,
					GetAngelOfValue(ColorScaleLineValues[i].X),
					(ColorScaleLineValues[i].Y - ColorScaleLineValues[i].X) * valueAngeleStep
					);
			}
		}

		protected void DrawArcLine(Graphics g, Pen pen, PointF center, float radius, float startAngle, float sweepAngle)
		{
			RectangleF rect = new RectangleF(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);

			g.DrawArc(pen, rect, startAngle, sweepAngle);
		}

		#endregion

		PointF GetPointOnCercle(PointF center, float radius, float angle)
		{
			float alphaRadians = (float)(angle * Math.PI / 180);

			float x = (float)(center.X + radius * Math.Cos(alphaRadians));
			float y = (float)(center.Y + radius * Math.Sin(alphaRadians));

			return new PointF(x, y);
		}

		private float GetAngelOfValue(float value)
		{
			return StartNeedleAngle + (valueAngeleStep * (MinValue + value));
		}

		private void SetValueAngeleStep()
		{
			valueAngeleStep = (EndNeedleAngle - StartNeedleAngle) / (MaxValue - MinValue);
		}
	}
}
