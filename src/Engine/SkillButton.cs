using Microsoft.Xna.Framework;

namespace ProjectBueno.Engine
{
	public class SkillButton
	{
		public SkillButton(Rectangle bounds, SkillHandler handler)
		{
			this.bounds = bounds;
			this.handler = handler;
		}

		public Rectangle bounds { get; protected set; }
		protected SkillHandler handler;

		public void Update()
		{
			
		}
		public void Draw()
		{
			if (bounds.Contains(handler.drawMouseState.X*handler.downscale, handler.drawMouseState.Y*handler.downscale))
			{
				Main.spriteBatch.Draw(Main.boxel,bounds,Color.White*0.5f);
			}
		}
	}
}
