namespace ProjectBueno.Game.Entities
{
	public abstract class Projectile
	{
		public abstract bool toRemove { get; }

		public abstract void Update();
		public abstract void Draw();
	}
}