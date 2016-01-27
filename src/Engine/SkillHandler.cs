using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBueno.Engine
{
	public class SkillHandler : IHandler
	{
		public GameHandler game;

		protected Texture2D background;
		protected List<Skill> skills;
		public float downscale { get; protected set; }
		protected Matrix sreenScale;

		public SkillHandler(GameHandler game, List<Skill> skills)
		{
			this.game = game;
			background = Main.content.Load<Texture2D>("skillTree");
			this.skills = skills;
		}

		public void Draw()
		{
			Main.graphicsManager.GraphicsDevice.Clear(Color.Gray);
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, sreenScale);
			Main.spriteBatch.Draw(background, Vector2.Zero, Color.White);
			foreach (Skill skillButton in skills)
			{
				skillButton.DrawButton(downscale);
            }
			Main.spriteBatch.End();
		}

		public void Update()
		{
			if (Main.newKeyState.IsKeyDown(Keys.Back) && !Main.oldKeyState.IsKeyDown(Keys.Back))
			{
				Main.handler = game;
			}
		}
        
		public void windowResize()
		{
			sreenScale = Matrix.CreateScale((float)Main.window.ClientBounds.Width / Main.xRatio);
			downscale = (float)Main.xRatio / Main.window.ClientBounds.Width ;
		}
	}
}
