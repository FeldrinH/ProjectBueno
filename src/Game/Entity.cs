using Microsoft.Xna.Framework;
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
		public static Dir DirEnum(this Vector2 dir)
		{
			return Math.Abs(dir.X) > Math.Abs(dir.Y) ? (dir.X < 0.0f ? Dir.LEFT : Dir.RIGHT) : (dir.Y < 0.0f ? Dir.UP : Dir.DOWN);
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

		protected const int DAMAGECOOLDOWN = 30;
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

		public bool checkTilesCollision()
		{
			return game.colMap[(int)((pos.X + size.X) * Tile.TILEMULT)][(int)((pos.Y + size.Y) * Tile.TILEMULT)]
				|| game.colMap[(int)(pos.X * Tile.TILEMULT)][(int)((pos.Y + size.Y) * Tile.TILEMULT)] 
				|| game.colMap[(int)((pos.X + size.X) * Tile.TILEMULT)][(int)(pos.Y * Tile.TILEMULT)] 
				|| game.colMap[(int)(pos.X * Tile.TILEMULT)][(int)(pos.Y * Tile.TILEMULT)];
		}
		public bool checkEntityCollision(Entity entity)
		{
			return pos.X + size.X > entity.pos.X && entity.pos.X + entity.size.X > pos.X && pos.Y + size.Y > entity.pos.Y && entity.pos.Y + entity.size.Y > pos.Y;
        }
		public virtual void dealDamage(float amount, Vector2 direction)
		{
			if (damageCooldown <= 0)
			{
				knockback = direction;
				health -= amount;
				damageCooldown = DAMAGECOOLDOWN;
			}
		}
		public abstract void loadTextures(JObject animData);
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
				//Console.WriteLine(knockback);
			}
			else
			{
				knockback = Vector2.Zero;
			}
		}
		public abstract void Draw();
	}
}