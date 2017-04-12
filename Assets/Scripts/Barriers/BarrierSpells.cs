using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Spells
{
	public abstract class BarrierSpells:NetworkBehaviour,IBarrier,ISpell
	{
		[SyncVar]
		private float duration;
		[SyncVar]
		private float cooldown;
		[SyncVar]
		private float damage;
		
		private int charges; 

		public string GetSpellType{
			get{return "barrier";}
		}

		void Awake()
		{
			Init();
		}
		
		public float Duration{get{return duration;}set{duration = value;}}
		public float Cooldown{get{return cooldown;}set{cooldown = value;}}
		public float Damage{get{return damage;}set{damage = value;}}

		public int Charges{ get{return charges;} set{charges = value;}}

		public abstract void Init ();

		public void OnHitReduceCharge(Collider col){
			if (col.tag == "Spell") {
				Charges--;
			}
			if (Charges == 0) {
				Destroy(gameObject);
			}
		}


	}
}

