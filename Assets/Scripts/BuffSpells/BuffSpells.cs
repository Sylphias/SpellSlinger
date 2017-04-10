using System;
using UnityEngine;
using UnityEngine.Networking;
namespace Spells
{
	public abstract class BuffSpell:NetworkBehaviour,IBuffSpell
	{
		private float cooldown =10, duration;
		private GameObject player;

		public string GetSpellType{
			get{return "buff";}
		}
		public float Cooldown { get; set;}
		public float Duration { get; set;}
		public GameObject Player { get; set;}

		public abstract void Init ();
	}
}

