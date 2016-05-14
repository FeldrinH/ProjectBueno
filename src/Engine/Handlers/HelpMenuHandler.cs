using Microsoft.Xna.Framework;
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
	class HelpMenuHandler : MenuHandler
	{
		public HelpMenuHandler(IHandler returnTo, string buttonName, GameHandler game = null)
		{
			background = Main.content.Load<Texture2D>((string)Main.Config[buttonName]["background"]);

			returnButton = new MoveButton(returnTo, (JObject)Main.Config[buttonName], true);
			this.game = game;
		}

		MoveButton returnButton;
		GameHandler game;

		public override void Draw()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, screenScale);
			Main.spriteBatch.Draw(background, Vector2.Zero, Color.White);
			returnButton.Draw(mouseX, mouseY);
			Main.spriteBatch.End();
		}

		public override void Update()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;
			if (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released)
			{
				returnButton.OnClick(mouseX, mouseY);
			}
			if (game != null && Main.newKeyState.IsKeyDown(Keys.P) && !Main.oldKeyState.IsKeyDown(Keys.P))
			{
				returnButton.moveTo.Deinitialize();
				Main.Handler = game;
			}
		}
	}
}
