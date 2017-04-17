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
	public Button toJoinButton;
	public Button joinButton;
	public Button matchmakeButton;
	public Button quitButton;
	public Button popupOkButton;
	public Button stopButton;

	public GameObject background;
	public GameObject menuPanel;
	public GameObject hostPanel;
	public GameObject joinPanel;
	public GameObject lobbyPanel;
	public GameObject popupPanel;

	public PlayerData playerData;

	public RectTransform playerListParent;

	ulong selectedNetworkId;
	ulong currentNetworkId;

	public RectTransform roomListParent;
	public Button roomOptionPrefab;

	bool host = false;

	void Start(){
		LogFilter.currentLogLevel = LogFilter.Debug;
		Debug.Log ("Network Lobby Manager is running");
		if (SceneManager.GetActiveScene ().name.Equals ("Menu")) {
			SetupMenu ();
		} else {
			Debug.Log ("I am at the game scene");

		}
	}
		
		
	public void ToHost(){
		Debug.Log ("ToHost button is pressed");
		if(Check ()){
			menuPanel.SetActive(false);
			hostPanel.SetActive(true);
		}
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
	public void HostMatch(){
		Debug.Log ("HostMatch");
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

		if (NetworkManager.singleton.matchMaker == null) {
			NetworkManager.singleton.StartMatchMaker();
		}

		NetworkManager.singleton.matchMaker.CreateMatch (roomName.text, max, true, "", "", "", 0, 0, OnMatchCreate);
			
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


	//List Matches
	public void ListMatches(){
		if (NetworkManager.singleton.matchMaker == null) {
			NetworkManager.singleton.StartMatchMaker();
		}
		NetworkManager.singleton.matchMaker.ListMatches (0, 5, "", true, 0, 0, OnMatchList);
	}

	public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches){
		if (success) {
			NetworkManager.singleton.matches = matches;
			if (matches.Count >= 1) {
				Debug.Log ("OnMatchList success: Populating list");
				PopulateList (matches);
				menuPanel.SetActive(false);
				joinPanel.SetActive(true);
			} else {
				PopMessage ("Sorry! There is no available room :C");
			}

		} else {
			Debug.LogError ("OnMatchList fail: Unable to list the matches");
		}
	}
		
	public void PopulateList (List<MatchInfoSnapshot> matches){
		Debug.Log ("Populating the list of matches");

		foreach (Transform child in roomListParent.transform) {
			GameObject.Destroy (child.gameObject);
		}

		foreach (MatchInfoSnapshot match in matches) {
			string matchLabel = match.name + " [ " + match.currentSize + "/" + match.maxSize + " ]";
			Debug.Log (matchLabel);
			Button tempButton = (Button)Instantiate (roomOptionPrefab);
			tempButton.GetComponentInChildren<Text>().text = matchLabel;
			tempButton.onClick.RemoveAllListeners ();
			tempButton.onClick.AddListener(() => {SetSelected(match.networkId);}); 

			tempButton.transform.SetParent (roomListParent, false);
		}
	}

	//Select Match
	public void SetSelected (NetworkID networkId){
		selectedNetworkId = (System.UInt64)networkId;
	}
		
	//Join Match
	public void JoinMatch(){
		Debug.Log ("JoinMatch");
		NetworkManager.singleton.matchMaker.JoinMatch ((NetworkID)selectedNetworkId, "", "", "", 0, 0, OnMatchJoined);
	}

	//Random Match
	public void RandomMatch(){
		if (NetworkManager.singleton.matchMaker == null) {
			NetworkManager.singleton.StartMatchMaker();
		}
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
			currentNetworkId = (System.UInt64)matchInfo.networkId;
			menuPanel.SetActive(false);
			joinPanel.SetActive(false);
			lobbyPanel.SetActive(true);
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
				NetworkManager.singleton.StopHost ();
			} else {
				NetworkManager.singleton.StopClient ();
			}
		}
	}
		
	public override void OnStopClient(){
		base.OnStopClient ();
		Debug.Log ("Stopped Client");
		lobbyPanel.SetActive(false);
		menuPanel.SetActive (true);
	}
				
	//Transition from Lobby Player to Game Player!
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		Debug.Log ("Scene Loaded");

		LobbyPlayer source = lobbyPlayer.GetComponent<LobbyPlayer>();
		Player player = gamePlayer.GetComponent<Player>();
		PlayerSpellcasting playerSpell = gamePlayer.GetComponent<PlayerSpellcasting>();


		if (source != null && player != null ) {
			
			Debug.Log ("Should be able to do transfer");
			player.playerName = source.UserName;
			playerSpell.spellList = new string[4];
			source.ActiveSpells.CopyTo (playerSpell.spellList);
		} else {
			Debug.Log ("Null players detected");
		}

		return true;
	}


	public override void OnClientSceneChanged (NetworkConnection conn){
		string loadedSceneName = SceneManager.GetSceneAt (0).name;
		if (loadedSceneName == "Menu") {
			background.SetActive (true);
			lobbyPanel.SetActive (true);
			//stopButton.gameObject.SetActive (false);
		} else {
			background.SetActive (false);
			lobbyPanel.SetActive (false);
			//stopButton.gameObject.SetActive (true);
		}
		base.OnClientSceneChanged (conn);
	}

	public override void OnLobbyServerPlayersReady ()
	{
		Debug.Log ("OnLobbyServerPlayerReady");
		playerListParent = GameObject.Find ("Player List").GetComponent<RectTransform> ();
		LobbyPlayer[] playerList = playerListParent.GetComponentsInChildren<LobbyPlayer> ();
		foreach (LobbyPlayer player in playerList) {
			player.startGame = true;
		}
		base.OnLobbyServerPlayersReady ();
	}

	public void StartMatch(){
		playerListParent = GameObject.Find ("Player List").GetComponent<RectTransform> ();
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


	public void SetupMenu (){
		Debug.Log ("Setting up menu");

		playerData = GameObject.Find ("Player Data").GetComponent<PlayerData> ();

		toHostButton.onClick.AddListener (() => ToHost());
		toJoinButton.onClick.AddListener (() => ListRoom ()); 
		matchmakeButton.onClick.AddListener (() => Matchmake ());
		hostButton.onClick.AddListener (() => HostMatch());
		joinButton.onClick.AddListener(() => JoinMatch());
		quitButton.onClick.AddListener (() => DropMatch ());
		popupOkButton.onClick.AddListener (() => DeactivatePopup());

		/*
		menuPanel = GameObject.Find ("Main Menu Panel");
		toHostButton = GameObject.Find ("To Host Button").GetComponent<Button> ();
		toJoinButton = GameObject.Find ("To Join Button").GetComponent<Button> ();
		matchmakeButton = GameObject.Find ("Matchmaker Button").GetComponent<Button> ();
		toHostButton.onClick.AddListener (() => ToHost());
		toJoinButton.onClick.AddListener (() => ListRoom ()); 
		matchmakeButton.onClick.AddListener (() => Matchmake ());

		hostPanel = GameObject.Find ("Host Panel");
		roomName = GameObject.Find ("Room Name Field").GetComponent<InputField> ();
		roomSize = GameObject.Find ("Max Player Field").GetComponent<InputField> ();
		hostButton = GameObject.Find ("Host Game Button").GetComponent<Button> ();
		hostButton.onClick.AddListener (() => HostMatch());

		joinPanel = GameObject.Find ("Join Panel");
		joinButton = GameObject.Find ("Join Game Button").GetComponent<Button> ();
		joinButton.onClick.AddListener(() => JoinMatch()); 

		lobbyPanel = GameObject.Find ("Lobby Panel");
		quitButton = GameObject.Find ("Quit Button").GetComponent<Button> ();
		quitButton.onClick.AddListener (() => DropMatch ());

		popupPanel = GameObject.Find ("PopUp Panel");
		popupOkButton = GameObject.Find ("Ok Button").GetComponent<Button> ();
		popupOkButton.onClick.AddListener (() => DeactivatePopup());

		hostPanel.SetActive(false);
		joinPanel.SetActive(false);
		lobbyPanel.SetActive(false);
		popupPanel.SetActive(false);
		*/
	}
}
