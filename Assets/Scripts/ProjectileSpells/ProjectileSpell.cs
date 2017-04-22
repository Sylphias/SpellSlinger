using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Spells
{
	public abstract class ProjectileSpell: NetworkBehaviour,ISpell,IProjectile,IExplosion{
		
		[SyncVar]
		private float duration;
		[SyncVar]
		private float cooldown;
		[SyncVar]
		private float damage;
		[SyncVar]
		private float radius;
		[SyncVar]
		private float explosionForce;
		[SyncVar]
		private float projectileSpeed;
		void Awake(){
			Init();
		}

		
		public string GetSpellType{
			get{return "projectile";}
		}
		public float ExplosionForce{get{return explosionForce;}set{explosionForce = value;}}
		public float Duration{get{return duration;}set{duration = value;}}
		public float Cooldown{get{return cooldown;}set{cooldown = value;}}
		public float Damage{get{return damage;}set{damage = value;}}
		public float Radius{get{return radius;}set{radius = value;}}
		public float ProjectileSpeed{get{return projectileSpeed;}set{projectileSpeed = value;}}

		public abstract void Init ();


	}

}


