using System;
using UnityEngine.Networking;
using UnityEngine;
namespace Spells
{
	public abstract class UtilitySpell: NetworkBehaviour,ISpell
	{
		[SyncVar]
		private float damage;
		[SyncVar]
		private float cooldown;

		void Awake(){
			Init ();
		}

		public float Damage{get{return damage;}set{damage = value;}}
		public float Cooldown{get{return cooldown;}set{cooldown = value;}}
		public GameObject Player{get;set;}

		public string GetSpellType{get{return "utility";}}

		public abstract void Init ();
	}

}

