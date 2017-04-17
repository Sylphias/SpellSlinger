using System;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Spells;



public class PlayerDeath:NetworkBehaviour
{
	private bool[] wasEnabled;
	float InvulerabilityTimer{get;set;}
	float DeathTimer{get;set;}
	private Animation playerAnimations;
    private HealthbarController hb;
	private System.Random rand;
	private float oldMovement,oldRotation; 
	//private GameObject gameOverTint;
	Player player;
	PlayerController controller;


	// Player when respawn is invulnerable for 5 seconds.
	public override void OnStartLocalPlayer(){
        if (!isLocalPlayer) return;
        player = GetComponent<Player>();
        playerAnimations = GetComponent<Animation>();
        hb = GetComponent<HealthbarController>();
        controller = GetComponent<PlayerController>();
        DeathTimer = 0;
        InvulerabilityTimer = 0;
        rand = new System.Random();
		base.OnStartLocalPlayer();
	}

	void FixedUpdate(){
        if (!isLocalPlayer)return;
        switch (player.state)
        {
            case "dead":
                if (DeathTimer > 2 && player.state == "dead")
                    RespawnSequence();
                DeathTimer += Time.deltaTime;
                break;
            case "endgame":
                ShowEndGameTitle();
                break;
            case "gameover":
                GameOverSequence();
                break;
            default:
                Invulnerable();
                break;
        }
    }

    void ShowEndGameTitle() {
	
	}

	public void DisablePlayer(float deathCount){
		playerAnimations.CrossFade("Death"+rand.Next(1,3));
		player.state = deathCount>= 3 ? "gameover":"dead";
        if (player.state == "gameover")
            GameOverSequence();
        GetComponent<Rigidbody>().isKinematic = true;
		controller.CmdUpdateSpeed(0);
		controller.CmdUpdateLookSensitivity(0);
	}

	void RespawnSequence(){
        Debug.Log("begin respawning");
		Transform newSpawn = NetworkManager.singleton.GetStartPosition();
		transform.position = newSpawn.position;
        // Spawn Animation
        CmdSpawnRespawnAnimations(transform.position,transform.rotation);
        hb.CurrentHealth = hb.MaxHealth;
		// Spawn Bubble animation for 3 seconds
		DeathTimer = 0;
        player.state = "respawned";
		GetComponent<Rigidbody>().isKinematic = false;
		controller.CmdUpdateSpeed(30);
		controller.CmdUpdateLookSensitivity(3);
	}
    [Command]
    void CmdSpawnRespawnAnimations(Vector3 position, Quaternion rotation)
    {
        GameObject respawnPillar = (GameObject)Instantiate(Resources.Load("Spells/Respawnpillar", typeof(GameObject)) as GameObject, position,rotation);
        NetworkServer.Spawn(respawnPillar);
        GameObject respawnShield = (GameObject)Instantiate(Resources.Load("Spells/Respawnshield", typeof(GameObject)) as GameObject, position, rotation);
        NetworkServer.Spawn(respawnShield);
        RpcInstantiateRespawnAnimations(respawnPillar, respawnShield);
    }

    [ClientRpc]
    void RpcInstantiateRespawnAnimations(GameObject respawnPillar, GameObject respawnShield)
    {
        respawnPillar.transform.parent = gameObject.transform;
        respawnShield.transform.parent = gameObject.transform;
        respawnShield.transform.Rotate(-90, 0, 0);
    }

    void GameOverSequence(){
		Debug.Log ("game over");
		GameObject.FindWithTag("GameOverTint").GetComponent<GameObject>().SetActive (true);
		player.SpectatorMode();
		GameManager.DeregisterPlayer(transform.name);
        //Add Notification that it is game over for the player.
		if (GameManager.GetAllPlayers ().Length <= 1)
			CmdEndGame ();
		else {
			
		}
	}	

	[Command]
	void CmdEndGame(){
		
	}

	[ClientRpc]
	void RpcShowEndGame(){
		if(!isLocalPlayer)
			return;
		player.state = "endgame";

	}

	void ExitGame(){
 		//NetworkManager.singleton.StopHost();

	}


	void Invulnerable(){
		if(InvulerabilityTimer > 5  && player.state == "respawned"){
			InvulerabilityTimer = 0;
            player.state = "alive";
        }
        else if(player.state == "respawned"){
			InvulerabilityTimer +=Time.deltaTime;
		}
		
	}
}

