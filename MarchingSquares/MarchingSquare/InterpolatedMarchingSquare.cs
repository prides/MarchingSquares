using System;
using System.Collections.Generic;
using System.Windows;
using MarchingSquares.Shapes;
using MarchingSquares.Models;
using MarchingSquares.Utils;

namespace MarchingSquares.MarchingSquare
{
	class InterpolatedMarchingSquare : MarchingSquareBase
	{
		static int squareId = 0;

		private const double FORCE = 1.0d;

		private int mId = 0;
		public int ID
		{
			get { return this.mId; }
			private set { this.mId = value; }
		}

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
			}
		}

		private Size mSize;
		public Size Size
		{
			get { return mSize; }
			set
			{
				mSize = value;
			}
		}

		protected ShapePoint LT;    //  LT ---- RT
		protected ShapePoint RT;    //  |        |
									//  |   .C   |   C(enter) == position
		protected ShapePoint RB;    //  |        |
		protected ShapePoint LB;    //  LB ---- RB

		private LineShapes mLineShape = LineShapes.Empty;
		public LineShapes LineShape
		{
			get { return this.mLineShape; }
			set
			{
				//if (this.mLineShape != value)
				{
					this.mLineShape = value;
					this.RefreshMarchingSquare();
				}
			}
		}

		private Line mFirst;
		private Line mSecond;

		public InterpolatedMarchingSquare(Point position, Size squareSize, InterpolatedMarchingSquare top, InterpolatedMarchingSquare left, bool randomMultiplier = false)
			: base(position, squareSize)
		{
			// Increment class instance ID first
			this.ID = squareId++;

			if (top != null)
			{
				this.LT = top.LB;
				this.RT = top.RB;
			}
			else
			{
				if (left != null)
				{
					this.LT = left.RT;
				}
				else
				{
					this.LT = new ShapePoint(position.X - squareSize.Width / 2, position.Y - squareSize.Height / 2, this.ID);
				}
				this.RT = new ShapePoint(position.X + squareSize.Width / 2, position.Y - squareSize.Height / 2, this.ID);
			}

			if (left != null)
			{
				this.LT = left.RT;
				this.LB = left.RB;
			}
			else
			{
				this.LB = new ShapePoint(position.X - squareSize.Width / 2, position.Y + squareSize.Height / 2, this.ID);
			}

			this.RB = new ShapePoint(position.X + squareSize.Width / 2, position.Y + squareSize.Height / 2, this.ID);

			this.Position = position;
			this.Size = squareSize;

			this.mFirst = new Line(this.LT.Coordinate, this.RB.Coordinate);
			this.mSecond = new Line(this.RT.Coordinate, this.LB.Coordinate);

			this.mFirst.WinShape.Stroke = System.Windows.Media.Brushes.Green;
			this.mSecond.WinShape.Stroke = System.Windows.Media.Brushes.Green;

			RefreshMarchingSquare();

			double multiplier = randomMultiplier ? MS_Math.getRandomDouble(1.0, 2.0) : 1.0;

			this.IsUseMultiplayer = randomMultiplier;
			this.CurrentMultiplier = multiplier;
			this.MinMultiplier = multiplier - 0.5d;
			this.MaxMultiplier = multiplier + 0.5d;
			this.MultiplierStep = multiplier / 1000;
			if (new Random().Next(100) < 50) this.MultiplierStep *= -1;
		}

		public override List<ShapeBase> getWinShapes()
		{
			List<ShapeBase> result = new List<ShapeBase>();
			result.Add(this.mFirst);
			result.Add(this.mSecond);
			result.Add(this.mGrid);
			return result;
		}

		public override void Update(float deltaTime, List<Circle> circles)
		{
			this.CurrentMultiplier += this.MultiplierStep;
			if ((this.CurrentMultiplier >= this.MaxMultiplier) || (this.CurrentMultiplier <= this.MinMultiplier))
				this.MultiplierStep *= -1;

			if (this.LT.ParentID == this.mId) this.LT.Force = CalcPointValue(this.LT.Coordinate, circles);
			if (this.RT.ParentID == this.mId) this.RT.Force = CalcPointValue(this.RT.Coordinate, circles);
			if (this.RB.ParentID == this.mId) this.RB.Force = CalcPointValue(this.RB.Coordinate, circles);
			if (this.LB.ParentID == this.mId) this.LB.Force = CalcPointValue(this.LB.Coordinate, circles);

			this.LineShape = this.defineShape();
		}

		public void RefreshMarchingSquare()
		{
			this.setLineCoordinatesPerPattern();
		}

		private LineShapes defineShape()
		{
			int value = 0;

			if (this.LT.Force >= FORCE)
			{
				value |= 8;
			}

			if (this.RT.Force >= FORCE)
			{
				value |= 4;
			}

			if (this.RB.Force >= FORCE)
			{
				value |= 2;
			}

			if (this.LB.Force >= FORCE)
			{
				value |= 1;
			}

			return (LineShapes)value;
		}

		private void setLineCoordinatesPerPattern()
		{
			//Line first  = (Line) this.mLines[0];
			//Line second = (Line) this.mLines[1];

			Line first = this.mFirst;
			Line second = this.mSecond;

			double x, y;

			switch (this.LineShape)
			{
				case LineShapes.All:
				case LineShapes.Empty:
					if (first.WinShape.Visibility != Visibility.Hidden) first.WinShape.Visibility = Visibility.Hidden;
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.BottomLeft:
				case LineShapes.AllButButtomLeft:
					if (first.WinShape.Visibility != Visibility.Visible) first.WinShape.Visibility = Visibility.Visible;
					this.interpolateVertical(this.LT, this.LB, out x, out y);
					first.setStart(x, y);
					this.interpolateHorisontal(this.LB, this.RB, out x, out y);
					first.setEnd(x, y);
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.BottomRight:
				case LineShapes.AllButButtomRight:
					if (first.WinShape.Visibility != Visibility.Visible) first.WinShape.Visibility = Visibility.Visible;
					this.interpolateHorisontal(this.LB, this.RB, out x, out y);
					first.setStart(x, y);
					this.interpolateVertical(this.RT, this.RB, out x, out y);
					first.setEnd(x, y);
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.Bottom:
				case LineShapes.Top:
					if (first.WinShape.Visibility != Visibility.Visible) first.WinShape.Visibility = Visibility.Visible;
					this.interpolateVertical(this.LT, this.LB, out x, out y);
					first.setStart(x, y);
					this.interpolateVertical(this.RT, this.RB, out x, out y);
					first.setEnd(x, y);
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.TopRight:
				case LineShapes.AllButTopRight:
					first.WinShape.Visibility = Visibility.Visible;
					this.interpolateHorisontal(this.LT, this.RT, out x, out y);
					first.setStart(x, y);
					this.interpolateVertical(this.RT, this.RB, out x, out y);
					first.setEnd(x, y);
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.TopRightBottomLeft:
					if (first.WinShape.Visibility != Visibility.Visible) first.WinShape.Visibility = Visibility.Visible;
					this.interpolateVertical(this.LT, this.LB, out x, out y);
					first.setStart(x, y);
					this.interpolateHorisontal(this.LT, this.RT, out x, out y);
					first.setEnd(x, y);

					if (second.WinShape.Visibility != Visibility.Visible) second.WinShape.Visibility = Visibility.Visible;
					this.interpolateHorisontal(this.LB, this.RB, out x, out y);
					second.setStart(x, y);
					this.interpolateVertical(this.RT, this.RB, out x, out y);
					second.setEnd(x, y);
					break;

				case LineShapes.Right:
				case LineShapes.Left:
					if (first.WinShape.Visibility != Visibility.Visible) first.WinShape.Visibility = Visibility.Visible;
					this.interpolateHorisontal(this.LT, this.RT, out x, out y);
					first.setStart(x, y);
					this.interpolateHorisontal(this.LB, this.RB, out x, out y);
					first.setEnd(x, y);
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.AllButTopLeft:
				case LineShapes.TopLeft:
					if (first.WinShape.Visibility != Visibility.Visible) first.WinShape.Visibility = Visibility.Visible;
					this.interpolateVertical(this.LT, this.LB, out x, out y);
					first.setStart(x, y);
					this.interpolateHorisontal(this.LT, this.RT, out x, out y);
					first.setEnd(x, y);
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.TopLeftBottomRight:
					if (first.WinShape.Visibility != Visibility.Visible) first.WinShape.Visibility = Visibility.Visible;
					this.interpolateVertical(this.LT, this.LB, out x, out y);
					first.setStart(x, y);
					this.interpolateHorisontal(this.LB, this.RB, out x, out y);
					first.setEnd(x, y);

					if (second.WinShape.Visibility != Visibility.Visible) second.WinShape.Visibility = Visibility.Visible;
					this.interpolateHorisontal(this.LT, this.RT, out x, out y);
					second.setStart(x, y);
					this.interpolateVertical(this.RT, this.RB, out x, out y);
					second.setEnd(x, y);
					break;

				default:
					if (first.WinShape.Visibility != Visibility.Hidden) first.WinShape.Visibility = Visibility.Hidden;
					if (second.WinShape.Visibility != Visibility.Hidden) second.WinShape.Visibility = Visibility.Hidden;
					break;
			}
		}

		private void interpolateVertical(ShapePoint start, ShapePoint end, out double x, out double y)
		{
			double top = 1 - start.Force;
			double bottom = end.Force - start.Force;

			y = start.Coordinate.Y + (end.Coordinate.Y - start.Coordinate.Y) * (top / bottom);
			x = start.Coordinate.X;
		}

		private void interpolateHorisontal(ShapePoint start, ShapePoint end, out double x, out double y)
		{
			double top = 1 - start.Force;
			double bottom = end.Force - start.Force;

			x = start.Coordinate.X + (end.Coordinate.X - start.Coordinate.X) * (top / bottom);
			y = start.Coordinate.Y;
		}
	}
}
