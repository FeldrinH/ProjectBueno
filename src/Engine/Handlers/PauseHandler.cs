using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ProjectBueno.Engine
{
	public class PauseHandler : MenuHandler
	{
		protected GameHandler game;
		protected Texture2D background;
		protected SoundEffectInstance music;

		protected List<IButton> buttons;

		public PauseHandler(GameHandler game)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("pauseScreen");
			music = Main.content.Load<SoundEffect>("pauseMusic").CreateInstance();
			music.IsLooped = true;

			buttons = new List<IButton>() { new TextMoveButton(new HelpMenuHandler(this, "backBtnPause", game), (JObject)Main.Config["helpBtn2"], true), new TextMoveButton(game, (JObject)Main.Config["resumeBtn"]) };
		}

		public override void Draw()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, screenScale);
			Main.spriteBatch.Draw(background, Vector2.Zero, Color.White);
			foreach (var btn in buttons)
			{
				btn.Draw(mouseX, mouseY);
			}
			Main.spriteBatch.End();
		}

		public override void Update()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;

			if (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released)
			{
				foreach (var btn in buttons)
				{
					btn.OnClick(mouseX, mouseY);
				}
			}

			if (Main.newKeyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P))
			{
				Main.Handler = game;
			}
		}

		public override void Initialize()
		{
			music.Play();
		}

		public override void Deinitialize()
		{
			music.Stop();
		}
	}
}
