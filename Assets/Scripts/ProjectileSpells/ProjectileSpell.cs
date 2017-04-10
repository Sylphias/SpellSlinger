using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Spells
{
	public abstract class ProjectileSpell: NetworkBehaviour,ISpell,IProjectile,IExplosion{
		
		private bool isCooldown;
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

		public float Duration{get;set;}
		public float Cooldown{get;set;}
		public float Damage{get;set;}
		public float Radius{get;set;}
		public float ExplosionForce{get;set;}
		public float ProjectileSpeed{get;set;}

		public abstract void Init ();


	}

}


