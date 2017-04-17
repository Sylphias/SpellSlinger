using System;
using UnityEngine.Networking;
using UnityEngine;
public class RespawnGameObjects:NetworkBehaviour
{
	public float destroyTime;
	void Start(){
		Destroy (gameObject, destroyTime);
	}
	void Update(){
	}		

}

