using System;
using System.Collections;
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
	private GameObject gameOverOverlay;
	public GameObject endGamePrefab;
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
		gameOverOverlay = GameObject.FindWithTag ("GameOverTint");
		gameOverOverlay.SetActive (false);
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
            case "gameover":
                GameOverSequence();
                break;
            default:
                Invulnerable();
                break;
        }
    }
		
	public void DisablePlayer(){
		if (!isLocalPlayer)return;
		player.state = player.Deaths >= 3 ? "gameover":"dead";
        player.Deaths++;
		player.BuffList = new List<IBuffable> ();
		playerAnimations.CrossFade("Death1");
		CmdSyncDeathAnimation ();
        if (player.state == "gameover")
            GameOverSequence();
        GetComponent<Rigidbody>().isKinematic = true;
		controller.CmdUpdateSpeed(0);
		controller.CmdUpdateLookSensitivity(0);
	}

	[Command]
	void CmdSyncDeathAnimation(){
		RpcClientDeathAnimation ();
		PlayDeathAnimation ();	
	}

	[ClientRpc]
	void RpcClientDeathAnimation(){
		PlayDeathAnimation ();	
	}

	void PlayDeathAnimation(){
		if(playerAnimations ==null)
			playerAnimations = GetComponent<Animation>();
		playerAnimations.CrossFade("Death1");
	}

	void RespawnSequence(){
        Debug.Log("begin respawning");
		Transform newSpawn = NetworkManager.singleton.GetStartPosition();
		transform.position = newSpawn.position;
        // Spawn Animation
		hb.CmdSetToFullHealth ();
		// Spawn Bubble animation for 3 seconds
		DeathTimer = 0;
        player.state = "respawned";
		GetComponent<Rigidbody>().isKinematic = false;
		controller.CmdUpdateSpeed(10);
		controller.CmdUpdateLookSensitivity(1);
		if(isLocalPlayer){
			CmdSpawnRespawnAnimations(newSpawn.position,transform.rotation);
		}
	}

    [Command]
    void CmdSpawnRespawnAnimations(Vector3 position, Quaternion rotation)
    {
        // Spawn the respawn animation. on non-player 
		GameObject respawnPillar = (GameObject)Instantiate(Resources.Load("Spells/Respawnpillar", typeof(GameObject)) as GameObject, Vector3.zero,rotation);
        NetworkServer.Spawn(respawnPillar);
		GameObject respawnShield = (GameObject)Instantiate(Resources.Load("Spells/Respawnshield", typeof(GameObject)) as GameObject, Vector3.zero, rotation);
		NetworkServer.Spawn(respawnShield);
		RpcInstantiateRespawnAnimations(respawnPillar, respawnShield, position);
    }

    [ClientRpc]
	void RpcInstantiateRespawnAnimations(GameObject respawnPillar, GameObject respawnShield,Vector3 position)
    {
        respawnPillar.transform.parent = gameObject.transform;
        respawnShield.transform.parent = gameObject.transform;
		respawnPillar.GetComponent<RespawnGameObjects> ().player = gameObject;
		respawnShield.GetComponent<RespawnGameObjects> ().player = gameObject;
		respawnShield.transform.Rotate(-90, 0, 0);
    }


    void GameOverSequence(){
		player.state = "spectating";
		gameOverOverlay.SetActive (true);
		player.SpectatorMode();
		CmdReducePlayerCount ();

		
	}	
	[Command]
	void CmdReducePlayerCount(){
		GameManager.KillPlayer();
		if (GameManager.GetNumberOfPlayersAlive() <= 1)
			EndGame ();
	}

	void EndGame(){
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
			player.GetComponent<PlayerDeath>().RpcShowEndGame ();
		}
	}

	[ClientRpc]
	void RpcShowEndGame(){
		if (!isLocalPlayer)
			return;
		ShowEndGame ();
	}

	void ShowEndGame(){
		Debug.Log ("Show end PASS for " + gameObject.transform.name);
		GameObject endSplash;
		string message;
		Debug.Log (player.state);
		if (player.state == "alive"){
			gameOverOverlay.SetActive (true);
			message = "You Win!";
			Debug.Log ("win here");
		} else {
			message = "You Lose!";
			Debug.Log ("lose here");
		}
		player.state = "endgameover";
		endSplash = (GameObject)Instantiate (endGamePrefab);
		endSplash.GetComponent<Text> ().text = message;
		endSplash.transform.SetParent (gameOverOverlay.GetComponent<RectTransform>(), false);
		

		StartCoroutine(GoBackToLobby());

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

	IEnumerator GoBackToLobby(){
		Debug.Log ("Going back to lobby");
		yield return new WaitForSecondsRealtime (5);

		//last resort
		GameObject.FindGameObjectWithTag ("LobbyManager").GetComponent<LobbyManager> ().DropMatch ();

		//GameObject.FindGameObjectWithTag ("LobbyManager").GetComponent<LobbyManager> ().SendReturnToLobby ();
		Destroy (this);
	}
}

