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

		/// <summary>
		/// Scales texture using a linear scaling method
		/// </summary>
		/// <param name="tex">Texture to be scaled</param>
		/// <param name="width">The desired width</param>
		/// <param name="height">The desired height</param>
		public static void ScalePoint(this Texture2D tex, int width, int height)
		{
			ThreadedScaling(tex, width, height, false);
		}

		/// <summary>
		/// Scales texture using a bilinear scaling method
		/// </summary>
		/// <param name="tex">Texture to be scaled</param>
		/// <param name="width">The desired width</param>
		/// <param name="height">The desired height</param>
		public static void ScaleBilinear(this Texture2D tex, int width, int height)
		{
			ThreadedScaling(tex, width, height, true);
		}

		/// <summary>
		/// Scales a given texuture to the given desired dimentions
		/// </summary>
		/// <param name="tex">Texture to be scaled</param>
		/// <param name="width">The desired width</param>
		/// <param name="height">The desired height</param>
		/// <param name="bilinear">Boolean dictating the method of scaling(linear or bilinear)</param>
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

				ThreadData td = new ThreadData(0, height);
				if (bilinear)
				{
					BilinearScale(td);
				}
				else
				{
					PointScale(td);
				}
			
			tex.Resize(width, height);
			tex.SetPixels(Col);
			tex.Apply();

			texCol = null;
			Col = null;
		}

		/// <summary>
		/// Scales a given set of texture data using a bilinear scaling method
		/// </summary>
		/// <param name="obj">Set of texture data to be scaled</param>
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
					Col[Row + x] = Color.LerpUnclamped(Color.LerpUnclamped(texCol[((y1 + xF) >= texCol.Length)?texCol.Length - 1:y1 + xF], texCol[((y1 + xF) >= texCol.Length) ?texCol.Length - 1 : y1 + xF + (((y1 + xF + 1) < texCol.Length) ? 1:0)], ratio), Color.LerpUnclamped(texCol[((y2 + xF) >= texCol.Length) ? texCol.Length - 1 : y2 + xF], texCol[((y2 + xF) >= texCol.Length) ? texCol.Length - 1 : y2 + xF + (((y2 + xF + 1) < texCol.Length) ? 1 : 0)], ratio), y * ratioY - yF);
				}
			}
		}

		/// <summary>
		/// Scales a given set of texture data using a Linear scaling method
		/// </summary>
		/// <param name="obj"></param>
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
			}
		}
	}
}
