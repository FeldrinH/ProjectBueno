using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using System;
using System.Collections.Generic;

namespace ProjectBueno.Game.Entities
{
	public static class DirExtensions
	{
		private static readonly Vector2[] dirVector = { new Vector2(0.0f, -1.0f), new Vector2(0.0f, 1.0f), new Vector2(-1.0f, 0.0f), new Vector2(1.0f, 0.0f) };
		public static Vector2 Vector(this Dir index)
		{
			return dirVector[(int)index];
		}
	}
	public static class VectorExtensions
	{
		public static Dir DirEnum(this Vector2 vec)
		{
			return Math.Abs(vec.X) > Math.Abs(vec.Y) ? (vec.X < 0.0f ? Dir.LEFT : Dir.RIGHT) : (vec.Y < 0.0f ? Dir.UP : Dir.DOWN);
		}
	}
	public enum Dir
	{
		UP = 0,
		DOWN = 1,
		LEFT = 2,
		RIGHT = 3
	}

	public abstract class Entity
	{
		public Entity(Vector2 pos, GameHandler game)
		{
			this.pos = pos;
			this.game = game;

			maxHealth = 100.0f; //Set default maxHealth
			maxHealthMult = 1.0f / maxHealth;
			_state = -1; //Set initial value to be non-zero, so setting state detects change
		}

		public Vector2 pos { get; set; } //To protect
		public Vector2 size { get; protected set; }
		public Vector2 knockback { get; protected set; }
		public int damageCooldown { get; protected set; }
		public float[] speeds { get; protected set; }
		protected float health;
		public float maxHealth { get; protected set; }
		public float maxHealthMult { get; protected set; }
		public bool canDamage { get { return damageCooldown <= 0; } }
		public int control;
		public bool isAlly { get { return control >= 90; } }

		public float Health
		{
			get
			{
				return health;
			}
			set
			{
				health = MathHelper.Min(value, maxHealth);
			}
		}

		public bool isDead
		{
			get
			{
				return health <= 0.0f;
			}
		}

		protected const int DAMAGECOOLDOWN = 5;
		protected const float KNOCKBACKDAMPENING = 0.75f;

		protected GameHandler game;
		private int _state;
		private Dir _dir;
		public int state
		{
			get
			{
				return _state;
			}
			set
			{
				if (value != _state)
				{
					curTexture = textures[value * 4 + (int)dir];
					curTexture.resetFrame();
					_state = value;
				}
			}
		}
		public Dir dir
		{
			get
			{
				return _dir;
			}
			set
			{
				if (value != _dir)
				{
					curTexture = textures[state * 4 + (int)value];
					curTexture.resetFrame();
					_dir = value;
				}
			}
		}
		protected AnimatedTexture curTexture;

		protected List<AnimatedTexture> textures = new List<AnimatedTexture>();

		public virtual void updateState()
		{
		}

		public bool checkCollision(Vector2 entPos, Vector2 entSize)
		{
			return pos.X + size.X > entPos.X && entPos.X + entSize.X > pos.X && pos.Y + size.Y > entPos.Y && entPos.Y + entSize.Y > pos.Y;
		}
		public virtual void dealDamage(float amount, Vector2 direction, int dmgCooldown)
		{
			if (damageCooldown <= 0)
			{
				knockback += direction;
				health -= amount;
				damageCooldown = dmgCooldown;
			}
		}

#warning Enemy method onPlayerCollide() in Entity class. To move.
		public abstract void onTargetCollide(Entity target);

		public void moveDir(Vector2 vec)
		{
			if (vec == Vector2.Zero)
			{
				return;
			}
			if (Math.Abs(vec.X) > Math.Abs(vec.Y))
			{
				if (vec.X > 0.0f)
				{
					dir = Dir.RIGHT;
				}
				else
				{
					dir = Dir.LEFT;
				}
			}
			else
			{
				if (vec.Y > 0.0f)
				{
					dir = Dir.DOWN;
				}
				else
				{
					dir = Dir.UP;
				}
			}
		}

		public abstract void loadTextures(JObject animData);
		public virtual void loadTexture(JObject anim)
		{
			Texture2D loadedTex = Main.content.Load<Texture2D>((string)anim["Texture"]);
			int w = (int)anim["Width"];
			int h = (int)anim["Height"];
			float xOffset = (float?)anim["xOffset"] ?? 0.0f;
			float yOffset = (float?)anim["yOffset"] ?? 0.0f;
			foreach (Dir dr in Enum.GetValues(typeof(Dir)))
			{
				textures.Add(new AnimatedTexture(loadedTex, (int)anim[dr.ToString()]["Frames"], (float)anim[dr.ToString()]["Speed"], w, h, (int)dr, xOffset, yOffset));
			}
		}

		///<summary>
		///Updates knockback and damage cooldown.
		///Should be called after custom update logic.
		///</summary>
		public virtual void Update()
		{
			if (!game.terrain.isColliding(pos + knockback, size))
			{
				pos += knockback;
			}
			if (damageCooldown > 0)
			{
				--damageCooldown;
			}
			if (knockback.LengthSquared() > 0.005f)
			{
				knockback *= KNOCKBACKDAMPENING;
			}
			else
			{
				knockback = Vector2.Zero;
			}
		}
		public virtual void Draw()
		{
			Main.spriteBatch.Draw(curTexture.texture, pos + curTexture.offset, curTexture.getCurFrame(), damageCooldown > 0 ? Color.Red : Color.White);
		}
		public virtual void DrawRaw() //Draws without any effects, just for shape
		{
			Main.spriteBatch.Draw(curTexture.texture, pos + curTexture.offset, curTexture.getCurFrame(), Color.White);
		}
		public virtual void DrawOutline(Color outlineColor)
		{
			Vector2 cpos = pos + curTexture.offset;
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(1.0f, 0.0f), curTexture.getCurFrame(), outlineColor);
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(0.0f, 1.0f), curTexture.getCurFrame(), outlineColor);
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(-1.0f, 0.0f), curTexture.getCurFrame(), outlineColor);
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(0.0f, -1.0f), curTexture.getCurFrame(), outlineColor);
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(1.0f, 1.0f), curTexture.getCurFrame(), outlineColor);
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(-1.0f, -1.0f), curTexture.getCurFrame(), outlineColor);
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(1.0f, -1.0f), curTexture.getCurFrame(), outlineColor);
			Main.spriteBatch.Draw(curTexture.texture, cpos + new Vector2(-1.0f, 1.0f), curTexture.getCurFrame(), outlineColor);
		}

		public void DrawDebug()
		{
			Main.spriteBatch.Draw(Main.boxel, pos, new Rectangle(0, 0, (int)size.X, (int)size.Y), Color.Red * 0.5f);
		}
	}
}