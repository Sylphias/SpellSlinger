using System;
using UnityEngine;
using UnityEngine.Networking;
namespace Spells
{
	public abstract class BuffSpell:NetworkBehaviour,IBuffSpell,ISpell
	{
		[SyncVar]
		private float cooldown;
		[SyncVar]
		private float duration;
		[SyncVar]
		private float damage;
		[SyncVar]
		private GameObject player;
		void Awake()
		{
			Init();
		}
		public string GetSpellType{
			get{return "buff";}
		}
		public float Duration{get{return duration;}set{duration = value;}}
		public float Cooldown{get{return cooldown;}set{cooldown = value;}}
		public float Damage{get{return damage;}set{damage = value;}}

		public GameObject Player { get{return player;} set{player = value;}}
		
		public abstract void Init ();
	}
}

