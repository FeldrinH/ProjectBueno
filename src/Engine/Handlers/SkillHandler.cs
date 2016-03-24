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
		protected Matrix screenScale;

		public static readonly Vector2 namePos;
		public static readonly Vector2 descPos;
		public static readonly Vector2 costPos;

		protected Skill curHeld;

		public SkillHandler(GameHandler game, Player player)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("skillTree");
			this.player = player;
		}

		static SkillHandler()
		{
			namePos = new Vector2((int)Main.Config["namePos"]["x"], (int)Main.Config["namePos"]["y"]);
			descPos = new Vector2((int)Main.Config["descPos"]["x"], (int)Main.Config["descPos"]["y"]);
			costPos = new Vector2((int)Main.Config["costPos"]["x"], (int)Main.Config["costPos"]["y"]);
		}

		public void Draw()
		{
			float mouseX = Main.newMouseState.X * downscale;
			float mouseY = Main.newMouseState.Y * downscale;
			Skill drawHeldText = curHeld;
			Main.graphicsManager.GraphicsDevice.Clear(Color.Gray);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, screenScale);
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
				Main.spriteBatch.DrawString(Main.retroFont, drawHeldText.cost.ToString() + " KP", costPos, Color.Green);
			}
			if (curHeld != null)
			{
				curHeld.Draw(new Vector2(mouseX-Skill.buttonSize*0.5f, mouseY-Skill.buttonSize*0.5f));
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
			screenScale = Matrix.CreateScale((float)Main.window.ClientBounds.Width / Main.xRatio);
			downscale = (float)Main.xRatio / Main.window.ClientBounds.Width;
		}
	}
}
