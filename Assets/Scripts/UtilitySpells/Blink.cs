using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Spells
{
	public class Blink: UtilitySpell {

		private float blinkDistance;
		private GameObject blinkRing;
		private GameObject blinkTrace;
		public override void Init(){
			blinkDistance = 20;
			Damage = 0;
			Cooldown = 10;
		}

		// Use this for initialization
		void Start () {
			blinkRing = Resources.Load ("Spells/Blinkring", typeof(GameObject))as GameObject;
			blinkTrace = Resources.Load ("Spells/Blinktrace", typeof(GameObject))as GameObject;
			Destroy(gameObject,0.1f);
		}

		void OnDestroy (){
			RaycastHit rch;
			GameObject blinkRingStart = (GameObject)Instantiate(blinkRing,Player.transform.position,Player.transform.rotation);

			GameObject initBlinkTrace = (GameObject)Instantiate(blinkTrace,Player.transform.position,Player.transform.rotation);
			blinkRingStart.transform.Rotate (-90, 0, 0);
			Vector3 direction = Player.transform.forward;
			Vector3 pos;
			if (!Physics.Raycast (Player.transform.position,direction, out rch,blinkDistance)) {
				Debug.DrawLine (Player.transform.position, Player.transform.position + direction * blinkDistance, Color.green, 3);
				pos = Player.transform.position + direction * blinkDistance;
			}
			else
			{
				Debug.DrawLine (Player.transform.position,rch.point,Color.red,3);
				pos = rch.point;
			}
			GameObject blinkRingEnd= (GameObject)Instantiate(blinkRing,pos,Player.transform.rotation);
			blinkRingEnd.transform.Rotate (-90, 0, 0);
			Player.transform.position = pos;


			Destroy(blinkRingStart,2);
			Destroy(blinkRingEnd,2);
			Destroy(initBlinkTrace,1);
		}

		// Update is called once per frame
		void Update () {
			
		}
	}
}