using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

	PlayerData playerData;
	List<string> activeSpells;

	static Color[] colors = new Color[]{Color.white, Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow}; 

	[SyncVar(hook = "OnChangeColor")]
	int currentColorIndex = 0; 

	[SyncVar(hook = "OnChangeName")]
	string userName;

	public Text nameLabel;

	public Button colorButton;
	public Button readyButton;
	public Button kickButton;
	public Button startButton;

	RectTransform playerListParent;

	void Start(){
		DontDestroyOnLoad (transform.gameObject);
		if (isLocalPlayer) {
			Debug.Log ("I am a Local Player at Start");
			SetupLocalPlayer ();
		} else{
			Debug.Log ("I am Not a Local Player at Start");
			SetupOtherPlayer ();
		}
	}
				
	public override void OnClientEnterLobby(){
		Debug.Log ("OnClientEnterLobby is called");
		SetupLobby ();
		Debug.Log ("Taking slot" + slot);
	}


	void SetupLocalPlayer(){
		Debug.Log ("Setting up local player");
		playerData = GameObject.Find ("Player Data").GetComponent<PlayerData> ();
		activeSpells = playerData.activeSpells;
		colorButton.onClick.AddListener (ChangeColor);
		readyButton.onClick.AddListener (Ready);
		string tempName = playerData.userName;
		this.name = userName;
		CmdSetName (tempName);
		gameObject.transform.SetParent (playerListParent, false);
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
		gameObject.transform.SetParent (playerListParent, false);
	}

	void SetupLobby(){
		GameObject.Find ("Room Name Label").GetComponent<Text>().text = "Placeholder";
		playerListParent = GameObject.Find ("Player List").GetComponent<RectTransform>();
	}

	[Command]
	void CmdSetName(string name){
		Debug.Log ("CmdSetName is called");
		userName = name;
	}
		
	void ChangeColor(){
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
		gameObject.transform.parent = null;
		Debug.Log ("Ready is pressed");
		if (readyToBegin) {
			SendNotReadyToBeginMessage ();
		} else {
			SendReadyToBeginMessage ();
		}
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
