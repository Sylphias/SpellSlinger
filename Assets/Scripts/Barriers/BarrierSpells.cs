using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Spells
{
	public abstract class BarrierSpells:NetworkBehaviour,IBarrier,ISpell
	{
		private float duration, cooldown, damage;
		private int charges; 

		public string GetSpellType{
			get{return "barrier";}
		}

		void Awake()
		{
			Init();
		}
		
		public float Cooldown{ get; set;}
		public float Damage{ get; set;}
		public float Duration{ get; set;}
		public int Charges{ get; set;}

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

