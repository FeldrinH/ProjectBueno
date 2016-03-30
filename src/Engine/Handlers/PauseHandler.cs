using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace ProjectBueno.Engine
{
	public class PauseHandler : IHandler
	{
		protected GameHandler game;
		protected Texture2D background;
		protected SoundEffectInstance music;

		public PauseHandler(GameHandler game)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("pauseScreen");
			music = Main.content.Load<SoundEffect>("pauseMusic").CreateInstance();
			music.IsLooped = true;
			music.Play();

			windowResize();
		}

		public void Draw()
		{
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
			Main.spriteBatch.Draw(background, new Rectangle(0, 0, Main.graphicsManager.GraphicsDevice.Viewport.Width, Main.graphicsManager.GraphicsDevice.Viewport.Height), Color.White);
			Main.spriteBatch.End();
		}

		public void Update()
		{
			if (Main.newKeyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P))
			{
				music.Stop();
				music.Dispose();
				game.windowResize();
				Main.handler = game;
			}
		}

		public void windowResize()
		{
		}
	}
}
