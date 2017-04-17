using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFloor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider col){
		if (col.tag == "Player")
			col.gameObject.GetComponent<PlayerHit> ().TakeDamage (10 * Time.deltaTime);
	}
}
