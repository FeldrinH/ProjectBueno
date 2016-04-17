using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Game.Tiles;
using ProjectBueno.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectBueno.Engine.World
{
	public struct TilePoint
	{
		public TilePoint(int x, int y, Tiles tile)
		{
			this.x = x;
			this.y = y;
			this.tile = tile;
		}

		public Tiles tile;
		public int x;
		public int y;
	}
	public enum Tiles : byte
	{
		Forest = 0,
		Desert = 16,
		Cold = 48,
		Sea = 80,
		ForestTree = 88,
		DesertTree = 96,
		ColdTree = 104,
		FilledForest = 255 //Only during generation
	}
	public class Terrain
	{
		public Terrain()
		{
			chunks = new Dictionary<Point, byte[][]>();
			terrainTex = Main.content.Load<Texture2D>("biomes");
		}

		//public static readonly List<Color> tileColors = new List<Color>() { Color.LightGreen, Color.LawnGreen, Color.Blue, Color.Yellow, Color.LightBlue };
		protected static readonly int[] xShift = { -1, 0, 1, -1, 1, -1, 0, 1 };
		protected static readonly int[] yShift = { -1, -1, -1, 0, 0, 1, 1, 1 };
		protected static readonly int[] xSide = { 0, 0, -1, 1 };
		protected static readonly int[] ySide = { -1, 1, 0, 0 };

		public static Texture2D terrainTex;

		public const int CHUNK_SIZE = 64;
		public const int CHUNK_BLEED = 8;
		public const float CHUNK_MULT = 1.0f / CHUNK_SIZE;
		public const float CHUNK_SHIFT = CHUNK_SIZE * Tile.TILESIZE * 0.5f;
		public const int BLOCK_SIZE = 16;
		public const int BLOCKS_PER_CHUNK = CHUNK_SIZE / BLOCK_SIZE;

		public static int xSize = 512;
		public static int ySize = 512;
		public static int xChunks = xSize / BLOCKS_PER_CHUNK;
		public static int yChunks = ySize / BLOCKS_PER_CHUNK;
		public static int tileLimit = 30000;
		protected Tiles[][] blockMap;
		protected Dictionary<Point, byte[][]> chunks;
		protected Queue<Point> callqueue = new Queue<Point>();
		public int tileCount;
		public int seaCount;

		public int terrainTop;
		public int terrainBottom;

		public Vector2 desertStart;
		public Vector2 desertEnd;
		public Vector2 coldStart;
		public Vector2 coldEnd;

		protected Random random = new Random();

		public bool isColliding(Vector2 pos, Vector2 size)
		{
			Point topLeft = getChunkFromPos(pos);
			Point bottomRight = getChunkFromPos(pos + size);
			for (int xC = topLeft.X; xC <= bottomRight.X; xC++)
			{
				for (int yC = topLeft.Y; yC <= bottomRight.Y; yC++)
				{
					Point curChunk = new Point(xC, yC);
					Point topLeftTile = getTileFromPos(curChunk, pos);
					Point bottomRightTile = getTileFromPos(curChunk, pos + size);
					byte[][] chunk = getChunk(curChunk);
					for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
					{
						for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
						{
							if (chunk[x][y] >= (byte)Tiles.Sea)
							{
								return true;
							}
						}
					}

				}
			}
			return false;
		}

		#region Chunk Generation
		public byte[][] getChunk(Point coords)
		{
			byte[][] returnChunk;
			if (chunks.TryGetValue(coords, out returnChunk))
			{
				return returnChunk;
			}
			else if (coords.X < 0 || coords.Y < 0 || coords.X >= xChunks || coords.Y >= yChunks)
			{
				return null;
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
			return new Point((int)pos.X, (int)pos.Y);
		}
		public static Point getTileFromPos(Point chunk, Vector2 pos)
		{
			pos = (pos * Tile.TILEMULT) - chunk.ToVector2() * CHUNK_SIZE;
			return new Point(MathHelper.Clamp((int)pos.X, 0, CHUNK_SIZE-1), MathHelper.Clamp((int)pos.Y, 0, CHUNK_SIZE-1));
		}

		protected byte[][] generateChunk(Point coords)
		{
			List<TilePoint> tileQueue = new List<TilePoint>();
			Tiles[][] chunk = Enumerable.Range(0, CHUNK_SIZE + CHUNK_BLEED * 2).Select(x => Enumerable.Range(0, CHUNK_SIZE + CHUNK_BLEED * 2).Select(y => getBlock(
				(x - CHUNK_BLEED + CHUNK_SIZE * BLOCK_SIZE) / BLOCK_SIZE - CHUNK_SIZE + coords.X * BLOCKS_PER_CHUNK,
				(y - CHUNK_BLEED + CHUNK_SIZE * BLOCK_SIZE) / BLOCK_SIZE - CHUNK_SIZE + coords.Y * BLOCKS_PER_CHUNK)
				).ToArray()).ToArray();

			for (int i = 0; i < 10; i++)
			{
				for (int xC = 0; xC < CHUNK_SIZE + CHUNK_BLEED * 2; xC++)
				{
					for (int yC = 0; yC < CHUNK_SIZE + CHUNK_BLEED * 2; yC++)
					{
						if (getPseudor(xC - CHUNK_BLEED + coords.X * CHUNK_SIZE, yC - CHUNK_BLEED + coords.Y * CHUNK_SIZE, (int)chunk[xC][yC] < (int)getAdjacentDifferent(chunk, xC, yC)))
						{
							tileQueue.Add(new TilePoint(xC, yC, getAdjacentDifferent(chunk, xC, yC)));
						}
					}
				}
				foreach (var upd in tileQueue)
				{
					chunk[upd.x][upd.y] = upd.tile;
				}
				tileQueue.Clear();
			}

			byte[][] returnChunk = new byte[CHUNK_SIZE][];
			for (int j = 0; j < CHUNK_SIZE; j++)
			{
				returnChunk[j] = new byte[CHUNK_SIZE];
			}

			int outTile;
			for (int x = CHUNK_BLEED; x < CHUNK_SIZE + CHUNK_BLEED; x++)
			{
				for (int y = CHUNK_BLEED; y < CHUNK_SIZE + CHUNK_BLEED; y++)
				{
					if (chunk[x][y] == Tiles.Sea)
					{
						outTile = (int)(Tiles.Sea) + random.Next(0, 7);
					}
					else
					{
						outTile = getAdjacentByte(chunk, x, y, Tiles.Sea);
						if (outTile == 0)
						{
							if (chunk[x][y] != Tiles.Forest)
							{
								outTile = getAdjacentByte(chunk, x, y, Tiles.Forest) + 16;
							}
						}
						outTile += (int)chunk[x][y];
					}

					returnChunk[x - CHUNK_BLEED][y - CHUNK_BLEED] = (byte)outTile;
				}
			}

			chunks.Add(coords, returnChunk);
			return returnChunk;
		}


		protected byte getAdjacentByte(Tiles[][] chunk, int x, int y, Tiles test)
		{
			byte outTile = 0;
			if (chunk[x + 1][y] == test)
			{
				outTile |= 1;
			}
			if (chunk[x - 1][y] == test)
			{
				outTile |= 2;
			}
			if (chunk[x][y - 1] == test)
			{
				outTile |= 4;
			}
			if (chunk[x][y + 1] == test)
			{
				outTile |= 8;
			}
			return outTile;
		}
		/*protected bool checkBlockBorders(int xB, int yB)
		{
			Tiles center = blockMap[xB][yB];
			return (center != blockMap[xB + 1][yB] || center != blockMap[xB][yB + 1] || center != blockMap[xB - 1][yB] || center != blockMap[xB][yB - 1] ||
				center != blockMap[xB + 1][yB + 1] || center != blockMap[xB - 1][yB - 1] || center != blockMap[xB + 1][yB - 1] || center != blockMap[xB - 1][yB + 1]);
		}*/
		protected Tiles getAdjacentDifferent(Tiles[][] chunk, int x, int y)
		{
			if (x < 1 || y < 1 || x > chunk.Length - 2 || y > chunk[0].Length - 2) //Broken workaround. FIXME
			{
				return chunk[x][y];
			}
			for (int i = 0; i < 4; i++)
			{
				if (chunk[x][y] != chunk[x + xSide[i]][y + ySide[i]])
				{
					return chunk[x + xSide[i]][y + ySide[i]];
				}
			}
			return chunk[x][y];
		}
		public bool getPseudor(int x, int y, bool flip) //Simplex noise to bool
		{
			return flip ? Noise.GetNoise(x * 0.125, y * 0.125, 0.0) > 0.5 : Noise.GetNoise(x * 0.125, y * 0.125, 0.0) < 0.5;
		}
		#endregion

		#region Draw
		public void drawChunk(Point coords)
		{
			byte[][] chunk = getChunk(coords);
			if (chunk == null)
			{
				return;
			}
			for (int x = 0; x < CHUNK_SIZE; x++)
			{
				for (int y = 0; y < CHUNK_SIZE; y++)
				{
					Main.spriteBatch.Draw(terrainTex, new Rectangle((x + coords.X * CHUNK_SIZE) * Tile.TILESIZE, (y + coords.Y * CHUNK_SIZE) * Tile.TILESIZE, Tile.TILESIZE, Tile.TILESIZE), new Rectangle(chunk[x][y] * Tile.TILESIZE, 0, Tile.TILESIZE, Tile.TILESIZE), Color.White);
				}
			}
		}
		public void drawChunkMap(Vector2 playerPos)
		{
			Main.spriteBatch.Draw(Main.boxel, new Rectangle(0, 0, xSize, ySize), Color.Black);
			for (int x = 1; x < xSize - 1; x++)
			{
				for (int y = 1; y < ySize - 1; y++)
				{
					Main.spriteBatch.Draw(terrainTex, new Rectangle(x, y, 1, 1), new Rectangle((int)blockMap[x][y] * Tile.TILESIZE, 0, 1, 1), Color.White);
				}
			}
			Main.spriteBatch.Draw(Main.boxel, playerPos * Tile.TILEMULT / BLOCK_SIZE, Color.Red);
		}
		#endregion

		#region Generate ChunkMap
		protected void setChunk(int x, int y, Tiles type)
		{
			if (emptyTile(x, y))
			{
				tileCount++;
			}
			blockMap[x][y] = type;
		}
		public Vector2 getRandomForestChunk()
		{
			int x, y;
			do
			{
				x = random.Next(0, xSize);
				y = random.Next(0, ySize);
			}
			while (blockMap[x][y] != Tiles.Forest && blockMap[x][y] != Tiles.FilledForest);
			return new Vector2(x, y);
		}
		protected bool emptyTile(int x, int y)
		{
			return blockMap[x][y] == Tiles.FilledForest;
		}
		protected void generateSea(int x, int y)
		{
			if (x > -1 && x < xSize && y > -1 && y < ySize)
			{
				if (blockMap[x][y] == Tiles.FilledForest)
				{
					seaCount++;
					blockMap[x][y] = Tiles.Sea;
					callqueue.Enqueue(new Point(x, y));
				}
				else if (blockMap[x][y] == Tiles.Forest)
				{
					seaCount++;
					tileCount--;
					blockMap[x][y] = Tiles.Sea;
				}
			}
		}
		protected void fillForest()
		{
			for (int x = 1; x < xSize - 1; x++)
			{
				for (int y = 1; y < ySize - 1; y++)
				{
					if (blockMap[x][y] == Tiles.FilledForest)
					{
						blockMap[x][y] = Tiles.Forest;
					}
				}
			}
		}
		protected void processSeaChunks(Point pos)
		{
			for (var i = 0; i < 4; i++)
			{
				generateSea(pos.X + xSide[i], pos.Y + ySide[i]);
			}
		}
		protected void generateBiomes(Vector2 startPoint, Vector2 endPoint, Tiles replaceTile, bool isAbove)
		{
			float mult = (startPoint.Y - endPoint.Y) / (startPoint.X - endPoint.X);
			float shift = startPoint.Y - mult * startPoint.X;
			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < xSize; y++)
				{
					if (blockMap[x][y] == Tiles.Forest || blockMap[x][y] == Tiles.FilledForest)
					{
						if ((x * mult + shift > y) != isAbove)
						{
							blockMap[x][y] = replaceTile;
						}
					}
				}
			}
		}

		public void generateChunkMap()
		{
			//Reset variables
			tileCount = 0;
			seaCount = 1;
			callqueue.Clear();
			blockMap = Enumerable.Range(0, xSize).Select(h => Enumerable.Range(0, ySize).Select(w => Tiles.FilledForest).ToArray()).ToArray();

			//Process land
			int x = xSize / 2;
			int y = ySize / 2;
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
			blockMap[0][0] = Tiles.Sea;
			callqueue.Enqueue(new Point(0, 0));
			while (callqueue.Count > 0)
			{
				processSeaChunks(callqueue.Dequeue());
			}

			fillForest();

			Console.WriteLine("Total land count: " + (xSize * ySize - seaCount));
			Console.WriteLine("Land count: " + tileCount);
			Console.WriteLine("Filled land count: " + (xSize * ySize - seaCount - tileCount));
		}

		public void processBiome()
		{
			for (int y = 1; y < ySize - 1; y++)
			{
				for (int x = 1; x < xSize - 1; x++)
				{
					if (blockMap[x][y] == Tiles.FilledForest || blockMap[x][y] == Tiles.Forest)
					{
						terrainTop = y;
						goto TerrainBottom;
					}
				}
			}
			TerrainBottom:
			for (int y = ySize - 1; y > 0; y--)
			{
				for (int x = 1; x < xSize - 1; x++)
				{
					if (blockMap[x][y] == Tiles.FilledForest || blockMap[x][y] == Tiles.Forest)
					{
						terrainBottom = y;
						goto ProcessBiome;
					}
				}
			}

			ProcessBiome:
			int terrainDelta = terrainBottom - terrainTop;
			desertStart = new Vector2(0.0f, (float)(terrainDelta * (1.0 / 3.0 + (random.NextDouble() - 0.5) * 0.3) + terrainTop));
			desertEnd = new Vector2(xSize, (float)(terrainDelta * (1.0 / 3.0 + (random.NextDouble() - 0.5) * 0.3) + terrainTop));
			coldStart = new Vector2(0.0f, (float)(terrainDelta * (2.0 / 3.0 + (random.NextDouble() - 0.5) * 0.3) + terrainTop));
			coldEnd = new Vector2(xSize, (float)(terrainDelta * (2.0 / 3.0 + (random.NextDouble() - 0.5) * 0.3) + terrainTop));

			generateBiomes(desertStart, desertEnd, Tiles.Desert, false);
			generateBiomes(coldStart, coldEnd, Tiles.Cold, true);
		}
		#endregion

		public Tiles getBlock(int x, int y)
		{
			if (x < 0 || y < 0 || x >= xSize || y >= ySize)
			{
				return Tiles.Sea;
			}
			return blockMap[x][y];
		}

		public void clearChunks()
		{
			chunks.Clear();
		}
	}
}
