using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FaceCameraScript : NetworkBehaviour {
	public Camera cameraToLookAt;

	public override void OnStartLocalPlayer()
	{
		cameraToLookAt = GameObject.FindWithTag("PlayerCamera").GetComponentInChildren<Camera>();
		base.OnStartLocalPlayer ();	
	}
	// Update is called once per frame
	void Update () {
		if(cameraToLookAt == null)
			GameObject.FindWithTag("PlayerCamera").GetComponentInChildren<Camera>();
		Vector3 v = cameraToLookAt.transform.position - transform.position;
		v.x = v.z = 0.0f;
		transform.LookAt (cameraToLookAt.transform.position - v);
		transform.rotation = cameraToLookAt.transform.rotation;
	}
}
