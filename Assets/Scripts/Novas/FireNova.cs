using System.Collections.Generic;
using UnityEngine;
namespace Spells
{
	public class FireNova:NovaSpell
	{
		private ContactPoint point;

		public override void Init ()
		{
			Radius = 50;
			Damage = 40;
			Cooldown = 1;
			ExplosionForce = 0;
	
		}

		// Use this for initialization
		void Start () {
			Debug.Log ("Frost");
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("RpcTakeDamage", Damage);
			messages.Add ("RpcBurned", 1);
			ExplosionUtilities.ExplosionScan (this,messages, colliders, gameObject.transform.position,true);
			Destroy (gameObject,2);
		}
	}
}
