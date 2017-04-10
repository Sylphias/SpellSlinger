using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Spells;
[RequireComponent(typeof(Player))]
public class Player : NetworkBehaviour {
	HealthbarController hb;
	[SyncVar] public string playerUniqueIdentity;
	private NetworkInstanceId playerNetID;
	private Transform myTransform;
    public GameObject bulletPrefab;



	
	public string[]spellList;
    
	float[] spellCooldown = new float[4] { 0, 0, 0, 0 };
	float[] spellLastCast = new float[4] { 0, 0, 0, 0 };


	Button [] buttonList = new Button[4];

	// We need to sync the speed and rotation of the player.
	[SyncVar]
	private float rotationSpeed,moveSpeed,damageMultiplier = 1 , radiusMultiplier = 1 , projectileSpeedMultiplier = 1 , knockbackModifier = 1;

	// We need to sync the buff list for the player.
	private List<IBuffable> buffList= new List<IBuffable> ();

    //other supporting vars
    private Vector3 moveVelocity;
 
    //objects (for reference to objects in game)
    private Rigidbody playerRigidBody;
    public Transform spawnPoint;

    //control (to control player)
    private VirtualJoystick joystick;
    private Vector3 moveInput;

	public float MoveSpeed{
		get{return moveSpeed;}
		set{ moveSpeed = value; }
	}
	public float RotationSpeed{
		get{return rotationSpeed;}
        set{rotationSpeed = value;}
	}
    // Use this for initialization

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

	 void Start(){
//		if (!isLocalPlayer)
//		{
//			return;
//		}
		// Initialize Player stats
		spellList = new string[4] { "Fireball", "Earthwall", "Frostnova", "Stonefist" };
		moveSpeed = 30;
		rotationSpeed = 5;
		InitializeButtons ();
		joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();
	}


	void InitializeButtons(){
		for (int i = 0; i < 4; i++) {			
			Button button = GameObject.Find("Cast"+i).GetComponent<Button>();
			button.name = i.ToString();
			Image icon = button.GetComponent<Image>();
			icon.sprite = Resources.Load("SpellIcons/"+spellList[i],typeof(Sprite))as Sprite;
			button.onClick.AddListener(delegate () { this.CmdCast( int.Parse(button.name)); });
			Debug.Log (spellList [i]);
			buttonList[i] = button;
		}
	}

	void OnDisable()
	{
		GameManager.UnRegisterPlayer(transform.name);
		Destroy(gameObject);
	}


    void Update ()
    {
		if (!isLocalPlayer)
		{
			return;
		}
		Move(); 
		checkBuffs ();
		UpdateCooldowns ();
    }

    public void Move()
    {		
        moveInput = joystick.getInput();
        moveVelocity = moveInput * moveSpeed;
		GetComponent<Rigidbody>().velocity = moveVelocity;
        //make the player look at the right direction
        transform.LookAt(transform.position + 100 * moveInput);
    }
		
    [Command]
    public void CmdCast(int spellNumber)
    {
		Debug.Log (spellList [spellNumber]);
        if ( Time.time - spellLastCast[spellNumber] >= spellCooldown[spellNumber]|| spellCooldown[spellNumber] == 0)
        {
            GameObject spellPrefab = Resources.Load("Spells/"+spellList[spellNumber], typeof(GameObject)) as GameObject;
			GameObject spell = (GameObject)Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
//			spellClass.Init();
//			SpellModifier.ModifySpell(spellClass,gameObject,damageMultiplier,projectileSpeedMultiplier,radiusMultiplier,knockbackModifier);
//			spellClass.Damage = 2;
//			Debug.Log (spellClass.Damage);


			NetworkServer.Spawn(spell);
			ISpell spellClass = spell.GetComponent<ISpell>();
			if(spellCooldown[spellNumber] == 0 )
				spellCooldown[spellNumber] = spellClass.Cooldown;
			// RpcInitializeSpellOnClient(spell,spellNumber);
			spellLastCast [spellNumber] = Time.time;
        }
    }

