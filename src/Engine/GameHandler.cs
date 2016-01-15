using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Game.Entities;
using ProjectBueno.Game.Tiles;
using System.Collections.Generic;
using ProjectBueno.Game.Terrain;
using System;

namespace ProjectBueno.Engine
{
	public class GameHandler : IHandler
    {
        public GameHandler()
        {
			player = new Player(new Vector2(TerrainGenerator.xSize*0.5f*Tile.TILESIZE,TerrainGenerator.ySize*0.5f*Tile.TILESIZE), this);
			screenScale = 2.0f;
			projectiles = new List<Projectile>();
			entities = new List<Entity>();

			TerrainGenerator.startGenerate();
			TerrainGenerator.startPoint = TerrainGenerator.getRandomForest();
			TerrainGenerator.endPoint = TerrainGenerator.getRandomForest();
			TerrainGenerator.processBiome();
		}

		public float screenScale;
		public Matrix screenMatrix;

		public List<List<bool>> colMap;
		public Dictionary<string, Tile> tileRegistry;

		public List<Entity> entities;
		public List<Projectile> projectiles;
		public Player player { get; private set; }

		public void addTile(string id,Tile tile)
		{
			tileRegistry.Add(id,tile);
		}

		public void windowResize()
		{
			screenMatrix = Matrix.CreateScale(screenScale)*Matrix.CreateTranslation(new Vector3((float)Math.Floor((Main.window.ClientBounds.Width - (player.size.X * screenScale)) * 0.5), (float)Math.Floor((Main.window.ClientBounds.Height - (player.size.Y * screenScale)) * 0.5), 0.0f));
		}

		public void Draw()
        {
			Main.graphicsManager.GraphicsDevice.Clear(Color.CornflowerBlue);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, Matrix.CreateTranslation(new Vector3(-player.pos, 0.0f))*screenMatrix);

			float scaleInv = 1 / screenScale * 0.5f;

			int xRight = Math.Min((int)((player.pos.X + Main.window.ClientBounds.Width * scaleInv)*Tile.TILEMULT)+2,TerrainGenerator.xSize); 
			int yBottom = Math.Min((int)((player.pos.Y + Main.window.ClientBounds.Height * scaleInv) * Tile.TILEMULT)+2, TerrainGenerator.ySize);

			for (int x = Math.Max((int)((player.pos.X- Main.window.ClientBounds.Width * scaleInv) * Tile.TILEMULT),0); x < xRight; x++)
			{
				for (int y = Math.Max((int)((player.pos.Y - Main.window.ClientBounds.Height * scaleInv)*Tile.TILEMULT), 0); y < yBottom; y++)
				{
					Main.spriteBatch.Draw(Main.boxel, new Rectangle(x * Tile.TILESIZE, y * Tile.TILESIZE, Tile.TILESIZE, Tile.TILESIZE), TerrainGenerator.tileColors[(int)TerrainGenerator.terrain[x][y]]);
                }
			}
			//Main.spriteBatch.DrawLine(TerrainGenerator.startPoint*Tile.TILESIZE,TerrainGenerator.endPoint*Tile.TILESIZE,Color.Black,1.0f);
			player.Draw();
			foreach (var proj in projectiles)
			{
				proj.Draw();
			}
			foreach (var ent in entities)
			{
				ent.Draw();
			}

			Main.spriteBatch.End();
		}

        public void Update()
        {
			if (Main.newKeyState.IsKeyDown(Keys.Up) && !Main.oldKeyState.IsKeyDown(Keys.Up))
			{
				screenScale *= 2.0f;
				windowResize();
			}
			if (Main.newKeyState.IsKeyDown(Keys.Down) && !Main.oldKeyState.IsKeyDown(Keys.Down))
			{
				screenScale *= 0.5f;
				windowResize();
			}
			if (Main.newKeyState.IsKeyDown(Keys.Enter) && !Main.oldKeyState.IsKeyDown(Keys.Enter))
			{
				TerrainGenerator.startGenerate();
				TerrainGenerator.startPoint = TerrainGenerator.getRandomForest();
				TerrainGenerator.endPoint = TerrainGenerator.getRandomForest();
				TerrainGenerator.processBiome();
			}
			foreach( var proj in projectiles )
            {
				proj.Update();
			}
			foreach (var ent in entities)
			{
				ent.Update();
			}
			player.Update();
			projectiles.RemoveAll(item => item.health == 0);
			entities.RemoveAll(item => item.health == 0.0f);
			if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
			{
				Main.handler = new SkillHandler(this);
			}
			if (Main.newKeyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P))
			{
				Main.handler = new PauseHandler(this);
			}
		}
    }
}