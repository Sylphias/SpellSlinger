using System.Collections;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardItem;
    [SerializeField]
    Transform playerScoreboardList;

    void OnEnable()
    {
        Player[] players = GameManager.GetAllPlayers();
        
        foreach(Player player in players)
        {
            GameObject itemGo = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
            PlayerScoreBoardItem item = itemGo.GetComponent<PlayerScoreBoardItem>();
            if (item != null)
            {
				item.scoreBoardSetup(player.transform.name, player.Kills, player.Deaths);
            }
        }
    }

    private void OnDisable()
    {
        foreach(Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}
