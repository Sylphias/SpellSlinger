using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Spells{
	public class Fireball : ProjectileSpell
	{
		private ContactPoint point;
		private GameObject impactPrefab;


		public override void Init(){
			Damage = 20;
			ProjectileSpeed = 30;
			ExplosionForce = 70;
			Duration = 5;
			Radius = 10;
			Cooldown = 2;
		}
			
		void Start () {
			impactPrefab = Resources.Load ("Spells/FireImpactMega", typeof(GameObject))as GameObject;
			Destroy (gameObject, Duration);
		}

		void OnCollisionEnter(Collision col){
			point = col.contacts [0];
			Destroy (gameObject);
		}

		void OnDestroy(){
			GameObject go = (GameObject)Instantiate (impactPrefab, gameObject.transform.position, gameObject.transform.rotation);
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("TakeDamage", Damage);
			ExplosionUtilities.ExplosionScan (this,messages, colliders,gameObject.transform.position);
			Destroy (go, 1);
		}


		// Update is called once per frame
		void Update () {
			transform.Translate (Vector3.forward*Time.deltaTime*ProjectileSpeed);
	    }
	}
}
