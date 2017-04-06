using System;
using UnityEngine;


namespace Spells
{
	public class SwiftBuff:IBuffable
	{
		private bool isFinished;
		private float finishTime;
		private float tickTime, speedMultiplier, oldSpeedValue, oldRotationValue;

		public SwiftBuff (float duration,float tickTime,float speedMult, float speed, float rotation){
			finishTime =Time.time + 5;
			speedMultiplier = speedMult;
			this.tickTime = tickTime;
			this.oldSpeedValue = speed;
			this.oldRotationValue = rotation;
		}

		public SwiftBuff(float speed,float rotation){
			finishTime = Time.time + 5.0f; // Adjust the slow timing
			tickTime = 0;
			speedMultiplier = 2f;
			oldSpeedValue = speed;
			oldRotationValue = rotation;
		}
		public string Type {
			get{ return "swiftness"; }
		}

		public float FinishTime{
			get{ return finishTime; }
			set{ finishTime = value; }
		}

		public bool Finished{
			get{
				if (finishTime < Time.time) {
					return true;
				} else {
					return false;
				}
			}
		}

		public float TickTime{
			get{ return tickTime; }
			set{ tickTime = value; }
		}

		public float SpeedMultiplier {
			get{ return speedMultiplier; }
			set{ speedMultiplier = value; }
		}

		public void Reset(Component victim){
			if (victim as PlayerScript) {
				PlayerScript ps = (PlayerScript)victim;
				ps.MoveSpeed = oldSpeedValue;
				ps.RotationSpeed = oldRotationValue;
			}
		}

		public void Apply(Component victim){
			if (victim as PlayerScript) {
				PlayerScript ps = (PlayerScript)victim;
				ps.MoveSpeed= oldSpeedValue*speedMultiplier;
				ps.RotationSpeed = oldRotationValue*speedMultiplier;
			}
		}
	}
}

