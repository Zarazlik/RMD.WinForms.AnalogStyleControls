using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMD.WinForms.AnalogStyleControls
{
	public struct Arc
	{
		public float StartRadian { get; set; }
		public float EndRadian { get; private set; }
		public float SweepRadian
		{
			get { return sweepRadian; }
			set
			{
				sweepRadian = value;
				EndRadian = StartRadian + sweepRadian;
			}
		}
		float sweepRadian;

		public Arc()
		{

		}

		public Arc(float startRadian, float sweepRadian)
		{
			StartRadian = startRadian;
			SweepRadian = sweepRadian;
		}
	}
}
