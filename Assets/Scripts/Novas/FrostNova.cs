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
			Damage = 40;
			Cooldown = 10;
			ExplosionForce = 0;
		}

		// Use this for initialization
		void Start () {
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,Radius);
			GetComponent<ParticleSystem> ().Play();
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("RpcTakeDamage", Damage);
			messages.Add ("RpcChilled", 0.5f);
			ExplosionUtilities.ExplosionScan (this,messages, colliders, gameObject.transform.position);
			Destroy (gameObject);
		}
	}
}

