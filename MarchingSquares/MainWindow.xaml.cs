using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using MarchingSquares.Shapes;
using MarchingSquares.MarchingSquare;
using MarchingSquares.Models;
using MarchingSquares.Utils;
using System.ComponentModel;

namespace MarchingSquares
{
	public partial class MainWindow : Window
	{
		private bool mIsAvailable = false;

		private bool mIsClosed = false;
		public bool IsClosed { get { return mIsClosed; } }

		private bool mIsPaused = false;

		private double mFps = 0;
		public double FPS { set { mFps = value; } get { return mFps; } }

		private bool mIsStarted = false;

		private List<Circle> mCircles = null;

		private List<List<MarchingSquareBase>> mMarchingSquares;

		private MarchingSquareType mType = MarchingSquareType.Interpolated;
		public MarchingSquareType Type
		{
			get { return mType; }
			set
			{
				mType = value;
				OnPropertyChanged("Type");
			}
		}

		private bool mDrawGrid = true;
		public bool DrawGrid
		{
			get { return mDrawGrid; }
			set
			{
				mDrawGrid = value;
				OnPropertyChanged("DrawGrid");
			}
		}

		private float mSquareSize = 25.0f;
		public float SquareSize
		{
			get { return mSquareSize; }
			set
			{
				mSquareSize = value;
				OnPropertyChanged("SquareSize");
			}
		}

		private bool mDrawCircle = true;
		public bool DrawCircle
		{
			get { return mDrawCircle; }
			set
			{
				mDrawCircle = value;
				OnPropertyChanged("DrawCircle");
			}
		}

		private bool mUseCustomCircles = false;
		public bool UseCustomCircles
		{
			get { return mUseCustomCircles; }
			set
			{
				mUseCustomCircles = value;
				OnPropertyChanged("UseCustomCircles");
			}
		}

		private int mCirclesCount = 11;
		public int CirclesCount
		{
			get { return mCirclesCount; }
			set
			{
				mCirclesCount = value;
				OnPropertyChanged("CirclesCount");
			}
		}

		private float mRadiusMin = 25.0f;
		public float RadiusMin
		{
			get { return mRadiusMin; }
			set
			{
				mRadiusMin = value;
				OnPropertyChanged("RadiusMin");
			}
		}

		private float mRadiusMax = 45.0f;
		public float RadiusMax
		{
			get { return mRadiusMax; }
			set
			{
				mRadiusMax = value;
				OnPropertyChanged("RadiusMax");
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			DataContext = this;

			this.Title = "Title";

			this.ExitMenuItem.Header = "Exit";
			this.PauseMenuItem.Header = "Pause";

			this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);

			this.Loaded += (s, e) =>
			{
				mIsAvailable = true;
			};

			this.Closing += (s, e) =>
			{
				mIsClosed = true;
				mIsAvailable = false;
			};
		}

		public void Start()
		{
			this.CreateMarchingSquares();

			this.CreateCircles();

			mIsStarted = true;
		}

		public void Stop()
		{
			this.RemoveMarchingSquares();

			this.RemoveCircles();
		}

		public void Restart()
		{
			this.Stop();
			this.Start();
		}

		public void Update(float deltaTime)
		{
			if (!mIsStarted)
			{
				this.Start();
			}

			if (!mIsAvailable) return;

			if (!mIsPaused)
			{
				if (null != mCircles)
				{
					foreach (BouncingCircle circle in mCircles)
					{
						circle.Update(deltaTime);
					}
				}

				if (null != mMarchingSquares)
				{
					foreach (List<MarchingSquareBase> msblist in mMarchingSquares)
					{
						foreach (MarchingSquareBase msb in msblist)
						{
							msb.Update(deltaTime, mCircles);
						}
					}
				}
			}
		}

		private void RemoveCircles()
		{
			if (null == mCircles)
			{
				return;
			}

			foreach(Circle circle in mCircles)
			{
				this.DrawCanvas.Children.Remove(circle.WinShape);
			}

			mCircles.Clear();
			mCircles = null;
		}

