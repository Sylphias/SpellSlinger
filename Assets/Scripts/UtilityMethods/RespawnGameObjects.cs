using System;
using UnityEngine.Networking;
using UnityEngine;
public class RespawnGameObjects:NetworkBehaviour
{
	public GameObject player;
	public float destroyTime;
	void Start(){
		Destroy (gameObject, destroyTime);
	}
	void FixedUpdate(){
		transform.position = player.transform.position;
	}		

}

