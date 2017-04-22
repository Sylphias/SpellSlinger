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
		if (SceneManager.GetActiveScene ().name.Equals ("Menu")) {
			SetupMenu ();
		}
	}		
		
	public void ToHost(){
		if(Check ()){
			menuPanel.SetActive(false);
			hostPanel.SetActive(true);
		}
	}
						
	public void ListRoom(){
		if (Check ()) 
			ListMatches ();
	}

	public void Matchmake(){
		if (Check ())
			RandomMatch ();
	}
						
	bool Check(){
		if (!playerData.userName.Equals("") && playerData.activeSpells.Count >= 4) {
			return true;
		} else {
			PopMessage ("Please load up your character from Edit Character button");
			return false;
		}
	}

	//NETWORK MATCHMAKER SECTION

	//Create Match
	public void HostMatch(){
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
		if (success) { 
			StartHost (matchInfo);
			NetworkServer.Listen (matchInfo, 9999);
			currentNetworkId = (System.UInt64)matchInfo.networkId;
		} else
			PopMessage ("Fail to create room");
	}

	public override void OnStartHost(){
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
		foreach (Transform child in roomListParent.transform) {
			GameObject.Destroy (child.gameObject);
		}

		foreach (MatchInfoSnapshot match in matches) {
			string matchLabel = match.name + " [ " + match.currentSize + "/" + match.maxSize + " ]";
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
				NetworkManager.singleton.matchMaker.JoinMatch (matches [matches.Count - 1].networkId, "", "", "", 0, 0, OnMatchJoined); 
			} else {
				PopMessage ("Sorry! There is no available room :C");
			}
		}
	}

	public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo){
		if (success) {
			StartClient (matchInfo);
			currentNetworkId = (System.UInt64)matchInfo.networkId;
			menuPanel.SetActive(false);
			joinPanel.SetActive(false);
			lobbyPanel.SetActive(true);
		}
	}
		
	//Drop Match
	public void DropMatch(){
		NetworkManager.singleton.matchMaker.DropConnection (matchInfo.networkId, matchInfo.nodeId, 0, OnMatchDropConnection);
	}

	public void OnMatchDropConnection(bool success, string extendedInfo){
		if (success) {
			if (host) {
				NetworkManager.singleton.matchMaker.DestroyMatch (matchInfo.networkId, 0, OnDestroyMatch);
				NetworkManager.singleton.StopHost ();
				NetworkManager.singleton.StopClient ();
				SetupMenu();
			} else {
				NetworkManager.singleton.StopClient ();
			}
		}
	}
		
	public override void OnStopClient(){
		base.OnStopClient ();
		lobbyPanel.SetActive(false);
		menuPanel.SetActive (true);
		background.SetActive (true);
	}
				
	//Transition from Lobby Player to Game Player!
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		LobbyPlayer source = lobbyPlayer.GetComponent<LobbyPlayer>();
		PlayerSpellcasting playerSpell = gamePlayer.GetComponent<PlayerSpellcasting>();
		if (source != null && playerSpell != null ) {
			for (int i = 0; i < source.ActiveSpells.Count; i++)
				playerSpell.spellList.Add(source.ActiveSpells[i]);
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
		playerData = GameObject.Find ("Player Data").GetComponent<PlayerData> ();
		background.SetActive (true);
		toHostButton.onClick.AddListener (() => ToHost());
		toJoinButton.onClick.AddListener (() => ListRoom ()); 
		matchmakeButton.onClick.AddListener (() => Matchmake ());
		hostButton.onClick.AddListener (() => HostMatch());
		joinButton.onClick.AddListener(() => JoinMatch());
		quitButton.onClick.AddListener (() => DropMatch ());
		popupOkButton.onClick.AddListener (() => DeactivatePopup());
	}
		
}
