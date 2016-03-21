namespace ProjectBueno.Game.Spells
{
	public abstract class Projectile
	{
		public abstract void Draw();
		public abstract void Update();

		protected const int TIMEOUT = 450;
	}
}