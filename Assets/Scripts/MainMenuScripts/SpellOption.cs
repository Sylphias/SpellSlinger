using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellOption : MonoBehaviour {

	bool selected = false;
	int index;
	GameObject localData;
	PlayerData playerData;
	Button spellOptionButton;
	Image buttonImage;
	Text spellName;

	void Start () {
		localData = GameObject.Find ("Player Data");
		playerData = localData.GetComponent<PlayerData>();

		spellOptionButton = GetComponent<Button>();
		spellOptionButton.onClick.AddListener (ChangeSelection);
		buttonImage = GetComponent<Image>();
		spellName = GetComponentInChildren<Text>();
	}
		
	public void ChangeSelection () {
		if (selected == false && playerData.activeSpells.Count < 4) {
			buttonImage.color = Color.green;
			playerData.activeSpells.Add (spellName.text);
			selected = true;
		} else if (selected == true) {
			buttonImage.color = Color.grey;
			playerData.activeSpells.Remove (spellName.text);
			selected = false;
		} else {
			Debug.Log ("You can only choose a maximum of 4 spells");
		}
	}

}
