using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using ProjectBueno.Game.Enemies;
using ProjectBueno.Game.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBueno.Engine
{
	public static class EnemyManager
	{
		private static List<Enemy> enemies;
		private static Random random = new Random();

		public static void Initialize()
		{
			enemies = new List<Enemy>();
			foreach (string fileName in Directory.GetFiles("Content/Enemies/", "*.json"))
			{
				JObject data = JObject.Parse(File.ReadAllText(fileName));
				enemies.Add(new Enemy(data, Path.GetFileNameWithoutExtension(fileName))); //For testing. Replace with reflection.
			}
		}

		public static Enemy CreateFromId(string id)
		{
			return enemies.Find(enemy => enemy.id == id);
		}

		public static Enemy SpawnEnemy(Vector2 pos, GameHandler game)
		{
			int totalChance = 0;
			foreach (Enemy enemy in enemies)
			{
				totalChance += enemy.GetSpawnChance(game);
			}
			if (totalChance == 0)
			{
				return null;
			}
			int enemyNum = random.Next(totalChance);
			totalChance = 0;
			foreach (Enemy enemy in enemies)
			{
				totalChance += enemy.GetSpawnChance(game);
				if (enemyNum < totalChance)
				{
					return enemy.Spawn(pos, game);
				}
			}
			return null;
		}
	}
}
