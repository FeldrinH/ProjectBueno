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
			screenScaleF = 2.0f;
			screenScale = Matrix.CreateScale(screenScaleF);
			screenShift = Matrix.CreateTranslation(new Vector3(Main.window.ClientBounds.Width * 0.5f, Main.window.ClientBounds.Height * 0.5f, 0.0f));
			projectiles = new List<Projectile>();
			entities = new List<Entity>();

			TerrainGenerator.startGenerate();
			TerrainGenerator.startPoint = TerrainGenerator.getRandomForest();
			TerrainGenerator.endPoint = TerrainGenerator.getRandomForest();
			TerrainGenerator.processBiome();
		}

		static GameHandler()
		{
			boxel = new Texture2D(Main.spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			boxel.SetData(new[] { Color.White });
		}

		public float screenScaleF;
		public Matrix screenScale;
		public Matrix screenShift;

		public List<List<bool>> colMap;
		public Dictionary<string, Tile> tileRegistry;

		public List<Entity> entities;
		public List<Projectile> projectiles;
		public Player player { get; private set; }

		public static readonly Texture2D boxel;

		public void addTile(string id,Tile tile)
		{
			tileRegistry.Add(id,tile);
		}

		public void windowResize()
		{
			screenShift = Matrix.CreateTranslation(new Vector3((float)Math.Floor((Main.window.ClientBounds.Width - (player.size.X * screenScaleF)) * 0.5), (float)Math.Floor((Main.window.ClientBounds.Height - (player.size.Y * screenScaleF)) * 0.5), 0.0f));
		}

		public void Draw()
        {
			Main.graphicsManager.GraphicsDevice.Clear(Color.CornflowerBlue);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, Matrix.CreateTranslation(new Vector3(-player.pos, 0.0f))*screenScale*screenShift);

			for (int x = 0; x < TerrainGenerator.xSize; x++)
			{
				for (int y = 0; y < TerrainGenerator.xSize; y++)
				{
					Main.spriteBatch.Draw(boxel, new Rectangle(x * Tile.TILESIZE, y * Tile.TILESIZE, Tile.TILESIZE, Tile.TILESIZE), TerrainGenerator.tileColors[(int)TerrainGenerator.terrain[x][y]]);
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
				screenScaleF *= 2.0f;
				screenScale = Matrix.CreateScale(screenScaleF);
				windowResize();
			}
			if (Main.newKeyState.IsKeyDown(Keys.Down) && !Main.oldKeyState.IsKeyDown(Keys.Down))
			{
				screenScaleF *= 0.5f;
				screenScale = Matrix.CreateScale(screenScaleF);
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