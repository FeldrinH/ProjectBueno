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

	public struct Tree
	{
		public Tree(int x, int y, int treeId)
		{
			this.pos = new Vector2(x, y) * Tile.TILESIZE;
			this.id = treeId;
		}

		static Tree()
		{
			treeOrigin = new Vector2((float)Main.Config["tree"]["x"], (float)Main.Config["tree"]["y"]);
			treeRect = new Rectangle(0, 0, (int)Main.Config["tree"]["w"], (int)Main.Config["tree"]["h"]);
		}

		public static readonly Vector2 treeOrigin;
		public static readonly Rectangle treeRect;

		public readonly Vector2 pos;
		public readonly int id;
	}
}
