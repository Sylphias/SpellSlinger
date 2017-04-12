using UnityEngine;
using System.Collections;
namespace Spells{
	public class BurnDebuff:IBuffable
	{
		private bool isFinished;
		public float TickTime{get;set;}
		public string Type {
			get{ return "BurnDebuff"; }
		}
		public float ComparableValue{
			get{return DamagePerSecond ;}
		}
		public float DamagePerSecond{get;set;}
		public float FinishTime{get;set;}
		public bool Finished{
			get{
				if (FinishTime < Time.time) {
					return true;
				} else {
					return false;
				}
			}
		}
		public BurnDebuff(float damagePerSecond){
			FinishTime = Time.time + 5.0f; // Adjust the slow timing
			DamagePerSecond = damagePerSecond;
			TickTime = 1;
		}
		//Resetting Burn does nothing
		public void Reset(Component victim){
			return;
		}

		public void Apply(Component victim){
			if (victim as Player) {
				Player ps = (Player)victim;
				ps.TakeDamage(DamagePerSecond);
			}
		}

	}

}