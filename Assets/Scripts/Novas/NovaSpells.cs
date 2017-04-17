using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Spells{
	public abstract class NovaSpell : NetworkBehaviour,IExplosion,ISpell {

		[SyncVar]
		private float damage;
		[SyncVar]
		private float  radius;
		[SyncVar]
		private float  cooldown;
		[SyncVar]
		private float  explosionForce;

		public string GetSpellType{
			get{return "nova";}
		}

		void Awake(){
			Init ();

		}
		public float ExplosionForce{get{return explosionForce;}set{explosionForce = value;}}
		public float Cooldown{get{return cooldown;}set{cooldown = value;}}
		public float Damage{get{return damage;}set{damage = value;}}
		public float Radius{get{return radius;}set{radius = value;}}

		public abstract void Init ();

		public void Init(float cooldown, float damage, float radius, float explosionForce){
			Cooldown = cooldown;
			Damage = damage;
			Radius = radius;
			ExplosionForce = explosionForce;
		}
	}
}