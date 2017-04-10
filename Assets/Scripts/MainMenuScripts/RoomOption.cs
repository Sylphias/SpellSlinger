using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Net;
using System.Net.Sockets;

public class RoomOption : MonoBehaviour {

	ulong selectedNetworkId;

	public RectTransform roomListParent;
	public Button roomOptionPrefab;

	public Button joinButton;

	LobbyManager lobbyManager;

	void Start(){
		lobbyManager = GetComponent<LobbyManager>();
		joinButton.onClick.RemoveAllListeners ();
		joinButton.onClick.AddListener(() => JoinMatch()); 
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
		
	public void SetSelected (NetworkID networkId){
		selectedNetworkId = (System.UInt64)networkId;
	}

	public void JoinMatch(){
		Debug.Log ("JoinMatch");
		NetworkManager.singleton.matchMaker.JoinMatch ((NetworkID)selectedNetworkId, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
	}
		
}
