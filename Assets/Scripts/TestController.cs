using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class TestController : NetworkBehaviour {
	public GameObject bulletPrefab;
	// Use this for initialization
    public Button button0;
	void Start () {

	}
	public override void OnStartLocalPlayer()
	{
		if (!isLocalPlayer)
        {
            return;
        }
		button0 = GameObject.Find("Cast0").GetComponent<Button>();
        Debug.Log("AddingListener");
        button0.onClick.AddListener(delegate () { this.CmdFire(); });
    	GetComponent<MeshRenderer>().material.color = Color.blue;
	}
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdFire();
		}
	}

	[Command]
	void CmdFire()
	{	
		GameObject gun = transform.Find("SpellCastPoint").gameObject;
		// Create the Bullet from the Bullet Prefab
		GameObject spellPrefab = Resources.Load("Fireball", typeof(GameObject)) as GameObject;
		var spell = (GameObject)Instantiate(spellPrefab, gun.transform.position, gun.transform.rotation);
		NetworkServer.Spawn(spell);
  			// var bullet = (GameObject)Instantiate(
        //     bulletPrefab,
        //     gun.transform.position, gun.transform.rotation);

        // // Add velocity to the bullet
        // bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;
		// NetworkServer.Spawn(bulletPrefab);
        // // Destroy the bullet after 2 seconds
        // Destroy(bullet, 2.0f);        
	}

	
}
