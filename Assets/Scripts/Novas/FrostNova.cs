using System.Collections.Generic;
using UnityEngine;
namespace Spells
{
	public class FrostNova:NovaSpells
	{
		private ContactPoint point;

		// Use this for initialization
		void Start () {
//			GameObject impactPrefab = AssetDatabase.LoadAssetAtPath ("Assets/MagicArsenal/MagicProjectiles/Prefabs/Frost/FrostImpactMega.prefab", typeof(GameObject))as GameObject;
//			Instantiate (impactPrefab);
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,Radius);
			Dictionary<string,float> messages = new Dictionary<string,float> ();
			messages.Add ("TakeDamage", Damage);
			messages.Add ("Chilled", 0.5f);
			explosionScan (messages, colliders, gameObject.transform.position);
			Destroy (gameObject);
		}

		// Update is called once per frame
		void Update () {

		}

	}
}

