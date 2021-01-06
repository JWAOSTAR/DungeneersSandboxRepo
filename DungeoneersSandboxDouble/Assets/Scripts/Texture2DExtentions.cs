using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RedExtentions
{
	public static class Texture2DExtentions
	{
		public class ThreadData
		{
			public int startTime;
			public int endTime;
			public ThreadData(int start, int end)
			{
				startTime = start;
				endTime = end;
			}
		}

		static Color[] texCol;
		static Color[] Col;
		static int texWidth;
		static int _width;
		static float ratioX;
		static float ratioY;
		static Mutex mutex;
		static int finishCount;

		public static void ScalePoint(this Texture2D tex, int width, int height)
		{
			ThreadedScaling(tex, width, height, false);
		}

		public static void ScaleBilinear(this Texture2D tex, int width, int height)
		{
			ThreadedScaling(tex, width, height, true);
		}

		static void ThreadedScaling (Texture2D tex, int width, int height, bool bilinear)
		{
			texCol = tex.GetPixels();
			Col = new Color[width * height];
			if(bilinear)
			{
				ratioX = 1.0f / ((float)width / (tex.width - 1));
				ratioY = 1.0f / ((float)height / (tex.height - 1));
			}
			else
			{
				ratioX = ((float)tex.width / width);
				ratioY = ((float)tex.height / height);
			}

			texWidth = tex.width;
			_width = width;
			float cores = Mathf.Min(SystemInfo.processorCount, height);
			float slice = height / cores;

			finishCount = 0;
			if(mutex == null)
			{
				mutex = new Mutex(false);
			}
			if(/*cores > 1*/ false)
			{
				int i = 0;
				ThreadData td;
				for(i = 0;  i < cores-1; i++)
				{
					td = new ThreadData((int)(slice * i), (int)(slice * (i + 1)));
					ParameterizedThreadStart pts = (bilinear) ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
					Thread thread = new Thread(pts);
					thread.Start(td);
				}
				td = new ThreadData((int)(slice * i), height);
				if(bilinear)
				{
					BilinearScale(td);
				}
				else
				{
					PointScale(td);
				}
				while(finishCount <= cores)
				{
					Thread.Sleep(1);
				}
			}
			else
			{
				ThreadData td = new ThreadData(0, height);
				if (bilinear)
				{
					BilinearScale(td);
				}
				else
				{
					PointScale(td);
				}
			}
			tex.Resize(width, height);
			tex.SetPixels(Col);
			tex.Apply();

			texCol = null;
			Col = null;
		}

		static void BilinearScale(System.Object obj)
		{
			ThreadData td = (ThreadData)obj;
			for(int y = td.startTime; y < td.endTime; y++)
			{
				int yF = (int)Mathf.Floor(y * ratioX);
				int y1 = yF * texWidth;
				int y2 = (yF + 1) * texWidth;
				int Row = y * _width;

				for(int x = 0; x < _width; x++)
				{
					int xF = (int)Mathf.Floor(x * ratioX);
					float ratio = x * ratioX - xF;
					if ((Row + x) > Col.Length || (y1 + xF) >= texCol.Length || (y2 + xF) >= texCol.Length || (y1 + xF) < 0 || (y1 + xF + 1) < 0 || (y2 + xF) < 0 || (y2 + xF + 1) < 0)
					{
						int test = 0;
						continue;
					}
					Col[Row + x] = Color.LerpUnclamped(Color.LerpUnclamped(texCol[y1 + xF], texCol[y1 + xF + (((y1 + xF + 1) < texCol.Length) ? 1:0)], ratio), Color.LerpUnclamped(texCol[y2 + xF], texCol[y2 + xF + (((y2 + xF + 1) < texCol.Length) ? 1 : 0)], ratio), y * ratioY - yF);
				}

				mutex.WaitOne();
				finishCount++;
				mutex.ReleaseMutex();
			}
		}

		static void PointScale(System.Object obj)
		{
			ThreadData td = (ThreadData)obj;
			for (int y = td.startTime; y < td.endTime; y++)
			{
				int texRow = (int)(ratioY * y) * texWidth;
				int Row = y * _width;
				for(int x = 0; x < _width; x++)
				{
					Col[Row + x] = texCol[(int)(texRow + ratioX * x)];
				}
				mutex.WaitOne();
				finishCount++;
				mutex.ReleaseMutex();
			}
		}
	}
}
