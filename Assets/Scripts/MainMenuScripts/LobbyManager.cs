using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Net;
using System.Net.Sockets;

public class LobbyManager: NetworkLobbyManager {

	public InputField roomName;
	public InputField roomSize;

	public Button toHostButton;
	public Button hostButton;
	public Button matchmakeButton;
	public Button hostRoomListButton;
	public Button backButton;
	public Button popupOkButton;

	public GameObject hostPanel;
	public GameObject joinPanel;
	public GameObject menuPanel;
	public GameObject lobbyPanel;
	public GameObject popupPanel;

	public PlayerData playerData;

	public RectTransform playerListParent;

	bool host = false;
	ulong currentNetworkId;

	void Start(){
		Debug.Log ("Network Lobby Manager is running");
		Setup ();
	}

	public void Setup(){
		Debug.Log ("Setting up");
		NetworkManager.singleton.StartMatchMaker();
		playerData = GameObject.Find ("Player Data").GetComponent<PlayerData> ();
		hostButton.onClick.AddListener (() => Host());
		hostRoomListButton.onClick.AddListener (() => ListRoom ()); 
		matchmakeButton.onClick.AddListener (() => Matchmake());
		backButton.onClick.AddListener (() => QuitRoom ()); 
		popupOkButton.onClick.AddListener (DeactivatePopup);
	}
				
	public void Host(){
		CreateMatch ();
	}
		
	public void ListRoom(){
		Debug.Log ("ListRoom button is pressed");
		if (Check ()) 
			ListMatches ();
	}

	public void Matchmake(){
		Debug.Log ("Matchmake button is pressed");
		if (Check ())
			RandomMatch ();
	}
		
	public void QuitRoom(){
		DropMatch ();
	}
				
	bool Check(){
		if (!playerData.userName.Equals("") && playerData.activeSpells.Count == 4) {
			return true;
		} else {
			PopMessage ("Please load up your character from Edit Character button");
			return false;
		}
	}

	//NETWORK MATCHMAKER SECTION

	//Create Match
	public void CreateMatch(){
		Debug.Log ("CreateMatch");
		if(roomName.text.Equals("")){
			PopMessage ("Please enter a room name");
			return;
		}
		uint max;
		if (!roomSize.text.Equals ("")) {
			max = System.UInt32.Parse(roomSize.text);
			if (max > 10) {
				PopMessage ("Maximum players in a room is 10");
				return;
			}
		} else {
			max = 4;
		}
		NetworkManager.singleton.matchMaker.CreateMatch (roomName.text, max , true, "", "", "", 0, 0, OnMatchCreate);
	}

	public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo){
		if (success){
			Debug.Log ("Room is successfully created"); 
			StartHost (matchInfo);
			NetworkServer.Listen (matchInfo, 9999);
			currentNetworkId = (System.UInt64)matchInfo.networkId;

		}
		else
			Debug.LogError ("Room failed to be created");
	}

	public override void OnStartHost(){
		Debug.Log("OnStartHost is called");
		host = true;
		hostPanel.SetActive (false);
		lobbyPanel.SetActive (true);
	}


	//List Match
	public void ListMatches(){
		NetworkManager.singleton.matchMaker.ListMatches (0, 5, "", true, 0, 0, OnMatchList);
	}

	public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches){
		if (success) {
			NetworkManager.singleton.matches = matches;
			if (matches.Count >= 1) {
				Debug.Log ("OnMatchList success: Populating list");
				GetComponent<RoomOption> ().PopulateList (matches);
				menuPanel.SetActive (false);
				joinPanel.SetActive (true); 

			} else {
				PopMessage ("Sorry! There is no available room :C");
			}

		} else {
			Debug.LogError ("OnMatchList fail: Unable to list the matches");
		}
	}
		


	//Random Match
	public void RandomMatch(){
		NetworkManager.singleton.matchMaker.ListMatches (0, 5, "", true, 0, 0, OnRandomMatchList);
	}

	public void OnRandomMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches){
		if (success) {
			if (matches.Count >= 1) {
				Debug.Log ("There is a match for RandomMatch");
				Debug.Log (matches.Count);
				NetworkManager.singleton.matchMaker.JoinMatch (matches [matches.Count - 1].networkId, "", "", "", 0, 0, OnMatchJoined); 
			} else {
				PopMessage ("Sorry! There is no available room :C");
			}
		} else {
			Debug.LogError ("Unable to RandomMatch");
		}
	}

	public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo){
		if (success) {
			Debug.Log ("OnMatchJoined success");
			StartClient (matchInfo);
			joinPanel.SetActive (false);
			lobbyPanel.SetActive (true);
			currentNetworkId = (System.UInt64)matchInfo.networkId;
		} else {
			Debug.Log ("OnMatchJoined fail");
		}
	}
		
	//Drop Match
	public void DropMatch(){
		NetworkManager.singleton.matchMaker.DropConnection (matchInfo.networkId, matchInfo.nodeId, 0, OnMatchDropConnection);
	}

	public void OnMatchDropConnection(bool success, string extendedInfo){
		if (success) {
			Debug.Log ("Connection is dropped");
			if (host) {
				Debug.Log ("I am a host");
				StopMatchMaker ();
				StopHost ();
			} else {
				StopMatchMaker ();
				StopClient ();
			}
		}
	}

	public override void OnStopHost(){
		Debug.Log ("Stopped Host");
		lobbyPanel.SetActive (false);
		menuPanel.SetActive (true);
		Setup ();
		//SceneManager.LoadScene ("Menu");
	}

	public override void OnStopClient(){
		base.OnStopClient ();
		Debug.Log ("Stopped Client");
		lobbyPanel.SetActive (false);
		menuPanel.SetActive (true);
		Setup ();
		//SceneManager.LoadScene ("Menu");
	}
				
	// EDIT CODE HERE !!!!!!!!!!!!!!!!!!!!!!!!!!!!
	//Transition from Lobby Player to Game Player!
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		Debug.Log ("Scene Loaded");

		LobbyPlayer source = lobbyPlayer.GetComponent<LobbyPlayer>();
		Player player = gamePlayer.GetComponent<Player>();

		string tobepassed = source.UserName;

		Debug.Log (tobepassed);

		if (source != null && player != null ) {
			Debug.Log ("Should be able to do transfer");
			return true;
		} else {
			Debug.Log ("Null players detected for" + tobepassed);
			return false;
		}
	}
				
	public override void OnLobbyServerPlayersReady ()
	{
		Debug.Log ("OnLobbyServerPlayerReady");
		
		LobbyPlayer[] playerList = playerListParent.GetComponentsInChildren<LobbyPlayer> ();
		foreach (LobbyPlayer player in playerList) {
			player.startGame = true;
		}
		base.OnLobbyServerPlayersReady ();
	}
		
	void PopMessage(string message){
		popupPanel.gameObject.GetComponentInChildren<Text> ().text = message;
		popupPanel.SetActive (true);
	}

	void DeactivatePopup(){
		popupPanel.SetActive (false);
	}	
		
}
