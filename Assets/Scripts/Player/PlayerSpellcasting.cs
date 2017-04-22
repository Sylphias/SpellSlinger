using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Spells;

class PlayerSpellcasting : NetworkBehaviour,IPlayer
{

    private Player player;
	public Transform spawnPoint;
	public SyncListString spellList = new SyncListString ();

    Button [] buttonList = new Button[4];
    private Animation playerAnimations;
	float[] spellCooldown = new float[4] { 0, 0, 0, 0 };
	float[] spellLastCast = new float[4] { 0, 0, 0, 0 };
    string[] spellTypes = new string[4];
	[SyncVar] private float damageMultiplier;
	[SyncVar] private float radiusMultiplier;
	[SyncVar] private float  projectileSpeedMultiplier;
	[SyncVar] private float  knockbackMultiplier;
    

	public override void OnStartLocalPlayer(){
        if (!isLocalPlayer) return;
		Init ();
		if (spellList == null || spellList.Count == 0)
			InitializeSpellListIfEmpty ();
		player = GetComponent<Player>();
		InitializeButtons();
        InitializeCooldowns();
        playerAnimations = GetComponent<Animation>();
        base.OnStartLocalPlayer();
	}

	private void InitializeSpellListIfEmpty(){
		string [] defaultSpellList = new string[4] { "Forcelightning", "Firenova", "Stonefist", "Blink" };
		foreach (string s in defaultSpellList)
			spellList.Add (s);
	}

	void FixedUpdate(){
		if (!isLocalPlayer)
			return;
		UpdateCooldowns();
	}

	[Command]
	void CmdUpdateSpellMultipliers(float damageMultiplier, float radiusMultiplier, float projectileSpeedMultiplier, float knockbackMultiplier){
		this.damageMultiplier = damageMultiplier;
		this.radiusMultiplier = radiusMultiplier;
		this.projectileSpeedMultiplier = projectileSpeedMultiplier;
		this.knockbackMultiplier = knockbackMultiplier;
	}
	public void Init(){
		damageMultiplier =1;
		radiusMultiplier = 1;
		projectileSpeedMultiplier = 1;
		knockbackMultiplier = 1;
	}


	//Set all the player cooldowns
	[Client]
	void InitializeCooldowns(){
		for(int i = 0 ; i< spellList.Count; i ++ ) {
			GameObject spellPrefab = Resources.Load ("Spells/" + spellList[i], typeof(GameObject)) as GameObject;
			ISpell preInitializedSpell = spellPrefab.GetComponent<ISpell>();
            preInitializedSpell.Init();// Initialize the spell with default values
            spellTypes[i] = preInitializedSpell.GetSpellType;// To prevent additional overheads from loading the spell every time it is cast on the client
            spellCooldown[i] = preInitializedSpell.Cooldown;
		}
	}

    [Client]
	void InitializeButtons(){
		if(player.state == "endgameover") return;
		for (int i = 0; i < 4; i++) {	
			Button button = GameObject.Find("Cast"+i).GetComponent<Button>();
			button.name = i.ToString();
			Image icon = button.GetComponent<Image>();
			icon.sprite = Resources.Load("SpellIcons/"+spellList[i],typeof(Sprite))as Sprite;
			button.onClick.AddListener(delegate () { this.CastSpell( int.Parse(button.name)); });
			buttonList[i] = button;
		}
	}
	[Client]
	void CastSpell(int spellNumber){
        if ( (Time.time - spellLastCast[spellNumber] >= spellCooldown[spellNumber]|| spellCooldown[spellNumber] == 0)&& player.state != "dead")
        {
			if (player == null)
				player = GetComponent<Player> ();
			if (player.state != "alive")
				return;
			playerAnimations.CrossFade("Attack");
            Vector3 spellSpawn = ((spellTypes[spellNumber] == "nova" || spellTypes[spellNumber] == "buff")) ?
transform.position : spawnPoint.position; 
            CmdCast(spellList[spellNumber],damageMultiplier,projectileSpeedMultiplier,radiusMultiplier,knockbackMultiplier,spellSpawn,spawnPoint.rotation);
			spellLastCast [spellNumber] = Time.time;

		}
	}

    [Command]
	void CmdCast(string spellName, float castedDamageMultiplier, float castedProjectileSpeedModifier, float castedRadiusMultiplier, float castedKnockbackMultiplier,Vector3 spellSpawnPos,Quaternion spellSpawnRotation)
    {
		GameObject spell = (GameObject)Instantiate(Resources.Load("Spells/" + spellName, typeof(GameObject)), spellSpawnPos, spellSpawnRotation);
		NetworkServer.SpawnWithClientAuthority(spell,connectionToClient);
		RpcInitializeClientSpell(spell,castedDamageMultiplier,castedProjectileSpeedModifier,castedRadiusMultiplier,castedKnockbackMultiplier);
    }

	[ClientRpc]
	void RpcInitializeClientSpell(GameObject spell, float castedDamageMultiplier, float castedProjectileSpeedModifier, float castedRadiusMultiplier, float castedKnockbackMultiplier){
			ISpell spellClass = spell.GetComponent<ISpell>();
			spellClass.Init();
			SpellModifier.ModifySpell(spellClass,gameObject,castedDamageMultiplier,castedProjectileSpeedModifier,castedRadiusMultiplier,castedKnockbackMultiplier);
	}

	void UpdateCooldowns(){
		if (player.state != "endgameover") {
			for (int i = 0; i < 4; i++) {
				Image cooldownFader = GameObject.FindWithTag ("Cd" + i).GetComponent<Image> ();
				float timeElapsed = Time.time - spellLastCast [i];
				cooldownFader.fillAmount = timeElapsed >= spellCooldown [i] ? 0 : (1 - timeElapsed / spellCooldown [i]);
			}
		}
	}

}
