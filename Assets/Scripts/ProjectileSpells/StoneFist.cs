using System;
using UnityEngine;
using System.Collections.Generic;
namespace Spells
{
	public class StoneFist:ProjectileSpell
	{

		private GameObject stonefistPrefab;
		private ContactPoint point;
		private GameObject impactPrefab;
		private float dot;

		// DOT is damage over time

		public float DoT {
			get{ return dot; }
			set{ this.dot = value; }
		}
			
		public void initialize(float cooldown, float duration, float radius, float dmg, float speed, float force, float dot, float knockbackFoce){
			Cooldown = cooldown;
			Duration = duration;
			Radius = radius;
			Damage = dmg;
			ProjectileSpeed = speed	;
			DoT = dot;
			ExplosionForce = force;
		}

		void Start () {
			impactPrefab = Resources.Load ("EarthImpactMega", typeof(GameObject))as GameObject;
			Destroy (gameObject,Duration);
		}


		void OnCollisionEnter(Collision col){
			Debug.Log (col.collider.tag);
			if(col.collider.tag =="Player"){
				col.collider.SendMessage ("RpcTakeDamage", 10);
				col.collider.GetComponent<Rigidbody> ().AddForce (transform.forward*10,ForceMode.Impulse);
			}
			else{
				Destroy(gameObject);
			}
		}

		void OnCollisionStay(Collision col){
			if(col.collider.tag =="Player"){
				col.collider.SendMessage ("TakeDamage", 10);
				col.collider.GetComponent<Rigidbody> ().AddForce (transform.forward*10,ForceMode.Impulse);
			}
		}

		void OnDestroy(){
			GameObject go = (GameObject)Instantiate (impactPrefab, gameObject.transform.position, gameObject.transform.rotation);
			Vector3 explosionPoint;
			explosionPoint = gameObject.transform.position;
			Collider[] colliders = Physics.OverlapSphere (explosionPoint,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("RpcTakeDamage", Damage);
			explosionScan (messages, colliders, explosionPoint);
			Destroy (go, 1);
		}

		// Update is called once per frame
		void Update () {
			transform.Translate (Vector3.forward*Time.deltaTime*ProjectileSpeed);	
		}
	}

}

