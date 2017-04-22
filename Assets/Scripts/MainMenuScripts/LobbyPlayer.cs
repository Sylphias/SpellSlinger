using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

	PlayerData playerData;

	public List<string> activeSpells;

	static Color[] colors = new Color[]{Color.white, Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow}; 

	[SyncVar(hook = "OnStartGame")] 
	public bool startGame = false;

	[SyncVar(hook = "OnChangeColor")]
	int currentColorIndex = 0; 

	[SyncVar(hook = "OnChangeName")]
	public string userName;

	public Text nameLabel;

	public Button colorButton;
	public Button readyButton;
	public Button kickButton;

	Button startButton;
	RectTransform playerListParent;

	void Start(){
		
		DontDestroyOnLoad (transform.gameObject);

		if (SceneManager.GetActiveScene ().name.Equals ("Menu")) {

			SetupLobby ();

			gameObject.transform.SetParent (playerListParent, false);

			if (hasAuthority) {
				SetupLocalPlayer ();
			} else {
				SetupOtherPlayer ();
			}
		}
	}
				
	void SetupLocalPlayer(){
		playerData = GameObject.Find ("Player Data").GetComponent<PlayerData> ();
		colorButton.onClick.AddListener (delegate {CmdChangeColor();});
		readyButton.onClick.AddListener (delegate {Ready();});
		string tempName = playerData.userName;
		this.name = userName;
		CmdSetName (tempName);
		activeSpells = playerData.activeSpells;
		foreach (string spell in activeSpells) {
			CmdTransferSpells (spell);
		}
	}
		
	[Command]
	void CmdTransferSpells(string spell){
		RpcTransferSpells (spell);
	}

	[ClientRpc]
	void RpcTransferSpells (string spell){
		if (activeSpells.Count >= 4)
			return;
		activeSpells.Add(spell);
	}

	void SetupOtherPlayer(){
		if (hasAuthority) {
			kickButton.onClick.AddListener (Kicked);
		} else {
			kickButton.gameObject.SetActive (false);
		}
		nameLabel.text = userName;
		colorButton.GetComponent<Image> ().color = colors[currentColorIndex];
		readyButton.interactable = false;
	}
				
	void SetupLobby(){
		playerListParent = GameObject.Find ("Player List").GetComponent<RectTransform>();
	}

	void Kicked(){
	}

	[Command]
	void CmdSetName(string name){
		userName = name;
	}
		
	[Command]
	void CmdChangeColor(){
		if (currentColorIndex == 6) {
			currentColorIndex = 0;
		} else {
			currentColorIndex++;
		}
	}

	void OnChangeColor(int newColorIndex){
		colorButton.GetComponent<Image> ().color = colors [newColorIndex];;
	}

	void OnChangeName(string newName){
		nameLabel.text = newName;
	}

	[Command]
	void CmdReady(bool ready){
		RpcReady (ready);
	}

	[ClientRpc]
	void RpcReady(bool ready){
		if (ready) {
			readyButton.GetComponent<Image>().color = new Color32(0x98, 0x98,0x98, 0xFF);
			readyButton.GetComponentInChildren<Text> ().text = "Ready";
		} else {
			readyButton.GetComponent<Image>().color = new Color32(0xDA, 0xDA, 0xDA, 0xFF);
			readyButton.GetComponentInChildren<Text> ().text = "Not Ready";
		}
	}
		
	void Ready(){
		if (readyToBegin) {
			SendNotReadyToBeginMessage ();
			CmdReady (false);
		} else {
			SendReadyToBeginMessage ();
			CmdReady (true);
		}

	}
		
	public void OnStartGame(bool state){
		if (state)
			gameObject.transform.parent = null;
	}
		
	public override void OnClientEnterLobby(){
		
		if (startGame) {
			readyButton.GetComponent<Image>().color = new Color32(0xDA, 0xDA, 0xDA, 0xFF);
			SetupLobby ();
			startGame = false;
		} 
	}

	public List<string> ActiveSpells{
		get{
			return activeSpells;
		}
	}

	public string UserName{
		get{
			return userName;
		}
	}

	public int ColorIndex{
		get{
			return currentColorIndex;
		}
	}
}
