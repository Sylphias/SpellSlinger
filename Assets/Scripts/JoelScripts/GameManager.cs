//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//
//public class GameManager : MonoBehaviour
//{
//
//    private const string PLAYER_PREFIXED_ID = "PlayerID ";
//    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
//
//    public static void RegisterPlayer(string networkID, Player player)
//    {
//        string playerID = PLAYER_PREFIXED_ID + networkID;
//        players.Add(playerID, player);
//        player.transform.name = playerID;
//    }
//
//    public static void DeregisterPlayer(string playerID)
//    {
//        players.Remove(playerID);
//    }
//
//    public static Player getPlayer(string playerID)
//    {
//        return players[playerID];
//    }
//
//    public static Player[] getAllPlayers()
//    {
//        return players.Values.ToArray();
//    }
//
//    private void OnGUI()
//    {
//        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
//        GUILayout.BeginVertical();
//        foreach (string playerID in players.Keys)
//        {
//            GUILayout.Label(playerID + " - " + players[playerID].transform.name);
//        }
//        GUILayout.EndVertical();
//        GUILayout.EndArea();
//    }
//}