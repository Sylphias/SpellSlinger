using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreBoardItem : MonoBehaviour {

    [SerializeField]
    Text usernameText;
    [SerializeField]
    Text killText;
    [SerializeField]
    Text deathText;

    public void scoreBoardSetup(string username, int kills, int deaths)
    {
        usernameText.text = username;
        killText.text = "Kills: " + kills;
        deathText.text = "Deaths: " + deaths;
    }
}
