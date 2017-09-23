using System;
using System.Windows;

namespace MarchingSquares.Utils
{
	public class MS_Math
	{
		private static Random mRandom = new Random();

		static public Point getRandomPoint(Size border)
		{
			Point result;

			result = new Point(mRandom.NextDouble() * border.Width, mRandom.NextDouble() * border.Height);

			if (result.X == double.NaN || result.Y == double.NaN)
			{
				result = getRandomPoint(border);
			}

			return result;
		}

		static public double getRandomDouble(double begin, double end)
		{
			double result = mRandom.NextDouble() * (end - begin);
			return result + begin;
		}

		static public double MarchingValue(Point point, Point circlePos, float radius)
		{
			double diffX = point.X - circlePos.X;
			double diffY = point.Y - circlePos.Y;
			return (radius * radius) / ((diffX * diffX) + (diffY * diffY));
		}
	}
}
