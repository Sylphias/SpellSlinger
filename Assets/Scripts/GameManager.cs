using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager:MonoBehaviour
{

	public static GameManager instance;

	#region Player Specific info

	private const string PLAYER_ID_PREFIX = "Player ";

	private static int PlayersAlive = 0;

	private static Dictionary<string, Player> players = new Dictionary<string, Player>();

	public static void RegisterPlayer (string _netID, Player _player)
	{
		string _playerID = PLAYER_ID_PREFIX + _netID;
		players.Add(_playerID, _player);
		_player.transform.name = _playerID;
		PlayersAlive++;
	}

	public static void DeregisterPlayer (string _playerID)
	{
		PlayersAlive--;
		players.Remove(_playerID);
	}

	public static int GetNumberOfPlayersAlive(){
		return PlayersAlive;
	}

	public static void KillPlayer(){
		PlayersAlive--;
	}
	

	public static Player GetPlayer (string _playerID)
	{
		return players[_playerID];
	}


	public static Player[] GetAllPlayers ()
	{
		Player[] playerList = new Player[players.Count];
		players.Values.CopyTo(playerList,0);
		return playerList;
	}
	#endregion
}

