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

namespace ProjectBueno.Game.Entities
{
	class Enemy : Entity //Make abstract after testing
	{
		public enum States : int
		{
			STANDING,
			WALKING
		}

		public Enemy(Vector2 pos, GameHandler game) : base(pos, game)
		{
			JObject data = JObject.Parse(File.ReadAllText("Content/EnemyTest.json"));
			JObject stats = (JObject)data["Stats"];

			loadTextures((JObject)data["Animations"]);

			health = (float)stats["Health"];
			speed = (float)stats["Speed"];
			size = new Vector2((float)stats["Width"], (float)stats["Height"]);
			damage = (float)stats["Damage"];
			hitForce = (float)stats["HitForce"];

			state = (int)States.STANDING;
			dir = Dir.DOWN;

			barOffset = new Vector2((size.X - barSize.X) * 0.5f, size.Y + barDistance);
		}

		static Enemy()
		{
			barSize = new Vector2((float)Main.Config["healthBar"]["w"], (float)Main.Config["healthBar"]["h"]);
			barDistance = (float)Main.Config["healthBar"]["d"];
			random = new Random();
		}

		public float damage { get; protected set; }
		public float hitForce { get; protected set; }

		public Entity target;

		protected Vector2 barOffset;
		protected static readonly Vector2 barSize;
		protected static readonly float barDistance;
		protected static readonly Random random;

		public override void updateState()
		{
			if (isAlly)
			{
				speed = game.player.speed;
				if (target == game.player)
				{
					target = null;
				}
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
				var targetList = game.entities.FindAll(ent => !ent.isAlly);
				target = targetList.Count == 0 ? null : targetList[random.Next(targetList.Count)];
			}
			else if (target == null)
			{
				target = game.player;
			}

			Vector2 totalMove = Vector2.Zero;
			if (target != null)
			{
				totalMove = target.pos - pos;
				totalMove.Normalize();
				totalMove *= speed;
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
