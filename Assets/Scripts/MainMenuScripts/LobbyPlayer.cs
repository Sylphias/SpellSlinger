using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

	PlayerData playerData;
	List<string> activeSpells;

	static Color[] colors = new Color[]{Color.white, Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow}; 

	[SyncVar(hook = "OnStartGame")] 
	public bool startGame = false;

	[SyncVar(hook = "OnChangeColor")]
	int currentColorIndex = 0; 

	[SyncVar(hook = "OnChangeName")]
	string userName;

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

			if (isLocalPlayer) {
				Debug.Log ("I am a Local Player at Start");
				SetupLocalPlayer ();
			} else {
				Debug.Log ("I am Not a Local Player at Start");
				SetupOtherPlayer ();
			}
		} else {
			Debug.Log ("I am at the game scene");
		}
	}
				
	void SetupLocalPlayer(){
		Debug.Log ("Setting up local player");
		playerData = GameObject.Find ("Player Data").GetComponent<PlayerData> ();
		activeSpells = playerData.activeSpells;
		colorButton.onClick.AddListener (delegate {CmdChangeColor();});
		readyButton.onClick.AddListener (delegate {Ready();});
		string tempName = playerData.userName;
		this.name = userName;
		CmdSetName (tempName);
		//gameObject.transform.SetParent (playerListParent, false);
	}

	void SetupOtherPlayer(){
		Debug.Log ("Setting up other player");
		if (hasAuthority) {
			kickButton.onClick.AddListener (Kicked);
		} else {
			kickButton.gameObject.SetActive (false);
		}
		nameLabel.text = userName;
		colorButton.GetComponent<Image> ().color = colors[currentColorIndex];
		readyButton.interactable = false;
		//gameObject.transform.SetParent (playerListParent, false);
	}
				
	void SetupLobby(){
		//GameObject.Find ("Room Name Label").GetComponent<Text>().text = "Placeholder";
		//startButton = GameObject.Find ("Start Button").GetComponent<Button>();
		playerListParent = GameObject.Find ("Player List").GetComponent<RectTransform>();
	}

	[Command]
	void CmdSetName(string name){
		Debug.Log ("CmdSetName is called");
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

	public void Ready(){
		Debug.Log ("Ready is pressed");
		if (readyToBegin) {
			SendNotReadyToBeginMessage ();
		} else {
			SendReadyToBeginMessage ();
		}
	}
		
	public void OnStartGame(bool state){
		Debug.Log (userName + " is ready");
		if (state)
			gameObject.transform.parent = null;
		else
			gameObject.transform.SetParent (playerListParent, false);
	}

	/*
	public override void OnClientEnterLobby(){
		Debug.Log ("On Client Enter Lobby");
		if (startGame) {
			Debug.Log ("Just ended a game, now back to lobby");
			startGame = false;
			//GameObject.Find ("Main Menu Panel").SetActive (false);
			//GameObject.Find ("Lobby Panel").SetActive (true);
		} 
	}
	*/

	public override void OnClientExitLobby(){
		Debug.Log ("On Client Exit Lobby");
	}

	public void Kicked(){
		Debug.Log ("Kicked" );
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
