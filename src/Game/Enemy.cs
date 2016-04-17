using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using ProjectBueno.Engine;
using Microsoft.Xna.Framework;
using System.IO;

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
		}

		static Enemy()
		{
			random = new Random();
		}

		public float damage { get; protected set; }
		public float hitForce { get; protected set; }

		public Entity target;

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
				target = targetList[random.Next(targetList.Count)];
			}
			else if (target == null)
			{
				target = game.player;
			}

			Vector2 totalMove = target.pos - pos;
			totalMove.Normalize();
			totalMove *= speed;

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

			if (checkCollision(target.pos, target.size))
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

		public override void onTargetCollide(Entity target)
		{
			Vector2 pushback = target.pos - pos;
			pushback.Normalize();
			pushback *= hitForce;
			target.dealDamage(damage, pushback, 5); //HARDCODE COOLDOWN 5
		}
	}
}
