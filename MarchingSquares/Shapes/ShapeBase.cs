using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace MarchingSquares.Shapes
{
	public class ShapeBase
	{
		protected Point mPosition;

		protected int mZOrder;
		public int ZOrder
		{
			get { return mZOrder; }
			set
			{
				this.mZOrder = value;
				if (null != this.mWinShape)
				{
					Canvas.SetZIndex(this.mWinShape, this.mZOrder);
				}
			}
		}

		protected Shape mWinShape;
		public Shape WinShape
		{
			get { return this.mWinShape; }
			set { this.mWinShape = value; }
		}
	}
}
