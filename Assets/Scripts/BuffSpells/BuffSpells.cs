using System;
using UnityEngine;
namespace Spells
{
	public class BuffSpells:MonoBehaviour,IBuffSpell
	{
		private float cooldown, duration;
		private GameObject player;
		public float Cooldown {
			get{ return cooldown; }
			set{ cooldown = value; }
		}
		public float Duration {
			get{ return duration; }
			set{ duration = value; }
		}

		public GameObject Player {
			get{ return player; }
			set{ player = value; }
		}

		public void initialize(float duration, float Cooldown,GameObject player){
			this.cooldown = cooldown;
			this.duration = duration;
			this.player = player;
		}

	}
}

