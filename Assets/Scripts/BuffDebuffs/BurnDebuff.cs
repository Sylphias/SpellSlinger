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
		public float TimeElapsed{ get; set; }
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
		public BurnDebuff(float damagePerSecond,float duration){
			FinishTime = Time.time + duration;
			DamagePerSecond = damagePerSecond;
			TickTime = 1;
			TimeElapsed = 0;
		}
		//Resetting Burn does nothing
		public void Reset(GameObject victim){
			return;
		}

		public void Apply(GameObject victim){
			HealthbarController playerHealth = (HealthbarController)victim.GetComponent<HealthbarController>();
			playerHealth.CmdTakeDamage(DamagePerSecond);
		}

	}

}