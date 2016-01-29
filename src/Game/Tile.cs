﻿using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Game.Tiles
{
    public class Tile
    {
		public Texture2D texture;

		public const int TILESIZE = 16; //Size of one tile
		public const float TILEMULT = 1.0f / TILESIZE;

		public static void initTiles(GameHandler game)
		{
			game.addTile("box",new Tile());
		}
	}
}