using UnityEngine;
using System.Collections;

//This script is for the behaviour of the earth wall shield. 
//Expire time is the duration of how long the earth shield will last


public class EarthWall : MonoBehaviour {
	public float expireTime;
	private int counter;
	// Use this for initialization
	void Start () {
		counter = 0;
		Destroy (gameObject, expireTime);
	}
	
	// Update is called once per frame

	void Update () {
	}

	void OnTriggerEnter(Collider col){
		if (col.tag != "player") {
			counter++;
		}
		if (counter == 5) {
			Destroy(gameObject);
		}
	}
}
