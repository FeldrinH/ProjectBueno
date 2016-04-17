using Microsoft.Xna.Framework;
using ProjectBueno.Game.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine.World
{
	public class Chunk
	{
		public Chunk(byte[][] tiles, List<Tree> trees)
		{
			this.tiles = tiles;
			this.trees = trees;
		}

		public byte[][] tiles;
		public List<Tree> trees;
	}

	public enum TreeId
	{
		Forest,
		Desert,
		Cold
	}

	public struct Tree
	{
		public Tree(int x, int y, TreeId treeId)
		{
			this.pos = new Vector2(x, y) * Tile.TILESIZE;
			this.id = treeId;
		}

		static Tree()
		{
			treeOrigins = new Vector2[3] { new Vector2((float)Main.Config["treeForest"]["x"], (float)Main.Config["treeForest"]["y"]), new Vector2((float)Main.Config["treeDesert"]["x"], (float)Main.Config["treeDesert"]["y"]), new Vector2((float)Main.Config["treeCold"]["x"], (float)Main.Config["treeCold"]["y"]) };
		}

		public static readonly Vector2[] treeOrigins;

		public readonly Vector2 pos;
		public readonly TreeId id;
	}
}
