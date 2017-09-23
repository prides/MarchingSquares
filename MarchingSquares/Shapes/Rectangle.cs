using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MarchingSquares.Shapes
{
	public class Rectangle : ShapeBase
	{
		public Point Position
		{
			get { return mPosition; }
			set
			{
				mPosition = value;
				Canvas.SetLeft(this.mWinShape, mPosition.X - mSize.Width / 2.0f);
				Canvas.SetTop(this.mWinShape, mPosition.Y - mSize.Height / 2.0f);
			}
		}

		protected Size mSize;
		public Size Size
		{
			get { return mSize; }
			set
			{
				mSize = value;
				this.mWinShape.Width = mSize.Width;
				this.mWinShape.Height = mSize.Height;
				this.Position = mPosition;
			}
		}

		public Rectangle(Point position, Size size)
		{
			this.WinShape = new System.Windows.Shapes.Rectangle();

			this.Size = size;

			this.Position = position;
		}
	}
}
