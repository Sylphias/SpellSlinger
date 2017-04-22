using System;
using UnityEngine;
using UnityEngine.Networking;
namespace Spells
{
	public abstract class SpraySpell:NetworkBehaviour,ISpell
	{
		[SyncVar]
		private float cooldown;
		[SyncVar]
		private float damage;
		[SyncVar]
		private float radius;

		public GameObject player{get;set;}
		public float Cooldown{get{return cooldown;}set{cooldown = value;}}
		public float Damage{get{return damage;}set{damage = value;}}
		public float Radius{get{return radius;}set{radius = value;}}


		public string GetSpellType{
			get{return "spray";}
		}

		void Awake()
		{
			Init();
		}

		public abstract void Init ();

	}
}

