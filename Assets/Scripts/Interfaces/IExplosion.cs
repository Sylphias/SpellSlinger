namespace Spells
{
    // This interface is for all spells that have an exploding effect.
    public interface IExplosion
    {
        float Radius{ get; set;}
		float ExplosionForce{ get; set;}
    }
}