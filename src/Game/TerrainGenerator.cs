using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Game.Terrain
{
	struct Coord
	{
		public Coord(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public int x;
		public int y;
	}
	enum Tiles : byte
	{
		FilledForest,
		Forest,
		FloodedSea,
        Sea,
		ColdForest,
		HotForest
	}
	static class TerrainGenerator
	{
		public static readonly List<Color> tileColors = new List<Color>() { Color.LawnGreen, Color.LawnGreen, Color.DarkBlue, Color.DarkBlue, Color.LightBlue, Color.Yellow };
		private static readonly int[] xShift = { -1, 0, 1, -1, 1, -1, 0, 1 };
		private static readonly int[] yShift = { -1, -1, -1, 0, 0, 1, 1, 1 };
		private static readonly int[] xSide = { 0, 0, -1, 1 };
		private static readonly int[] ySide = { -1, 1, 0, 0 };
		public static int xSize = 500;
		public static int ySize = 500;
		public static List<List<Tiles>> terrain;
		private static List<Coord> callqueue = new List<Coord>();
		public static int tileCount;
		public static int seaCount;
		public static Vector2 startPoint;
		public static Vector2 endPoint;

		private static Random random = new Random();

		private static void setTile(int x, int y, Tiles type)
		{
			if (emptyTile(x, y))
			{
				tileCount++;
			}
			terrain[x][y] = type;
		}
		public static Vector2 getRandomForest()
		{
			int x, y;
			do
			{
				x = random.Next(0, xSize);
				y = random.Next(0, ySize);
			}
			while (terrain[x][y] != Tiles.Forest && terrain[x][y] != Tiles.FilledForest);
            return new Vector2(x, y);
		}
		private static bool emptyTile(int x, int y)
		{
			return terrain[x][y] == Tiles.FilledForest;
		}
		private static void generateSea(int x, int y)
		{
			if (x > -1 && x < xSize && y > -1 && y < ySize)
			{
				if (terrain[x][y] == Tiles.FilledForest)
				{
					seaCount++;
					terrain[x][y] = Tiles.Sea;
					callqueue.Add(new Coord(x, y));
				}
				else if (terrain[x][y] == Tiles.Forest)
                {
					seaCount++;
					tileCount--;
					terrain[x][y] = Tiles.FloodedSea;
				}
			}
		}
		public static void startGenerate()
		{
			//Reset variables
			tileCount = 0;
			seaCount = 1;
			callqueue.Clear();
			terrain = Enumerable.Range(0, xSize).Select(h => Enumerable.Range(0, ySize).Select(w => Tiles.FilledForest).ToList()).ToList();

			//Process land
			int x = xSize/2;
			int y = ySize/2;
			while (tileCount < 30000)
			{
				Tiles type = Tiles.Forest;
				if (x < 1 || x > xSize - 2 || y < 1 || y > ySize - 2)
				{
					break;
				}
				setTile(x, y, type);
				int remove = random.Next(0, 8);
				x += xShift[remove];
				y += yShift[remove];
			}

			//Process sea
			terrain[0][0] = Tiles.Sea;
			callqueue.Add(new Coord(0, 0));
			while (callqueue.Count > 0)
			{
				processSea(callqueue[0].x,callqueue[0].y);
				callqueue.RemoveAt(0);
			}
			Console.WriteLine("Total land count: " + (xSize * ySize - seaCount));
			Console.WriteLine("Land count: " + tileCount);
			Console.WriteLine("Filled land count: " + (xSize * ySize - seaCount - tileCount));
		}
		public static void processBiome()
		{
			bool desertLeft = random.NextDouble() < 0.5;
            float mult = (startPoint.Y - endPoint.Y) / (startPoint.X - endPoint.X);
			if (float.IsInfinity(mult))
			{
				for (int x = 0; x < xSize; x++)
				{
					for (int y = 0; y < xSize; y++)
					{
						if (terrain[x][y] == Tiles.Forest || terrain[x][y] == Tiles.FilledForest)
						{
							if ((x > startPoint.X) != desertLeft)
							{
								terrain[x][y] = Tiles.ColdForest;
							}
							else
							{
								terrain[x][y] = Tiles.HotForest;
							}
						}
					}
				}
			}
			float shift = startPoint.Y - mult * startPoint.X;
			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < xSize; y++)
				{
					if (terrain[x][y] == Tiles.Forest || terrain[x][y] == Tiles.FilledForest)
					{
						if ((x * mult + shift > y) != desertLeft)
						{
							terrain[x][y] = Tiles.ColdForest;
						}
						else
						{
							terrain[x][y] = Tiles.HotForest;
						}
					}
				}
			}
		}

		private static void processSea(int x, int y)
		{
			for (var i = 0; i < xSide.Length; i++)
			{
				generateSea(x + xSide[i], y + ySide[i]);
			}
		}
	}
}
