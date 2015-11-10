using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;

namespace ProjectBueno.Game.Entities
{
	//Static Entity Texture
	public class StaticTexture
	{
		public StaticTexture(Texture2D texture, float xOffset, float yOffset, int w, int h)
		{
			this.texture = texture;

			this.xOffset = xOffset;
			this.yOffset = yOffset;
			this.w = w;
			this.h = h;
		}
		public readonly Texture2D texture;
		public readonly float xOffset;
		public readonly float yOffset;
		public readonly int w;
		public readonly int h;
	}

	//Animated Entity Texture with frames(horizontal) and states(vertical)
	public class AnimatedTexture : StaticTexture
	{
		public AnimatedTexture(Texture2D[] textures, float[] maxFrame, int length, int maxState, float xOffset, float yOffset, int w, int h)
			: base(new RenderTarget2D(Main.graphics.GraphicsDevice,w*length,h*maxState),xOffset,yOffset,w,h)
		{
			//Stitch textures together for memory conservation
			Main.graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D)texture);
			SpriteBatch batch = new SpriteBatch(Main.graphics.GraphicsDevice);
			float i = 0.0F;
			batch.Begin();
			foreach (Texture2D strip in textures)
			{
				batch.Draw(strip, new Vector2(0.0F, i), Color.White);
				i += h;
			}
			batch.End();

			this.maxFrame = maxFrame;
			this.maxState = maxState;
		}
		protected float curFrame; //Current frame (Float to allow incrementing every frame)
		protected readonly float[] maxFrame; //Maximum frame (Float to avoid casting)
		protected int curState;
		protected readonly int maxState;
		public Rectangle getCurFrame()
		{
			return new Rectangle();
		}
	}
}
