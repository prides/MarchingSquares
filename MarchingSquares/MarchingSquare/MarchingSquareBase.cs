using System.Collections.Generic;
using System.Windows;
using MarchingSquares.Shapes;
using MarchingSquares.Utils;

namespace MarchingSquares.MarchingSquare
{
	public abstract class MarchingSquareBase
	{
		public abstract Point Position { get; set; }

		protected double CurrentMultiplier = 1.0d;
		protected double MaxMultiplier = 1.5d;
		protected double MinMultiplier = 0.5d;
		protected double MultiplierStep = 0.01d;
		protected bool IsUseMultiplayer = false;

		protected Rectangle mGrid;

		protected bool mDrawGrid = true;
		public bool DrawGrid
		{
			get { return this.mDrawGrid; }
			set
			{
				this.mDrawGrid = value;
				this.mGrid.WinShape.Visibility = value ? Visibility.Visible : Visibility.Hidden;
			}
		}

		public abstract List<ShapeBase> getWinShapes();

		public MarchingSquareBase(Point position, Size squareSize)
		{
			this.mGrid = new Rectangle(position, squareSize);
			this.mGrid.WinShape.Stroke = System.Windows.Media.Brushes.LightGray;
			this.mGrid.ZOrder = 10;
		}

		public abstract void Update(float deltaTime, List<Circle> circles);

		protected virtual double CalcPointValue(Point point, List<Circle> circles)
		{
			double result = 0.0;
			double value;
			foreach (Circle circle in circles)
			{
				value = MS_Math.MarchingValue(point, circle.Position, circle.Radius);
				//if (value > result)
				//{
				result += value * (this.IsUseMultiplayer ? this.CurrentMultiplier : 1);
				//}
			}
			return result;
		}
	}
}
