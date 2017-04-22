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
    public void OnChangeHealth(float health)
	{
		currHealth = health;
		if (isLocalPlayer)
			healthBarFill.localScale = new Vector3 (1f, currHealth / maxHealth, 1f);
		if(miniHealthBar == null)
        	miniHealthBar = transform.FindChild("HealthBarBg").FindChild("HPOverlay").GetComponentInChildren<RawImage>();
		miniHealthBar.rectTransform.localScale = new Vector3 (currHealth / maxHealth, 1, 1);
		if (currHealth <= 0  && isLocalPlayer) {
			if (player.state == "alive") {
				Debug.Log ("Dead.");
				PlayerDeath pd = GetComponent<PlayerDeath> ();
				pd.DisablePlayer ();
			}
		}
	}

	[Command]
	public void CmdSetToFullHealth(){
		currHealth = maxHealth;
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
