using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;

namespace ProjectBueno.Engine
{
	//Animated Entity Texture with frames(horizontal)
	public class AnimatedTexture
	{
		public AnimatedTexture(AnimatedTexture anim) //Clone constructor
		{
			texture = anim.texture;
			hShift = anim.hShift;
			offset = anim.offset;
			w = anim.w;
			h = anim.h;
			curFrame = 0.0f;
			maxFrame = anim.maxFrame;
			speed = anim.speed;
		}

		public AnimatedTexture(JObject anim, int hShift) : this(anim)
		{
			this.hShift = hShift * h;
		}

		public AnimatedTexture(JObject anim)
		{
			texture = Main.content.Load<Texture2D>((string)anim["Texture"]);
			w = (int)anim["Width"];
			h = (int)anim["Height"];
			maxFrame = (float)anim["Frames"];
			speed = (float)anim["Speed"];

			offset = new Vector2((float?)anim["xOffset"] ?? 0.0f, (float?)anim["yOffset"] ?? 0.0f);

			hShift = 0;
			curFrame = 0.0f;
		}

		public AnimatedTexture(Texture2D texture, int frameCount, float speed, int w, int h, int hShift = 0, float xOffset = 0.0f, float yOffset = 0.0f)
		{
			this.texture = texture;
			this.hShift = hShift * h;
			this.offset = new Vector2(xOffset, yOffset);
			this.w = w;
			this.h = h;
			this.curFrame = 0.0f;
			this.maxFrame = frameCount;
			this.speed = speed;
		}

		public readonly Texture2D texture;

		public readonly int hShift;
		public readonly Vector2 offset;
		public readonly int w;
		public readonly int h;

		protected float curFrame; //Current frame (Float to allow incrementing every frame)
		protected readonly float maxFrame; //Maximum frame (Float to avoid casting)
		protected readonly float speed; //Speed frames/tick

		public Rectangle getCurFrame()
		{
			return new Rectangle(w * (int)curFrame, hShift, w, h);
		}
		public void incrementAnimation(float mult)
		{
			curFrame += speed * mult;
			if (curFrame >= maxFrame)
			{
				curFrame -= maxFrame;
			}
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
