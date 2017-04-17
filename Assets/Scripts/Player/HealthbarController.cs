using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using Spells;

public class HealthbarController : NetworkBehaviour,IPlayer {

	public GameObject hbGameObj;
	public GameObject UIhealthbar;
    private RawImage miniHealthBar;

    private const float maxHealth =100;

	private Player player;
    private RectTransform healthBarFill;
    [SyncVar (hook ="OnChangeHealth")]
	float currHealth ;
    public float MaxHealth { get { return maxHealth; } }
    public float CurrentHealth {
		get{ return currHealth; }
		set{ currHealth = value; }
	}
	public override void OnStartLocalPlayer(){
        if (!isLocalPlayer) return;
        Debug.Log("Player Healthbar initialized");
        player = GetComponent<Player>();
        healthBarFill = GameObject.Find("HealthBarFill").GetComponent<RectTransform>();
        Init();
        base.OnStartLocalPlayer ();
	}

    public void Init()
    {
        miniHealthBar = transform.FindChild("HealthBarBg").FindChild("HPOverlay").GetComponentInChildren<RawImage>();
        currHealth = maxHealth;
    }
    //Not sure why Syncvar is not working
    public void OnChangeHealth(float health)
	{
		currHealth = health;
		Debug.Log (gameObject.transform.name + " Change health");
		if (isLocalPlayer)
			healthBarFill.localScale = new Vector3 (1f, currHealth / maxHealth, 1f);
		if(miniHealthBar == null)
        	miniHealthBar = transform.FindChild("HealthBarBg").FindChild("HPOverlay").GetComponentInChildren<RawImage>();
		miniHealthBar.rectTransform.localScale = new Vector3 (currHealth / maxHealth, 1, 1);
		Debug.Log ("Player Health: " + currHealth);
		if (currHealth <= 0) {
			Debug.Log ("Dead.");
			PlayerDeath pd = GetComponent<PlayerDeath> ();
			player.Deaths++;
			player.BuffList = new List<IBuffable> ();
			pd.DisablePlayer (player.Deaths);
		}
	}

    [Command]
	public void CmdTakeDamage(float damage){
        Debug.Log(gameObject.transform.name + " has taken " + damage + " damage");
        if (player == null)
            player = gameObject.GetComponent<Player>();
		if(player.state == "alive")
			currHealth -= damage;
		if (currHealth < 0) {
			currHealth = 0;
		}
		if (currHealth > 100) {
			currHealth = 100;
		}
    }

}