	// [ClientRpc]
	// // Ensures that all clients initialize the spell as well
	// void RpcInitializeSpellOnClient(GameObject spell,int spellNumber){
	// 	ISpell spellClass = spell.GetComponent<ISpell>();
	// 	spellClass.Init();
	// 	SpellModifier.ModifySpell(spellClass,gameObject,damageMultiplier,projectileSpeedMultiplier,radiusMultiplier,knockbackModifier);
	// 	if(spellCooldown[spellNumber] == 0 )
	// 	spellCooldown[spellNumber] = spellClass.Cooldown;
	// }

	void UpdateCooldowns(){
		for (int i = 0; i < 4; i++) {
			Image cooldownFader = buttonList[i].GetComponentInChildren<Image>();
			float timeElapsed = Time.time - spellLastCast[i];
			cooldownFader.fillAmount = timeElapsed >= spellCooldown[i] ? 0:timeElapsed/spellCooldown[i];
		}
	}

	public void Heal(float heal){
		hb.CurrentHealth += heal;
		if (hb.CurrentHealth > 100) {
			hb.CurrentHealth = 100;
		}
	}

	// If I have a fixed number of buffs, then I can just use a hashmap and check if the buff timer is active. This will speed things up.
	[ClientRpc]	
	public void RpcSwift(){
		SwiftBuff sb = new SwiftBuff(moveSpeed,rotationSpeed);
		if (buffList.Count == 0) {
			buffList.Add (sb);
			return;
		}

		// Check if there is another swift debuf in the bufflist, if yes then replace with the new debuff to refresh the time
		foreach (IBuffable b in buffList) {
			if (b.Type == "Swift") {
				FrostDebuff sbOld = b as FrostDebuff;
				if (sbOld.SpeedMultiplier < sb.SpeedMultiplier) {
					buffList.Remove (b);
					buffList.Add (sb);
				} else {
					sbOld.FinishTime = sb.FinishTime;
				}
			}
		}

	}

	[ClientRpc]
	public void RpcChilled(float value){
		Debug.Log ("Chilled");
		FrostDebuff fd = new FrostDebuff(moveSpeed,rotationSpeed);
		if (buffList.Count == 0) {
			buffList.Add (fd);
			return;
		}

		// Check if there is another chilled debuf in the bufflist, if yes then replace with the new debuff to refresh the time
		foreach (IBuffable b in buffList) {
			if (b.Type == "Chilled") {
				FrostDebuff fdOld = b as FrostDebuff;
				if (fdOld.SpeedMultiplier < fd.SpeedMultiplier) {
					buffList.Remove (b);
					buffList.Add (fd);
				} else {
					fdOld.FinishTime = fd.FinishTime;
				}
			}
		}
	}

	[ClientRpc]
	public void RpcExplosionKnockback(Vector3 explosionPoint, float explosionForce, float radius ){
		Debug.Assert (explosionForce != null);
		Vector3 direction = (transform.position - explosionPoint);
		GetComponent<Rigidbody> ().velocity = ((1/Vector3.Distance(transform.position,explosionPoint))*direction.normalized * explosionForce);
	}

	[ClientRpc]
	public void RpcKnockback(Vector3 direction, float force){
		Debug.Log ("Knockback");
		GetComponent<Rigidbody> ().velocity = (direction * force);
	}
	// Checks and updates buffs on the user. 
	// To prevent
	public void checkBuffs(){
		float oldTime = Time.time;
		// Make a copy of the buff list to iterate through
		IBuffable [] _buffListCopy = new IBuffable[buffList.Count];
		buffList.CopyTo(_buffListCopy,0);
		foreach (IBuffable b in _buffListCopy){
			if ((Time.time - oldTime) >= b.TickTime) {
				b.Apply (this);
				oldTime = Time.time;
			}
			if(b.Finished){
				b.Reset (this);
				buffList.Remove (b);
			}
		}
	}

	[ClientRpc]
	public void RpcTakeDamage(float damage){		
		Debug.Log ("damaged");
//		hb.CurrentHealth -= damage;
//		if (hb.CurrentHealth < 0) {
//			hb.CurrentHealth = 0;
//		}
	}

}
