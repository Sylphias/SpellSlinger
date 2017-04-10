using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Spells{
	public abstract class NovaSpell : NetworkBehaviour,IExplosion,ISpell {

		private float damage, radius, cooldown = 10, explosionForce;

		public string GetSpellType{
			get{return "nova";}
		}

		void Awake(){
			Init ();
		}
		public float ExplosionForce{get;set;}
		public float Radius {get;set;}
		public float Damage {get;set;}
		public float Cooldown{get;set;}

		public abstract void Init ();

		public void Init(float cooldown, float damage, float radius, float explosionForce){
			Cooldown = cooldown;
			Damage = damage;
			Radius = radius;
			ExplosionForce = explosionForce;
		}
	}
}