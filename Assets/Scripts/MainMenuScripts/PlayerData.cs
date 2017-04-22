using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {
	
	public List<string> activeSpells = new List<string>();
	public string userName = "";

	void Start(){
		if (!userName.Equals ("")) {
			Debug.Log ("Getting prefs");
			userName = PlayerPrefs.GetString ("username");
			if (activeSpells.Count == 0) {
				activeSpells.Clear();
				activeSpells.Add (PlayerPrefs.GetString ("spell1"));
				activeSpells.Add (PlayerPrefs.GetString ("spell2"));
				activeSpells.Add (PlayerPrefs.GetString ("spell3"));
				activeSpells.Add (PlayerPrefs.GetString ("spell4"));
			}
		} else {
			Debug.Log ("No saved prefs");
		}
    }

    public void saveUsername(string newUsername){
		userName = newUsername;
		PlayerPrefs.SetString ("username", newUsername);
	}

	public void saveSpells(){
		int x = 1;
		foreach (string spell in activeSpells) {
			string label = "spell" + x;
			PlayerPrefs.SetString (label, spell);
			x++;
		}
	}
}
