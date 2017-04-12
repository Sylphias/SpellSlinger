using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour {
    public Text killCount;
    public Text deathCount;

    void Start()
    {

    }

    void onReceivedData(string data)
    {
        if (killCount == null || deathCount == null) return;
        killCount.text = DataTranslator.DataToKills(data).ToString() + " KILLS";
        deathCount.text = DataTranslator.DataToDeaths(data).ToString() + " DEATHS";
    }
}
