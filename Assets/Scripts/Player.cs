using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Spells;
[RequireComponent(typeof(Player))]
public class Player : NetworkBehaviour {
	HealthbarController hb;
	private Transform myTransform;
	
	public string[]spellList;
	float[] spellCooldown = new float[4] { 0, 0, 0, 0 };
	float[] spellLastCast = new float[4] { 0, 0, 0, 0 };
	Button [] buttonList = new Button[4];
    
	[SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;
    private bool firstSetup = true;
    [SerializeField]
    private int maxHealth = 100;
	[SyncVar]
	public float currentHealth;
	[SyncVar] public string playerUniqueIdentity;
	// We need to sync the speed and rotation of the player.
	[SyncVar] private float rotationMultiplier;
	[SyncVar] private float movementMultiplier;
	[SyncVar] private float damageMultiplier;
	[SyncVar] private float radiusMultiplier;
	[SyncVar] private float  projectileSpeedMultiplier;
	[SyncVar] private float  knockbackMultiplier;
	// We need to sync the buff list for the player.
	public Transform spawnPoint;
	//control (to control player)
	private VirtualJoystick joystick;
	private Vector3 moveInput;

	public float MovementMultiplier{get{return movementMultiplier;}set{movementMultiplier= value;}}
	public float RotationMultiplier{get{return rotationMultiplier;}set{rotationMultiplier= value;}}
	public float DamageMultiplier{get{return damageMultiplier;}set{damageMultiplier =value;}}
	public float ProjectileSpeedMultiplier{get{return projectileSpeedMultiplier;}set{projectileSpeedMultiplier = value;}}
	public float KnockbackMultiplier{get{return knockbackMultiplier;}set{knockbackMultiplier=value;}}
	public float RadiusMultiplier{get{return radiusMultiplier;}set{radiusMultiplier=value;}}
	public int Kills{get;set;}
	public int Deaths{get;set;}
	
	public List<IBuffable> BuffList{get;set;}
	private	void Awake()
	{
		myTransform = transform;
	}       

	// Generate Player ID only once player has joined the server

	public override void OnStartClient(){
		base.OnStartClient();
		string _netID =  GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();
		GameManager.RegisterPlayer(_netID,_player);
	}

    [SyncVar]
    private bool isDead = false;

    public bool IsDead{get{return isDead;}protected set{isDead = value;}}

	public override void OnStartLocalPlayer(){
		if (!isLocalPlayer)
		{
			return;
		}
		Debug.Log ("Player Initialized");
		// Initialize Player stats
		InitializePlayerValues();
		InitializeButtons ();
		InitializeCooldowns ();
		joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();
		base.OnStartLocalPlayer();
	}

	[Client]
	void InitializePlayerValues(){
		spellList = new string[4] { "Iceball", "Swiftness", "Fireball", "Stonefist" };
		MovementMultiplier = 30;
		RotationMultiplier = 5;
		DamageMultiplier =2;
		RadiusMultiplier = 1;
		ProjectileSpeedMultiplier = 1;
		KnockbackMultiplier = 1;
		isDead = false;
        currentHealth = maxHealth;
		BuffList = new List<IBuffable> ();
	}
	[Client]
	void InitializeButtons(){
		for (int i = 0; i < 4; i++) {			
			Button button = GameObject.Find("Cast"+i).GetComponent<Button>();
			button.name = i.ToString();
			Image icon = button.GetComponent<Image>();
			icon.sprite = Resources.Load("SpellIcons/"+spellList[i],typeof(Sprite))as Sprite;
			button.onClick.AddListener(delegate () { this.CastSpell( int.Parse(button.name)); });
			Debug.Log (spellList [i]);
			buttonList[i] = button;
		}
	}

	//Set all the player cooldowns
	[Client]
	void InitializeCooldowns(){
		for(int i = 0 ; i< spellList.Length; i ++ ) {
			GameObject spellPrefab = Resources.Load ("Spells/" + spellList[i], typeof(GameObject)) as GameObject;
			ISpell preInitializedSpell = spellPrefab.GetComponent<ISpell>();
			spellCooldown[i] = preInitializedSpell.Cooldown;
		}
	}
	void OnDisable()
	{
		GameManager.DeregisterPlayer(transform.name);
		Destroy(gameObject);
	}


    void Update ()
    {

    }


	// Fixed Update ticks every 0.02
	void FixedUpdate()
	{
		if (!isLocalPlayer){
			return;
		}
		Move(); 
		checkBuffs ();
		UpdateCooldowns ();
	}

    public void Move()
    {
        moveInput = joystick.getInput();
        Vector3 moveVelocity = moveInput * MovementMultiplier;
		GetComponent<Rigidbody>().velocity = moveVelocity;
        //make the player look at the right direction
        transform.LookAt(transform.position + 100 * moveInput);
    }
	
	[Client]
	void CastSpell(int spellNumber){
        if ( Time.time - spellLastCast[spellNumber] >= spellCooldown[spellNumber]|| spellCooldown[spellNumber] == 0)
        {
			CmdCast(spellList[spellNumber],DamageMultiplier,ProjectileSpeedMultiplier,RadiusMultiplier,KnockbackMultiplier);
		}
	}

    [Command]
	public void CmdCast(string spellName, float castedDamageMultiplier, float castedProjectileSpeedModifier, float castedRadiusMultiplier, float castedKnockbackMultiplier)
    {
		GameObject spellPrefab = Resources.Load("Spells/"+spellName, typeof(GameObject)) as GameObject;
		ISpell spellClass = spellPrefab.GetComponent<ISpell>();
		Vector3 spellSpawn = ((spellClass.GetSpellType == "nova" || spellClass.GetSpellType=="buff"))?
		transform.position:spawnPoint.position;
		GameObject spell = (GameObject)Instantiate(spellPrefab, spellSpawn, spawnPoint.rotation);
		spellClass = spell.GetComponent<ISpell>();
		NetworkServer.SpawnWithClientAuthority(spell,connectionToClient);
		RpcInitializeClientSpell(spell,castedDamageMultiplier,castedProjectileSpeedModifier,castedRadiusMultiplier,castedKnockbackMultiplier);
    }

	[ClientRpc]
	void RpcInitializeClientSpell(GameObject spell, float castedDamageMultiplier, float castedProjectileSpeedModifier, float castedRadiusMultiplier, float castedKnockbackMultiplier){
			Debug.Log("Called Client Spell");
			
			ISpell spellClass = spell.GetComponent<ISpell>();
			spellClass.Init();
			SpellModifier.ModifySpell(spellClass,gameObject,castedDamageMultiplier,castedProjectileSpeedModifier,castedRadiusMultiplier,castedKnockbackMultiplier);
	}

	void UpdateCooldowns(){
		for (int i = 0; i < 4; i++) {
			Image cooldownFader = buttonList[i].GetComponentInChildren<Image>();
			float timeElapsed = Time.time - spellLastCast[i];
			cooldownFader.fillAmount = timeElapsed >= spellCooldown[i] ? 0:timeElapsed/spellCooldown[i];
		}
	}
	public void TakeDamage(float damage){
		hb.CurrentHealth -= damage;
		if (hb.CurrentHealth < 0) {
			hb.CurrentHealth = 0;
		}
		if (hb.CurrentHealth > 100) {
			hb.CurrentHealth = 100;
		}
	}

	// If I have a fixed number of buffs, then I can just use a hashmap and check if the buff timer is active. This will speed things up.
	// Checks and updates buffs on the user. 
	// To prevent
	public void checkBuffs(){
		float oldTime = Time.time;
		// Make a copy of the buff list to iterate through
		IBuffable [] _buffListCopy = new IBuffable[BuffList.Count];
		BuffList.CopyTo(_buffListCopy,0);
		foreach (IBuffable b in _buffListCopy){
			if ((Time.time - oldTime) >= b.TickTime) {
				b.Apply (this);
				oldTime = Time.time;
			}
			if(b.Finished){
				b.Reset (this);
				BuffList.Remove (b);
			}
		}
	}
	
}
