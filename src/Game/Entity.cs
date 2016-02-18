using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using ProjectBueno.Game.Tiles;
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
	public enum Dir : int
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
			_state = -1; //Set initial value to be non-zero, so setting state detects change
		}

		public Vector2 pos { get; protected set; }
		public Vector2 size { get; protected set; }
		public Vector2 knockback { get; protected set; }
		public int damageCooldown { get; protected set; }
		protected float speed;
		public float health { get; protected set; }

		protected const int DAMAGECOOLDOWN = 15;
		protected const float KNOCKBACKDAMPENING = 0.75f;

		protected readonly GameHandler game;
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
					curTexture = textures[value*4+(int)dir];
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
					curTexture = textures[state*4+(int)value];
					curTexture.resetFrame();
					_dir = value;
				}
			}
		}
		protected AnimatedTexture curTexture;

		protected List<AnimatedTexture> textures = new List<AnimatedTexture>();

		public bool checkTilesCollision() //Might not work
		{
			return game.colMap[(int)((pos.X + size.X) * Tile.TILEMULT)][(int)((pos.Y + size.Y) * Tile.TILEMULT)]
				|| game.colMap[(int)(pos.X * Tile.TILEMULT)][(int)((pos.Y + size.Y) * Tile.TILEMULT)] 
				|| game.colMap[(int)((pos.X + size.X) * Tile.TILEMULT)][(int)(pos.Y * Tile.TILEMULT)] 
				|| game.colMap[(int)(pos.X * Tile.TILEMULT)][(int)(pos.Y * Tile.TILEMULT)];
		}
		public bool checkCollision(Vector2 entPos, Vector2 entSize)
		{
			return pos.X + size.X > entPos.X && entPos.X + entSize.X > pos.X && pos.Y + size.Y > entPos.Y && entPos.Y + entSize.Y > pos.Y;
        }
		public virtual void dealDamage(float amount, Vector2 direction)
		{
			if (damageCooldown <= 0)
			{
				knockback += direction;
				health -= amount;
				damageCooldown = DAMAGECOOLDOWN;
			}
		}

		#warning Enemy method onPlayerCollide() in Entity class. To move.
		public abstract void onPlayerCollide(Player player);

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
			foreach (Dir dr in Enum.GetValues(typeof(Dir)))
			{
				textures.Add(new AnimatedTexture(loadedTex, (int)anim[dr.ToString()]["Frames"], (float)anim[dr.ToString()]["Speed"], w, h, 0, (int)dr * h));
			}
		}

		///<summary>
		///Updates knockback and damage cooldown.
		///Should be called after custom update logic.
		///</summary>
		public virtual void Update()
		{
			pos += knockback;
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
			curTexture.incrementAnimation();
			Main.spriteBatch.Draw(curTexture.texture, pos, curTexture.getCurFrame(), damageCooldown > 0 ? Color.Red : Color.White);
		}
	}
}