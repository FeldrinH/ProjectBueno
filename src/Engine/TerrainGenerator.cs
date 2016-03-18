using Microsoft.Xna.Framework;
using ProjectBueno.Game.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectBueno.Engine.World
{
	public enum Tiles : byte
	{
		FilledForest,
		Forest,
		FloodedSea,
        Sea,
		ColdForest,
		HotForest
	}
	public class Terrain
	{
		public Terrain()
		{
			chunks = new Dictionary<Point, List<List<Tiles>>>();
		}

		public static readonly List<Color> tileColors = new List<Color>() { Color.LawnGreen, Color.LawnGreen, Color.DarkBlue, Color.DarkBlue, Color.LightBlue, Color.Yellow };
		protected static readonly int[] xShift = { -1, 0, 1, -1, 1, -1, 0, 1 };
		protected static readonly int[] yShift = { -1, -1, -1, 0, 0, 1, 1, 1 };
		protected static readonly int[] xSide = { 0, 0, -1, 1 };
		protected static readonly int[] ySide = { -1, 1, 0, 0 };

		public const int CHUNK_SIZE = 64;
		public const float CHUNK_MULT = 1.0f / CHUNK_SIZE;
		public const float CHUNK_SHIFT = CHUNK_SIZE * Tile.TILESIZE * 0.5f;
		public const int BLOCK_SIZE = 16;
		public const int BLOCKS_PER_CHUNK = CHUNK_SIZE / BLOCK_SIZE;

		public static int xSize = 512;
		public static int ySize = 512;
		public static int tileLimit = 30000;
		protected List<List<Tiles>> chunkMap;
		protected Dictionary<Point, List<List<Tiles>>> chunks;
		protected List<Point> callqueue = new List<Point>();
		public int tileCount;
		public int seaCount;
		public Vector2 startPoint;
		public Vector2 endPoint;

		protected Random random = new Random();

		public List<List<Tiles>> getChunk(Point coords)
		{
			List<List<Tiles>> returnChunk;
			if (chunks.TryGetValue(coords, out returnChunk))
			{
				return returnChunk;
			}
			else
			{
				returnChunk = generateChunk(coords);
				return returnChunk;
			}
		}
		public static Point getChunkFromPos(Vector2 pos)
		{
			pos *= CHUNK_MULT * Tile.TILEMULT;
			return new Point((int)pos.X,(int)pos.Y);
		}

		protected List<List<Tiles>> generateChunk(Point coords)
		{
			List<List<Tiles>> chunk = Enumerable.Range(0, CHUNK_SIZE).Select(x => Enumerable.Range(0, CHUNK_SIZE).Select(y => chunkMap[x / BLOCK_SIZE + coords.X * BLOCKS_PER_CHUNK][y / BLOCK_SIZE + coords.Y * BLOCKS_PER_CHUNK]).ToList()).ToList();
			for(int x = 0; x <= CHUNK_SIZE; x++)
			{
				for (int y = 0; y <= CHUNK_SIZE; y++)
				{
				}
			}
			chunks.Add(coords, chunk);
			return chunk;
		}

		public void drawChunk(Point coords)
		{
			List<List<Tiles>> chunk = getChunk(coords);
            for (int x = 0; x < CHUNK_SIZE; x++)
			{
				for (int y = 0; y < CHUNK_SIZE; y++)
				{
					Main.spriteBatch.Draw(Main.boxel, new Rectangle((x + coords.X * CHUNK_SIZE) * Tile.TILESIZE, (y + coords.Y * CHUNK_SIZE) * Tile.TILESIZE, Tile.TILESIZE, Tile.TILESIZE), tileColors[(int)chunk[x][y]]);
				}
			}
		}

		protected void setChunk(int x, int y, Tiles type)
		{
			if (emptyTile(x, y))
			{
				tileCount++;
			}
			chunkMap[x][y] = type;
		}
		public Vector2 getRandomForestChunk()
		{
			int x, y;
			do
			{
				x = random.Next(0, xSize);
				y = random.Next(0, ySize);
			}
			while (chunkMap[x][y] != Tiles.Forest && chunkMap[x][y] != Tiles.FilledForest);
            return new Vector2(x, y);
		}
		protected bool emptyTile(int x, int y)
		{
			return chunkMap[x][y] == Tiles.FilledForest;
		}
		protected void generateSea(int x, int y)
		{
			if (x > -1 && x < xSize && y > -1 && y < ySize)
			{
				if (chunkMap[x][y] == Tiles.FilledForest)
				{
					seaCount++;
					chunkMap[x][y] = Tiles.Sea;
					callqueue.Add(new Point(x, y));
				}
				else if (chunkMap[x][y] == Tiles.Forest)
                {
					seaCount++;
					tileCount--;
					chunkMap[x][y] = Tiles.FloodedSea;
				}
			}
		}
		public void generateChunkMap()
		{
			//Reset variables
			tileCount = 0;
			seaCount = 1;
			callqueue.Clear();
			chunkMap = Enumerable.Range(0, xSize).Select(h => Enumerable.Range(0, ySize).Select(w => Tiles.FilledForest).ToList()).ToList();

			//Process land
			int x = xSize/2;
			int y = ySize/2;
			while (tileCount < tileLimit)
			{
				Tiles type = Tiles.Forest;
				if (x < 1 || x > xSize - 2 || y < 1 || y > ySize - 2)
				{
					break;
				}
				setChunk(x, y, type);
				int remove = random.Next(0, 8);
				x += xShift[remove];
				y += yShift[remove];
			}

			//Process sea
			chunkMap[0][0] = Tiles.Sea;
			callqueue.Add(new Point(0, 0));
			while (callqueue.Count > 0)
			{
				processSeaChunks(callqueue[0].X,callqueue[0].Y);
				callqueue.RemoveAt(0);
			}
			Console.WriteLine("Total land count: " + (xSize * ySize - seaCount));
			Console.WriteLine("Land count: " + tileCount);
			Console.WriteLine("Filled land count: " + (xSize * ySize - seaCount - tileCount));
		}
		public void processBiome()
		{
			bool desertLeft = random.NextDouble() < 0.5;
			float mult = (startPoint.Y - endPoint.Y) / (startPoint.X - endPoint.X);
			if (float.IsInfinity(mult))
			{
				for (int x = 0; x < xSize; x++)
				{
					for (int y = 0; y < xSize; y++)
					{
						if (chunkMap[x][y] == Tiles.Forest || chunkMap[x][y] == Tiles.FilledForest)
						{
							if ((x > startPoint.X) != desertLeft)
							{
								chunkMap[x][y] = Tiles.ColdForest;
							}
							else
							{
								chunkMap[x][y] = Tiles.HotForest;
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
					if (chunkMap[x][y] == Tiles.Forest || chunkMap[x][y] == Tiles.FilledForest)
					{
						if ((x * mult + shift > y) != desertLeft)
						{
							chunkMap[x][y] = Tiles.ColdForest;
						}
						else
						{
							chunkMap[x][y] = Tiles.HotForest;
						}
					}
				}
			}
		}
		protected void processSeaChunks(int x, int y)
		{
			for (var i = 0; i < xSide.Length; i++)
			{
				generateSea(x + xSide[i], y + ySide[i]);
			}
		}

		public void clearChunks()
		{
			chunks.Clear();
		}
	}
}
