using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    Player player;
    int lastKill = 0;
    int lastDeath = 0;

	void Start ()
    {
        player = GetComponent<Player>();
        //StartCoroutine(loopToUpdateScore());
	}

    //IEnumerator loopToUpdateScore()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(10f);
    //        getSyn();
    //    }
    //}

    //void getSync()
    //{

    //}
	
    void onDataRecieved(string data)
    {
        if (player.Kills <= lastKill && player.Deaths <= lastDeath) return;
        int sinceLastKill = player.Kills - lastKill;
        int sinceLastDeath = player.Deaths - lastDeath;
        int Kills = DataTranslator.DataToKills(data);
        int Deaths = DataTranslator.DataToDeaths(data);
        int updateKill = sinceLastKill + Kills;
        int updateDeath = sinceLastDeath + Deaths;
        string updatedData = DataTranslator.ValuesToData(updateKill, updateDeath);
        Debug.Log("Sync: " + updatedData);
        lastKill = player.Kills;
        lastDeath = player.Deaths;

        // need to send the updatedData back to user not implemented
    }
}
