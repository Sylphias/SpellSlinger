using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Spells{
	public class IceBall : ProjectileSpell {
		private GameObject impactPrefab;
		private ContactPoint point;

		// Use this for initialization
		public override void Init ()
		{
			Damage = 10;
			ProjectileSpeed = 20;
			ExplosionForce = 10;
			Duration = 5;
			Radius = 10;
			Cooldown = 2;
		}

		void Start () {
			impactPrefab = Resources.Load ("Spells/FrostImpactMega", typeof(GameObject))as GameObject;
			Destroy (gameObject, Duration);
		}

		void OnCollisionEnter(Collision col){
			point = col.contacts [0];
			Destroy (gameObject);
		}

		void OnDestroy(){
			GameObject go = (GameObject)Instantiate (impactPrefab, gameObject.transform.position, gameObject.transform.rotation);
			Vector3 explosionPoint;
			explosionPoint = gameObject.transform.position;
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("TakeDamage", Damage);
			messages.Add ("RpcChilled", 0.5f);
			ExplosionUtilities.ExplosionScan (this,messages, colliders,gameObject.transform.position);
			Destroy (go, 1);
		}
			

		// Update is called once per frame
		void Update () {
			transform.Translate (Vector3.forward*Time.deltaTime*ProjectileSpeed);	
		}
	}
}