using System.Collections.Generic;
using UnityEngine;
namespace Spells
{
	public class FrostNova:NovaSpell
	{
		private ContactPoint point;

		public override void Init ()
		{
			Radius = 50;
			Damage = 10;
			Cooldown = 5;
			ExplosionForce = 0;
	
		}

		// Use this for initialization
		void Start () {
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("TakeDamage", Damage);
			messages.Add ("Chilled", 0.2f);
			ExplosionUtilities.ExplosionScan (this,messages, colliders, gameObject.transform.position,true);
			Destroy (gameObject,2);
		}
	}
}