		private void CreateCircles()
		{
			if (null != mCircles)
			{
				return;
			}

			Random random = new Random();
			mCircles = new List<Circle>();
			BouncingCircle circle;
			Point position;
			float radius;
			Size border = this.DrawCanvas.RenderSize;

			int circlesCount = this.mUseCustomCircles ? this.mCirclesCount : CircledJapan.Coordinates.Length;

			for (int i = 0; i < circlesCount; i++)
			{
				if (this.mUseCustomCircles)
				{
					position = MS_Math.getRandomPoint(this.DrawCanvas.RenderSize);
					radius = (float)random.NextDouble() * mRadiusMax + mRadiusMin;
				}
				else
				{
					double x = CircledJapan.Coordinates[i][0];
					double y = CircledJapan.Coordinates[i][1];
					double r = CircledJapan.Coordinates[i][2];
					position = new Point(x, y);
					radius = (float)(r / 2.5);
				}

				circle = new BouncingCircle(position, radius, border);
				circle.ZOrder = 99;
				this.mCircles.Add(circle);
				if (mDrawCircle)
				{
					this.DrawCanvas.Children.Add(circle.WinShape);
				}
			}
		}

		private void RemoveMarchingSquares()
		{
			if (null == mMarchingSquares)
			{
				return;
			}

			foreach (List<MarchingSquareBase> row in this.mMarchingSquares)
			{
				foreach (MarchingSquareBase square in row)
				{
					List<ShapeBase> shapes = square.getWinShapes();
					foreach (ShapeBase shape in shapes)
					{
						this.DrawCanvas.Children.Remove(shape.WinShape);
					}
				}
			}

			this.mMarchingSquares = null;
		}

		private void CreateMarchingSquares()
		{
			if (null != mMarchingSquares)
			{
				return;
			}

			mMarchingSquares = new List<List<MarchingSquareBase>>();

			Size size = new Size(mSquareSize, mSquareSize);

			int xCount = (int)(this.DrawCanvas.RenderSize.Width / this.mSquareSize) + 1;
			int yCount = (int)(this.DrawCanvas.RenderSize.Height / this.mSquareSize) + 1;

			List<MarchingSquareBase> row;
			MarchingSquareBase square = null;

			MarchingSquareBase neighbourTop = null;
			MarchingSquareBase neighbourLeft = null;

			for (int rowIndex = 0; rowIndex < yCount; rowIndex++)
			{
				row = new List<MarchingSquareBase>();
				double squareY = mSquareSize * rowIndex + mSquareSize * 0.5;

				for (int colIndex = 0; colIndex < xCount; colIndex++)
				{
					double squareX = mSquareSize * colIndex + mSquareSize * 0.5;

					Point position = new Point(squareX, squareY);

					if (rowIndex > 0)
					{
						neighbourTop = mMarchingSquares[rowIndex - 1][colIndex];
					}

					switch(mType)
					{
						case MarchingSquareType.Simple:
							square = new SimpleMarchingSquare(position, size);
							break;
						case MarchingSquareType.Calculated:
							square = new CalculatedMarchingSquare(position, size, (CalculatedMarchingSquare)neighbourTop, (CalculatedMarchingSquare)neighbourLeft);
							break;
						case MarchingSquareType.Interpolated:
							square = new InterpolatedMarchingSquare(position, size, (InterpolatedMarchingSquare)neighbourTop, (InterpolatedMarchingSquare)neighbourLeft);
							break;
					}
					square.DrawGrid = mDrawGrid;

					row.Add(square);

					List<ShapeBase> shapes = square.getWinShapes();

					foreach (ShapeBase shape in shapes)
					{
						this.DrawCanvas.Children.Add(shape.WinShape);
					}

					neighbourLeft = square;
				}

				neighbourLeft = null;

				mMarchingSquares.Add(row);
			}
		}

		public void Draw()
		{
			if (!mIsAvailable) return;

			label_fps.Content = "FPS:" + mFps.ToString();
		}

		private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void MenuItem_Pause_Click(object sender, RoutedEventArgs e)
		{
			mIsPaused = !mIsPaused;
		}

		private void MainWindow_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.P:
					mIsPaused = !mIsPaused; break;

				case Key.Escape:
					this.Close(); break;
			}
		}

		private void OnSizeChanged()
		{
			this.RemoveMarchingSquares();
			this.CreateMarchingSquares();

			if (null != mCircles)
			{
				foreach (BouncingCircle circle in mCircles)
				{
					circle.Border = this.DrawCanvas.RenderSize;
				}
			}
		}

		private const int WmExitSizeMove = 0x232;

		private IntPtr HwndMessageHook(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case WmExitSizeMove:
					this.OnSizeChanged();
					// DO SOMETHING HERE
					handled = true;
					break;
			}
			return IntPtr.Zero;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var helper = new WindowInteropHelper(this);
			if (helper.Handle != null)
			{
				var source = HwndSource.FromHwnd(helper.Handle);
				if (source != null)
					source.AddHook(HwndMessageHook);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(String name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		private void OnApplyButtonClick(object sender, RoutedEventArgs e)
		{
			Restart();
		}
	}
}
