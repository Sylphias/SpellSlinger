using System;
using UnityEngine;
using System.Collections.Generic;
namespace Spells
{
	public class Stonefist:ProjectileSpell
	{

		private GameObject stonefistPrefab;
		private ContactPoint point;
		private GameObject impactPrefab;
		private float dot;
		private float knockbackForce;

		// DOT is damage over time

		public float DoT { get; set;}
		public float KnockbackForce{ get; set; }

		public override void Init ()
		{
			Damage = 10;
			ProjectileSpeed = 5;
			ExplosionForce = 100;
			KnockbackForce = 300;
			Duration = 5;
			Radius = 10;
		}
			
		public void Init(float cooldown, float duration, float radius, float dmg, float speed, float force, float dot, float knockbackFoce){
			Cooldown = cooldown;
			Duration = duration;
			Radius = radius;
			Damage = dmg;
			ProjectileSpeed = speed	;
			DoT = dot;
			ExplosionForce = force;
			Cooldown = 2;
		}
			
		void Start () {

			impactPrefab = Resources.Load ("Spells/EarthImpactMega", typeof(GameObject))as GameObject;
			Destroy (gameObject,Duration);
		}


		void OnCollisionEnter(Collision col){
			if(col.collider.tag =="Player"){
				Player playerObj = col.collider.GetComponent<Player>();
				playerObj.RpcTakeDamage(Damage);
				playerObj.RpcKnockback(transform.forward,KnockbackForce);
			}
			else{
				Destroy(gameObject);
			}
		}

		void OnCollisionStay(Collision col){
			if(col.collider.tag =="Player"){
				col.collider.SendMessage ("RpcTakeDamage", 10);
				col.collider.GetComponent<Rigidbody> ().AddForce (transform.forward*10,ForceMode.Impulse);
			}
		}

		void OnDestroy(){
			GameObject go = (GameObject)Instantiate (impactPrefab, gameObject.transform.position, gameObject.transform.rotation);
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("RpcTakeDamage", Damage);
			ExplosionUtilities.ExplosionScan (this,messages,colliders, gameObject.transform.position);
			Destroy (go, 1);
		}

		// Update is called once per frame
		void Update () {
			transform.Translate (Vector3.forward*Time.deltaTime*ProjectileSpeed);	
		}
	}

}

