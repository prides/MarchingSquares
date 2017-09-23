using System.Collections.Generic;
using System.Windows;
using MarchingSquares.Shapes;

namespace MarchingSquares.MarchingSquare
{
	class SimpleMarchingSquare : MarchingSquareBase
	{
		private double mMarchingValue = 0.0;
		public double MarchingValue
		{
			get { return mMarchingValue; }
			set
			{
				mMarchingValue = value;
				RefreshMarchingSquare();
			}
		}

		private Point mPosition;
		public override Point Position
		{
			get { return mPosition; }
			set
			{
				this.mPosition = value;
				this.mSquare.Position = this.mPosition;
			}
		}

		private Size mSize;
		public Size Size
		{
			get { return mSize; }
			set
			{
				mSize = value;
				this.mSquare.Size = this.mSize;
			}
		}

		private Rectangle mSquare;

		public SimpleMarchingSquare(Point position, Size squareSize) : base(position, squareSize)
		{
			mSquare = new Rectangle(position, squareSize);

			this.Position = position;
			this.Size = squareSize;

			RefreshMarchingSquare();
		}

		public override List<ShapeBase> getWinShapes()
		{
			List<ShapeBase> result = new List<ShapeBase>();

			result.Add(mSquare);
			result.Add(mGrid);

			return result;
		}

		public override void Update(float deltaTime, List<Circle> circles)
		{
			this.MarchingValue = CalcPointValue(Position, circles);
		}

		public void RefreshMarchingSquare()
		{
			mSquare.WinShape.Fill = mMarchingValue > 1.0 ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Transparent;
			//mSquare.WinShape.Stroke = mDrawGrid ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent;
		}
	}
}
