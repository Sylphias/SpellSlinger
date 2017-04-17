using System;
using UnityEngine;


namespace Spells
{
	public class SwiftBuff:IBuffable
	{
		private bool isFinished;
		private float tickTime, speedMultiplier, oldSpeedValue, oldRotationValue;
		public float TimeElapsed{get;set;}

		public SwiftBuff (float duration,float tickTime,float speedMult, float speed, float rotation){
			FinishTime =Time.time + 10;
			speedMultiplier = speedMult;
			this.tickTime = tickTime;
			this.oldSpeedValue = speed;
			this.oldRotationValue = rotation;
		}

		public SwiftBuff(float speed,float rotation){
			FinishTime = Time.time + 10.0f; // Adjust the slow timing
			tickTime = 0;
			TimeElapsed = 0;
			speedMultiplier = 2f;
			oldSpeedValue = speed;
			oldRotationValue = rotation;


		}
		public string Type {
			get{ return "SpeedBuff"; }
		}

		public float ComparableValue{
			get{return speedMultiplier ;}
		}
		public float TickTime{get;set;}
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

		public float SpeedMultiplier {
			get{ return speedMultiplier; }
			set{ speedMultiplier = value; }
		}

		public void Reset(GameObject victim)
		{
			victim.GetComponent<PlayerController>().CmdUpdateSpeed (oldSpeedValue);
		}

		public void Apply(GameObject victim)
		{

			PlayerController ps = victim.GetComponent<PlayerController>();
			ps.CmdUpdateSpeed (oldSpeedValue * speedMultiplier);

		}
	}
}

