using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MarchingSquares.Shapes
{
	public class Line : ShapeBase
	{
		public Point Begin
		{
			get { return mPosition; }
			set
			{
				mPosition = value;
				((System.Windows.Shapes.Line)this.mWinShape).X1 = mPosition.X;
				((System.Windows.Shapes.Line)this.mWinShape).Y1 = mPosition.Y;
			}
		}

		protected Point mEnd;
		public Point End
		{
			get { return mEnd; }
			set
			{
				mEnd = value;
				((System.Windows.Shapes.Line)this.mWinShape).X2 = mEnd.X;
				((System.Windows.Shapes.Line)this.mWinShape).Y2 = mEnd.Y;
			}
		}
        static int lineNo = 0;
		public Line(Point begin, Point end)
		{
			this.WinShape = new System.Windows.Shapes.Line();

            lineNo++;

            //Console.WriteLine("Line created: " + lineNo);

			this.Begin = begin;
			this.End = end;
		}

        public void setStart(double x, double y)
        {
            ((System.Windows.Shapes.Line)this.mWinShape).X1 = mPosition.X = x;
            ((System.Windows.Shapes.Line)this.mWinShape).Y1 = mPosition.Y = y;
        }

        public void setEnd(double x, double y)
        {
            ((System.Windows.Shapes.Line)this.mWinShape).X2 = mEnd.X = x;
            ((System.Windows.Shapes.Line)this.mWinShape).Y2 = mEnd.Y = y;
        }
    }
}
