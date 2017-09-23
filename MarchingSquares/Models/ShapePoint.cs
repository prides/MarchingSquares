using System.Windows;

namespace MarchingSquares.Models
{
	internal class ShapePoint
	{
		public int ParentID
		{
			get;
			private set;
		}

		public Point Coordinate { get; set; }

		public double Force { get; set; }

		public ShapePoint(double x, double y, int parent_id)
		{
			this.Coordinate = new Point(x, y);

			this.ParentID = parent_id;
		}
	}
}
