﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Spells{
	public class IceBall : ProjectileSpell {
		private GameObject impactPrefab;
		private ContactPoint point;

		// Use this for initialization
		void Start () {
			Damage = 10;
			ProjectileSpeed = 7;
			ExplosionForce = 10;
			Duration = 5;
			impactPrefab = Resources.Load ("FrostImpactMega", typeof(GameObject))as GameObject;
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
			Collider[] colliders = Physics.OverlapSphere (explosionPoint,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("TakeDamage", Damage);
			messages.Add ("Chilled", 0.2f);
			explosionScan (messages, colliders, explosionPoint);
			Destroy (go, 1);
		}
		// Update is called once per frame
		void Update () {
			transform.Translate (Vector3.forward*Time.deltaTime*ProjectileSpeed);	
		}
	}
}