using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Engine;

namespace ProjectBueno.Engine
{
	//Animated Entity Texture with frames(horizontal)
	public class AnimatedTexture
	{
		public AnimatedTexture(Texture2D texture, int frameCount, float speed, int w, int h, int xOffset=0, int yOffset=0)
		{
			this.texture = texture;
			this.xOffset = xOffset;
			this.yOffset = yOffset;
			this.w = w;
			this.h = h;
			this.curFrame = 0.0F;
			this.maxFrame = frameCount;
			this.speed = speed;
		}

		public readonly Texture2D texture;
		public readonly int xOffset;
		public readonly int yOffset;
		public readonly int w;
		public readonly int h;

		protected float curFrame; //Current frame (Float to allow incrementing every frame)
		protected readonly float maxFrame; //Maximum frame (Float to avoid casting)
		protected readonly float speed; //Speed frames/tick

		public Rectangle getCurFrame()
		{
			return new Rectangle(w * (int)curFrame + xOffset, yOffset, w, h);
		}
		public void incrementAnimation()
		{
			curFrame += speed;
			if (curFrame >= maxFrame)
			{
				curFrame -= maxFrame;
			}
		}
		public void resetFrame()
		{
			curFrame = 0.0F;
		}
	}
}
