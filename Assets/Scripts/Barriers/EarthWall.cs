using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//This script is for the behaviour of the earth wall shield. 
//Expire time is the duration of how long the earth shield will last

namespace Spells{
	public class EarthWall : BarrierSpells {
		//Sets default values to the spell
		public override void Init(){
			Duration = 10;
			Charges = 5;
			Cooldown = 10;
		}
		void Start () {
			Destroy(gameObject,Duration);
		}
		
		// Update is called once per frame

		void Update () {
		}

		void OnTriggerEnter(Collider col){
			OnHitReduceCharge(col);
		}
	}
}