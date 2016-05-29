using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ProjectBueno.Utility;
using ProjectBueno.Game.Entities;
using ProjectBueno.Engine.World;

namespace ProjectBueno.Game.Enemies
{
	public class Enemy : Entity //Make abstract after testing
	{
		public enum States : int
		{
			STANDING,
			WALKING
		}

		public Enemy(JObject data, string ID) : base(Vector2.Zero, null) //Template enemy constructor
		{
			JObject stats = (JObject)data["Stats"];

			loadTextures((JObject)data["Animations"]);

			id = ID;

			health = (float)stats["Health"];
			var speedList = stats["Speed"];
			speeds = new[] { (float)speedList["Forest"], (float)speedList["Desert"], (float)speedList["Cold"], (float)speedList["Solid"] };
			speedDeviation = (double)stats["SpeedDeviation"];
			size = new Vector2((float)stats["Width"], (float)stats["Height"]);
			damage = (float)stats["Damage"];
			hitForce = (float)stats["HitForce"];

			spawnBiome = (Biome)Enum.Parse(typeof(Biome), (string)stats["Biome"]);

			state = (int)States.STANDING;
			dir = Dir.DOWN;

			barOffset = new Vector2((size.X - barSize.X) * 0.5f, size.Y + barDistance);
		}

		protected Enemy(Enemy data, Vector2 pos, GameHandler game) : base(pos, game) //Instance enemy constructor. To be called by Spawn()
		{
			foreach (AnimatedTexture tex in data.textures)
			{
				textures.Add(new AnimatedTexture(tex));
			}

			id = data.id;

			health = data.health;
			speeds = Array.ConvertAll(data.speeds, speed => speed + (float)(random.NextDouble() * 2.0 * data.speedDeviation - data.speedDeviation));
			size = data.size;
			damage = data.damage;
			hitForce = data.hitForce;

			spawnBiome = data.spawnBiome;

			state = (int)States.STANDING;
			dir = Dir.DOWN;

			barOffset = data.barOffset;
		}

		static Enemy()
		{
			barSize = new Vector2((float)Main.Config["healthBar"]["w"], (float)Main.Config["healthBar"]["h"]);
			barDistance = (float)Main.Config["healthBar"]["d"];
			random = new Random();
		}

		public string id;

		public float damage { get; protected set; }
		public float hitForce { get; protected set; }

		public Biome spawnBiome;

		public Entity target;

		protected Vector2 barOffset;
		protected static readonly Vector2 barSize;
		protected static readonly float barDistance;
		protected static readonly Random random;

		protected double speedDeviation;

		//Spawn new enemy using current enemy as a template.
		public virtual Enemy Spawn(Vector2 pos, GameHandler game) //For EnemyManager, to be overridden to use derived constructor.
		{
			return new Enemy(this, pos, game);
		}

		public int GetSpawnChance(GameHandler game)
		{
			return game.terrain.getTileAtPos(game.player.pos + game.player.size).ToBiome() == spawnBiome ? 1 : 0;
		}


		public override void updateState()
		{
			if (isAlly)
			{
				speeds = Array.ConvertAll(speeds, speed => speed * 1.25f);
				if (target == game.player)
				{
					target = null;
				}
				game.player.knowledgePoints += 10;
			}
		}

		public override void Update()
		{
			AI();
			base.Update();
		}

		public void AI() //Make abstract after testing
		{
			if (isAlly && (target == null || target.isDead || target.isAlly))
			{
				target = GetClosest(game.entities.Where(ent => !ent.isAlly));
			}
			else if (!isAlly && (target == null || !target.isAlly))
			{
				target = game.player;
			}

			Vector2 totalMove = Vector2.Zero;

			if (target != null)
			{
				float speed = speeds[(int)game.terrain.getTileAtPos(pos + size).ToBiome()];

				totalMove = target.pos - pos;
				totalMove.Normalize();
				if (float.IsNaN(totalMove.X))
				{
					totalMove = Vector2.Zero;
				}
				totalMove *= speed;

				curTexture.incrementAnimation(speed);
			}

			moveDir(totalMove);
			if (totalMove != Vector2.Zero)
			{
				state = (int)States.WALKING;
			}
			else
			{
				state = (int)States.STANDING;
			}

			pos += totalMove;

			if (target != null && checkCollision(target.pos, target.size))
			{
				onTargetCollide(target);
			}
		}

		public override void loadTextures(JObject animData)
		{
			foreach (States st in Enum.GetValues(typeof(States)))
			{
				loadTexture((JObject)animData[st.ToString()]);
			}
		}

		public override void Draw()
		{
			base.Draw();
			Main.spriteBatch.Draw(Main.boxel, pos + barOffset, null, isAlly ? Color.DarkGreen : Color.DarkRed, 0f, Vector2.Zero, barSize, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(Main.boxel, pos + barOffset, null, isAlly ? Color.Lime : Color.Red, 0f, Vector2.Zero, barSize.MultX(health * maxHealthMult), SpriteEffects.None, 0f);
		}

		public override void onTargetCollide(Entity target)
		{
			Vector2 pushback = target.pos - pos;
			pushback.Normalize();
			pushback *= hitForce;
			target.dealDamage(damage, pushback, 5); //HARDCODE COOLDOWN 5
		}
	}
}
