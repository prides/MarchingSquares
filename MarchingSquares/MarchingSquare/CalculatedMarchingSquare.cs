using System.Collections.Generic;
using System.Windows;
using MarchingSquares.Shapes;
using MarchingSquares.Models;

namespace MarchingSquares.MarchingSquare
{
	class CalculatedMarchingSquare : MarchingSquareBase
	{
		static int squareId = 0;

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
				if (this.mLineShape != value)
				{
					this.mLineShape = value;
					this.RefreshMarchingSquare();
				}
			}
		}

		//private List<ShapeBase> mLines = new List<ShapeBase>();
		Line mFirst;
		Line mSecond;

		public CalculatedMarchingSquare(Point position, Size squareSize, CalculatedMarchingSquare top, CalculatedMarchingSquare left)
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

			//this.mLines.Add(new Line(this.LT.Coordinate, this.RB.Coordinate));
			//this.mLines.Add(new Line(this.RT.Coordinate, this.LB.Coordinate));
			this.mFirst = new Line(this.LT.Coordinate, this.RB.Coordinate);
			this.mSecond = new Line(this.RT.Coordinate, this.LB.Coordinate);

			//this.mLines[0].WinShape.Stroke = System.Windows.Media.Brushes.Green;
			//this.mLines[1].WinShape.Stroke = System.Windows.Media.Brushes.Green;

			this.mFirst.WinShape.Stroke = System.Windows.Media.Brushes.Green;
			this.mSecond.WinShape.Stroke = System.Windows.Media.Brushes.Green;

			RefreshMarchingSquare();
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

			if (this.LT.Force > 1.0d)
			{
				value |= 8;
			}

			if (this.RT.Force > 1.0d)
			{
				value |= 4;
			}

			if (this.RB.Force > 1.0d)
			{
				value |= 2;
			}

			if (this.LB.Force > 1.0d)
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

			switch (this.LineShape)
			{
				case LineShapes.All:
				case LineShapes.Empty:
					first.WinShape.Visibility = Visibility.Hidden;
					second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.BottomLeft:
				case LineShapes.AllButButtomLeft:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.LB.Coordinate.X, this.Position.Y);
					first.End = new Point(this.Position.X, this.LB.Coordinate.Y);
					second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.BottomRight:
				case LineShapes.AllButButtomRight:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.Position.X, this.RB.Coordinate.Y);
					first.End = new Point(this.RB.Coordinate.X, this.Position.Y);
					second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.Bottom:
				case LineShapes.Top:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.LB.Coordinate.X, this.Position.Y);
					first.End = new Point(this.RB.Coordinate.X, this.Position.Y);
					second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.TopRight:
				case LineShapes.AllButTopRight:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.Position.X, this.RT.Coordinate.Y);
					first.End = new Point(this.RT.Coordinate.X, this.Position.Y);
					second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.TopRightBottomLeft:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.LB.Coordinate.X, this.Position.Y);
					first.End = new Point(this.Position.X, this.LB.Coordinate.Y);

					second.WinShape.Visibility = Visibility.Visible;
					second.Begin = new Point(this.Position.X, this.RT.Coordinate.Y);
					second.End = new Point(this.RT.Coordinate.X, this.Position.Y);
					break;

				case LineShapes.Right:
				case LineShapes.Left:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.Position.X, this.RT.Coordinate.Y);
					first.End = new Point(this.Position.X, this.RB.Coordinate.Y);
					second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.AllButTopLeft:
				case LineShapes.TopLeft:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.LT.Coordinate.X, this.Position.Y);
					first.End = new Point(this.Position.X, this.LT.Coordinate.Y);
					second.WinShape.Visibility = Visibility.Hidden;
					break;

				case LineShapes.TopLeftBottomRight:
					first.WinShape.Visibility = Visibility.Visible;
					first.Begin = new Point(this.LB.Coordinate.X, this.Position.Y);
					first.End = new Point(this.Position.X, this.LB.Coordinate.Y);

					second.WinShape.Visibility = Visibility.Visible;
					second.Begin = new Point(this.Position.X, this.RT.Coordinate.Y);
					second.End = new Point(this.RT.Coordinate.X, this.Position.Y);
					break;

				default:
					first.WinShape.Visibility = Visibility.Hidden;
					second.WinShape.Visibility = Visibility.Hidden;
					break;
			}
		}
	}
}
