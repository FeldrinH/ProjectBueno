using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectBueno.Game.Entities;
using ProjectBueno.Game.Spells;
using System;

namespace ProjectBueno.Engine
{
	public class SkillHandler : IHandler
	{
		public GameHandler game;

		protected Texture2D background;
		protected Player player;
		public float downscale { get; protected set; }
		protected Matrix sreenScale;

		public static readonly Vector2 namePos = new Vector2(72,41);
		public static readonly Vector2 descPos = new Vector2(72,51);

		protected Skill curHeld;

		public SkillHandler(GameHandler game, Player player)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("skillTree");
			this.player = player;
		}

		public void Draw()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;
			Skill drawHeldText = curHeld;
			Main.graphicsManager.GraphicsDevice.Clear(Color.Gray);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, sreenScale);
			Main.spriteBatch.Draw(background, Vector2.Zero, Color.White);
			foreach (Skill skillButton in player.skills)
			{
				if (skillButton.DrawButton(mouseX, mouseY))
				{
					drawHeldText = skillButton;
				}
			}
			player.spells[player.selectedSpell].DrawButtons(mouseX,mouseY);
			if (drawHeldText != null)
			{
				Main.spriteBatch.DrawString(Main.retroFont, drawHeldText.name, namePos, Color.Purple);
				Main.spriteBatch.DrawString(Main.retroFont, drawHeldText.description, descPos, Color.Black);
			}
			if (curHeld != null)
			{
				curHeld.Draw(new Vector2(mouseX, mouseY));
			}

			Main.spriteBatch.End();
		}

		public void Update()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;
			if (Main.newMouseState.LeftButton == ButtonState.Pressed && Main.oldMouseState.LeftButton == ButtonState.Released)
			{
				foreach (Skill skill in player.skills)
				{
					skill.onClick(mouseX, mouseY, ref player.knowledgePoints, ref curHeld);
				}
				player.spells[player.selectedSpell].onPlaceClick(mouseX, mouseY, ref curHeld);
			}
			if (Main.newMouseState.RightButton == ButtonState.Pressed && Main.oldMouseState.RightButton == ButtonState.Released)
			{
				if (!player.spells[player.selectedSpell].onClearClick(mouseX, mouseY))
				{
					curHeld = null;
				}
			}
			if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
			{
				Main.handler = game;
			}
		}
		
		public void windowResize()
		{
			sreenScale = Matrix.CreateScale((float)Main.window.ClientBounds.Width / Main.xRatio);
			downscale = (float)Main.xRatio / Main.window.ClientBounds.Width;
		}
	}
}
