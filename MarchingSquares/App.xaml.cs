using System;
using System.Windows;
using System.Windows.Threading;

namespace MarchingSquares
{
	public partial class App : Application
	{
		private long mNextTick;
		private long mLastCountTick;
		private long mLastFpsTick;

		private long mCurrentTick;
		private int mFrameCount = 0;
		private double mFrameRate;
		private const double mIdealFrameRate = 30;

		private static MainWindow mWin;

		private static System.Threading.Mutex mMutex;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			mMutex = new System.Threading.Mutex(false, Application.ResourceAssembly.FullName);
			if (!mMutex.WaitOne(0, false))
			{
				mMutex.Close();
				mMutex = null;
				this.Shutdown();
			}

			Start();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (mMutex != null)
			{
				mMutex.ReleaseMutex();
				mMutex.Close();
			}
		}

		private void Start()
		{
			mWin = new MainWindow();
			mWin.Show();

			while (!mWin.IsClosed)
			{
				mCurrentTick = Environment.TickCount;
				double diffms = Math.Floor(1000.0 / mIdealFrameRate);
				if (mNextTick == 0)
				{
					mNextTick = mCurrentTick + (long)diffms;
				}

				if (mCurrentTick < mNextTick)
				{
					
				}
				else
				{
					if (mLastCountTick != 0)
					{
						mWin.Update((mCurrentTick - mLastCountTick) / 1000.0f);
					}

					if (Environment.TickCount >= mNextTick + diffms)
					{

					}
					else
					{
						mWin.Draw();
					}

					mFrameCount++;
					mLastCountTick = mCurrentTick;
					while (mCurrentTick >= mNextTick)
					{
						mNextTick += (long)diffms;
					}
				}

				if (mCurrentTick - mLastFpsTick >= 1000)
				{
					mFrameRate = mFrameCount * 1000 / (double)(mCurrentTick - mLastFpsTick);
					mWin.FPS = mFrameRate;
					mFrameCount = 0;
					mLastFpsTick = mCurrentTick;
				}

				DoEvents();
			}
		}

		private void DoEvents()
		{
			DispatcherFrame frame = new DispatcherFrame();

			DispatcherOperationCallback exitFrameCallback = (f) =>
			{
				((DispatcherFrame)f).Continue = false;
				return null;
			};

			DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
				DispatcherPriority.Background, exitFrameCallback, frame);

			Dispatcher.PushFrame(frame);

			if (exitOperation.Status != DispatcherOperationStatus.Completed)
			{
				exitOperation.Abort();
			}
		}
	}
}
