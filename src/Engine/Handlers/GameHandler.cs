using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Game.Entities;
using ProjectBueno.Game.Tiles;
using System.Collections.Generic;
using System;
using ProjectBueno.Engine.World;

namespace ProjectBueno.Engine
{
	public class GameHandler : IHandler
    {
        public GameHandler()
        {
			Main.exiting += onExitSave;

			player = new Player(new Vector2(Terrain.xSize*0.5f*Tile.TILESIZE,Terrain.ySize*0.5f*Tile.TILESIZE), this);
			screenScale = 2.0f;
			projectiles = new List<Projectile>();
			entities = new List<Entity>();
			entities.Add(new Enemy(new Vector2((Terrain.xSize * 0.5f + 20.0f) * Tile.TILESIZE, (Terrain.ySize * 0.5f + 20.0f) * Tile.TILESIZE), this)); //Add enemy for testing

			terrain = new Terrain();

			terrain.generateChunkMap();
			terrain.startPoint = terrain.getRandomForestChunk();
			terrain.endPoint = terrain.getRandomForestChunk();
			terrain.processBiome();
		}

		private float _screenScale;
		public float screenScale
		{
			get { return _screenScale; }
			set {
				screenScaleInv = 1 / value;
				_screenScale = value;
			}
		}
		public float screenScaleInv;
		public Vector2 screenShift;
		public Matrix screenMatrix;

		public List<List<bool>> colMap;

		public List<Entity> entities;
		public List<Projectile> projectiles;
		public Player player { get; protected set; }
		public Terrain terrain { get; protected set; }

		public Vector2 posFromScreenPos(Vector2 screenPos)
		{
			return (screenPos - screenShift) * screenScaleInv + player.pos;
		}

		private void onExitSave(object sender, EventArgs args)
		{
			Console.WriteLine("NOW PRETENDING TO SAVE GAME");
		}

		public void closeGameHandler()
		{
			Main.exiting -= onExitSave;
		}

		public void windowResize()
		{
			screenShift = new Vector2((float)Math.Floor((Main.window.ClientBounds.Width - (player.size.X * screenScale)) * 0.5), (float)Math.Floor((Main.window.ClientBounds.Height - (player.size.Y * screenScale)) * 0.5));
			screenMatrix = Matrix.CreateScale(screenScale)*Matrix.CreateTranslation(new Vector3(screenShift, 0.0f));
		}

		public void Draw()
        {
			Main.graphicsManager.GraphicsDevice.Clear(Color.CornflowerBlue);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, Matrix.CreateTranslation(new Vector3(-player.pos, 0.0f))*screenMatrix);

			//int xRight = Math.Min((int)((player.pos.X + Main.window.ClientBounds.Width * screenScaleInv * 0.5f) * Tile.TILEMULT) + 2, Terrain.xSize);
			//int yBottom = Math.Min((int)((player.pos.Y + Main.window.ClientBounds.Height * screenScaleInv * 0.5f) * Tile.TILEMULT) + 2, Terrain.ySize);

			terrain.drawChunk(Terrain.getChunkFromPos(new Vector2(player.pos.X + 500.0f, player.pos.Y + 500.0f)));
			terrain.drawChunk(Terrain.getChunkFromPos(new Vector2(player.pos.X - 500.0f, player.pos.Y - 500.0f)));
			terrain.drawChunk(Terrain.getChunkFromPos(new Vector2(player.pos.X + 500.0f, player.pos.Y - 500.0f)));
			terrain.drawChunk(Terrain.getChunkFromPos(new Vector2(player.pos.X - 500.0f, player.pos.Y + 500.0f)));
			//Main.spriteBatch.DrawLine(TerrainGenerator.startPoint*Tile.TILESIZE,TerrainGenerator.endPoint*Tile.TILESIZE,Color.Black,1.0f);

			if (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released)
			{
				Console.WriteLine("Mouse:" + posFromScreenPos(Main.newMouseState.Position.ToVector2()) + " Player:" + player.pos);
			}

			player.Draw();
			for (int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Draw();
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
				terrain.generateChunkMap();
				terrain.startPoint = terrain.getRandomForestChunk();
				terrain.endPoint = terrain.getRandomForestChunk();
				terrain.processBiome();
			}
			for (int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Update();
			}
			foreach (var ent in entities)
			{
				ent.Update();
			}
			player.Update();
			projectiles.RemoveAll(item => item.toRemove);
			entities.RemoveAll(item => item.health == 0.0f);
			if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
			{
				Main.handler = new SkillHandler(this,player);
			}
			if (Main.newKeyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P))
			{
				Main.handler = new PauseHandler(this);
			}
		}
    }
}