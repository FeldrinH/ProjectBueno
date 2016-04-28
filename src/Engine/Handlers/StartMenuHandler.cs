using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	class StartMenuHandler : MenuHandler
	{
		public StartMenuHandler()
		{
			background = Main.content.Load<Texture2D>("startMenu");

			music = Main.content.Load<SoundEffect>("startMusic").CreateInstance();
			music.IsLooped = true;

			buttons = new List<IButton>() { new TextMoveButton(new GameHandler(), (JObject)Main.Config["startBtn"]), new TextMoveButton(new HelpMenuHandler(this, "backBtnStart"), (JObject)Main.Config["helpBtn"],true), new QuitButton((JObject)Main.Config["quitBtn"]) };
		}

		protected SoundEffectInstance music;

		protected List<IButton> buttons;

		public override void Draw()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, screenScale);
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
