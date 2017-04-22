using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFloor : MonoBehaviour {
	private float spawnTimer;
	GameObject fireExp ;
	// Use this for initialization
	void Start () {
		spawnTimer = 0;
		fireExp = Resources.Load ("Spells/Firedot", typeof(GameObject))as GameObject;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider col){
		
		if (spawnTimer >= 0.3 && col.tag == "Player") {
			spawnTimer = 0;	
			GameObject exp =(GameObject) Instantiate (fireExp, col.gameObject.transform.position, col.gameObject.transform.rotation);
			Destroy (exp, 1);
		}

		spawnTimer += Time.deltaTime;
		if (col.tag == "Player")
			col.gameObject.GetComponent<PlayerHit> ().TakeDamage (10 * Time.deltaTime);
	}
}
