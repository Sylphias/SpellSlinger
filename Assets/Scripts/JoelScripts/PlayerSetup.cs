using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;
    Camera sceneCamera;

    void Start()
    {
        //if (!isLocalPlayer)
        //{
        //    for (int i = 0; i < componentsToDisable.Length; i++)
        //    {
        //        componentsToDisable[i].enabled = false;
        //    }
        //}

        //else
        //{
        //    sceneCamera = Camera.main;
        //    if(sceneCamera != null)
        //    {
        //        sceneCamera.gameObject.SetActive(false);
        //    }
        //}
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string networkID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(networkID, player);
    }

    private void OnDisable()
    {
        //if (sceneCamera != null)
        //{
        //    sceneCamera.gameObject.SetActive(true);
        //}

        GameManager.DeregisterPlayer(transform.name);
    }

     [Command]
     void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
			player.transform.name= username;
            Debug.Log(username + " has joined the game.");
        }
    }
}
