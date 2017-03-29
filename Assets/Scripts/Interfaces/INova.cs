using System;

namespace Spells
{
	public interface INova
	{
		float Radius{ get; set;}
		float Damage{ get; set;}
		float Cooldown{ get;set;}
		float ExplosionForce{ get; set;}
		void initialize(float cooldown, float damage, float radius,float explosionForce);
	}
}

