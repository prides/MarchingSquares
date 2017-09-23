using MarchingSquares.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarchingSquares.Shapes
{
	public class BouncingCircle : Circle
	{
		private float mSpeed = 100.0f;
		public float Speed { get { return mSpeed; } set { mSpeed = value; } }

		private Vector mDirection;
		public Vector Direction { get { return mDirection; } set { mDirection = value; } }

		private Size mBorder;
		public Size Border { get { return mBorder; } set { mBorder = value; } }

		public BouncingCircle(Point pos, float radius, Size border) : base(pos, radius)
		{
			this.mBorder = border;
			this.mSpeed = (float)MS_Math.getRandomDouble(50.0, 100.0);

			Point targetPosition = MS_Math.getRandomPoint(border);

			this.mDirection = Point.Subtract(this.Position, targetPosition);
			this.mDirection.Normalize();
		}

		public void Update(float deltaTime)
		{
			this.Position += this.Direction * this.Speed * deltaTime;
			CheckCollision();
		}

		public void CheckCollision()
		{
			if (this.Position.X + this.Radius >= this.mBorder.Width)
			{
				this.mDirection.X *= -1.0f;
				this.mPosition.X = this.mBorder.Width - this.Radius;
			}
			if (this.Position.X - this.Radius <= 0)
			{
				this.mDirection.X *= -1.0f;
				this.mPosition.X = this.Radius;
			}
			if (this.Position.Y + this.Radius >= this.mBorder.Height)
			{
				this.mDirection.Y *= -1.0f;
				this.mPosition.Y = this.mBorder.Height - this.Radius;
			}
			if (this.Position.Y - this.Radius <= 0)
			{
				this.mDirection.Y *= -1.0f;
				this.mPosition.Y = this.Radius;
			}
		}
	}
}
