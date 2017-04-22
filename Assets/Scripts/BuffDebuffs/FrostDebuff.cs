using UnityEngine;
using System.Collections;
namespace Spells
{
    public class FrostDebuff : IBuffable
    {
        private bool isFinished;
        private float finishTime;
        private float tickTime, speedMultiplier, oldSpeedValue, oldRotationValue;
        public FrostDebuff(float duration, float tickTime, float speedMult, float speed, float rotation)
        {
            finishTime = Time.time + 5;
            speedMultiplier = speedMult;
            this.tickTime = tickTime;
            this.oldSpeedValue = speed;
            this.oldRotationValue = rotation;
        }
        public FrostDebuff(float speed, float rotation,float duration)
        {
            finishTime = Time.time + duration; // Adjust the slow timing
            TimeElapsed = 0;
            tickTime = 0;
            speedMultiplier = 0.5f;
            oldSpeedValue = speed;
            oldRotationValue = rotation;

        }
        public float ComparableValue
        {
            get { return speedMultiplier; }
        }
        public float TimeElapsed { get; set; }

        public string Type
        {
            get { return "FrostDebuff"; }
        }
        public float TickTime { get; set; }
        public float FinishTime
        {
            get { return finishTime; }
            set { finishTime = value; }
        }

        public bool Finished
        {
            get
            {
                if (finishTime < Time.time)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public float SpeedMultiplier
        {
            get { return speedMultiplier; }
            set { speedMultiplier = value; }
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
