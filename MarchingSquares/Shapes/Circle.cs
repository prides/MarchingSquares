using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace MarchingSquares.Shapes
{
	public class Circle : ShapeBase
	{
		public Point Position
		{
			get
			{
				return mPosition;
			}
			set
			{
				mPosition = value;
				Canvas.SetLeft(this.WinShape, mPosition.X - this.Radius);
				Canvas.SetTop(this.WinShape, mPosition.Y - this.Radius);
			}
		}

		private float mRadius;
		public float Radius
		{
			get { return mRadius; }
			set
			{
				mRadius = value;
				this.WinShape.Width = Radius * 2;
				this.WinShape.Height = Radius * 2;
			}
		}

		public Circle(Point pos, float radius)
		{
			this.WinShape = new Ellipse();
			this.Radius = radius;
			this.Position = pos;

			SolidColorBrush mySolidColorBrush = new SolidColorBrush();
			mySolidColorBrush.Opacity = 0.6;

			mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
			this.WinShape.Fill = mySolidColorBrush;
			this.WinShape.StrokeThickness = 2;
			this.WinShape.Stroke = Brushes.Black;
		}
	}
}
