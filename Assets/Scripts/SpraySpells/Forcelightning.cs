using System;
using UnityEngine;
using UnityEngine.Networking;
namespace Spells
{
	public class Forcelightning:SpraySpell
	{
		public override void Init(){
			Cooldown = 1.0f;
			Damage = 5.0f;
			Radius = 1.0f;
		}

		void Start () {
			Destroy(gameObject,1f);
		}
		void FixedUpdate(){
			if (player != null) {
				transform.position = player.transform.position + transform.forward * 3;
				transform.rotation = player.transform.rotation;
				RaycastHit[] rchArr = Physics.SphereCastAll (transform.position + transform.forward, Radius, player.transform.forward, 10.0f);
				if (rchArr.Length != 0) {
					foreach (RaycastHit hit in rchArr) {
						if (hit.collider.tag == "Player")
							hit.collider.gameObject.GetComponent<PlayerHit> ().TakeDamage (Damage * Time.deltaTime);
					}
				}
			}
		}
		void OnDestroy(){
			
		}

		// Update is called once per frame

		void Update () {
		}
			
	}
}

