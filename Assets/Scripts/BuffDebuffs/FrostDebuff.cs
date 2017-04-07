using UnityEngine;
using System.Collections;
namespace Spells{
	public class FrostDebuff:IBuffable
	{
		private bool isFinished;
		private float finishTime;
		private float tickTime, speedMultiplier, oldSpeedValue, oldRotationValue;
		public FrostDebuff (float duration,float tickTime,float speedMult, float speed, float rotation){
			finishTime =Time.time + 5;
			speedMultiplier = speedMult;
			this.tickTime = tickTime;
			this.oldSpeedValue = speed;
			this.oldRotationValue = rotation;
		}

		public FrostDebuff(float speed,float rotation){
			finishTime = Time.time + 5.0f; // Adjust the slow timing
			tickTime = 0;
			speedMultiplier = 0.2f;
			oldSpeedValue = speed;
			oldRotationValue = rotation;
		}
		public string Type {
			get{ return "chilled"; }
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
			if (victim as Player) {
				Player ps = (Player)victim;
				ps.MoveSpeed = oldSpeedValue;
			}
		}
			
		public void Apply(Component victim){
			if (victim as Player) {
				Player ps = (Player)victim;
				ps.MoveSpeed= oldSpeedValue*speedMultiplier;
			}
		}
			
	}

}