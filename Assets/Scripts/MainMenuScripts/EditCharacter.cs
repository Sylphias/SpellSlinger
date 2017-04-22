using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditCharacter : MonoBehaviour {

	List<string> allSpells = new List<string> {"Iceball", "Fireball", "Stonefist", "Swiftness", "Earthwall", "Frostnova", "Firenova", "Blink", "Forcelightning"};

	GameObject localData;
	PlayerData playerData;

	public InputField tempUsername;

	public Button spellOptionPrefab;
	public Text activeSpellPrefab;
	public RectTransform spellOptionParent;
	public RectTransform activeSpellsParent;

	public Button saveEdit;
	public Button saveModify;
	public Button popupOkButton;

	public GameObject modifyPanel;
	public GameObject mainMenuPanel;
	public GameObject popupPanel;

	void Start () {
		localData = GameObject.Find ("Player Data");
		playerData = localData.GetComponent<PlayerData>();

		foreach (string spell in allSpells){
			Button tempButton = (Button)Instantiate (spellOptionPrefab);
			tempButton.transform.SetParent (spellOptionParent, false);
			tempButton.GetComponentInChildren<Text> ().text = spell;
		}

		saveEdit.onClick.AddListener (() => saveUsername());
		saveModify.onClick.AddListener (() => saveSpells());
		popupOkButton.onClick.AddListener (DeactivatePopup);
	}

	public void saveUsername () {
		if (tempUsername.text == "") {
			PopMessage ("Please enter a username");
		} else if (playerData.activeSpells.Count < 4) {
			PopMessage ("Please choose 4 spells by clicking Modify button");
		} else {
			playerData.saveUsername(tempUsername.text);
			gameObject.SetActive (false);
			mainMenuPanel.SetActive (true);
			//tempUsername.placeholder = tempUsername.text;
		}
	}

	public void saveSpells(){
		
		if (playerData.activeSpells.Count < 4) {
			PopMessage("You have to choose 4 spells");
		} else {
			playerData.saveSpells ();
			foreach (Transform child in activeSpellsParent.transform) {
				GameObject.Destroy (child.gameObject);
			}
			foreach (string spell in playerData.activeSpells){
				Text tempText = (Text)Instantiate (activeSpellPrefab);
				tempText.text = spell;
				tempText.transform.SetParent (activeSpellsParent, false);
			}
			modifyPanel.SetActive (false);
		}
	}

	void PopMessage(string message){
		popupPanel.gameObject.GetComponentInChildren<Text> ().text = message;
		popupPanel.SetActive (true);
	}

	void DeactivatePopup(){
		popupPanel.SetActive (false);
	}	
}
