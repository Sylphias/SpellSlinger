using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Spells;
public class PlayerScript : NetworkBehaviour {
	HealthbarController hb;
	[SyncVar] public string playerUniqueIdentity;
	private NetworkInstanceId playerNetID;
	private Transform myTransform;
    public GameObject bulletPrefab;


    //stats (movement speed, spells)

    public string[] spellList = new string[4] { "Fireball", "Iceball", "Earthwall", "Stonefist" };
    
	public float[] spellCooldown = new float[4] { 0, 0, 0, 0 };

	// We need to sync the speed and rotation of the player.
	[SyncVar]
	private float rotationSpeed,moveSpeed;
	// We need to sync the buff list for the player.
	private List<IBuffable> buffList= new List<IBuffable> ();

    //other supporting vars
    private Vector3 moveVelocity;
 
    //objects (for reference to objects in game)
    private Rigidbody playerRigidBody;
    public Transform spawnPoint;
    private Button button0, button1, button2, button3;
	// Talking to gamelab dev guy, he says keep a matrix of debuff values in the player and not on the spell itself

    //control (to control player)
    public VirtualJoystick joystick;
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

	public override void OnStartLocalPlayer(){
		if (!isLocalPlayer)
		{
			return;
		}

		GetNetworkIdentity();
		SetIdentity();

		// Initialize Player stats
		moveSpeed = 30;
		rotationSpeed = 5;
		joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();


		spawnPoint = gameObject.transform.Find("SpawnPoint");
		playerRigidBody = GetComponent<Rigidbody>();
		//		playerAnimations = gameObject.GetComponent<Animation> ();
		button0 = GameObject.Find("Cast0").GetComponent<Button>();
		Debug.Log("AddingListener");
		button0.onClick.AddListener(delegate () { this.CmdCast0(); });
		Debug.Log ("Button Initialized");
        button1 = GameObject.Find("Cast1").GetComponent<Button>();
        button1.onClick.AddListener(delegate () { this.CmdCast1(); });

        button2 = GameObject.Find("Cast2").GetComponent<Button>();
		button2.onClick.AddListener(delegate () { this.CmdCast2(); });
        button3 = GameObject.Find("Cast3").GetComponent<Button>();
		button3.onClick.AddListener(delegate () { this.CmdCast3(); });

	}

    // Update is called once per frame
    void Update ()
    {
		if(myTransform.name == "" || myTransform.name == "PlayerPrefab(Clone)")
		{
			SetIdentity();
		}
        if (!isLocalPlayer)
        {
            return;
        }

        Move();
		checkBuffs ();
    }

    public void Move()
    {		
		
        moveInput = joystick.getInput();
        moveVelocity = moveInput * moveSpeed;
        playerRigidBody.velocity = moveVelocity;
        //make the player look at the right direction
        transform.LookAt(transform.position + 100 * moveInput);
    }

	// Can simplify to 1 method that takes in the button number as argument.
    [Command]
    public void CmdCast0()
    {
//		playerAnimations.CrossFade("Attack");
		Debug.Log("Casting spel");
        if (spellCooldown[0]==0)
        {
            Debug.Log("Casting spell 1");
            GameObject spellPrefab = Resources.Load(spellList[1], typeof(GameObject)) as GameObject;

            var spell = (GameObject)Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(spell);
        }
    }
 
    [Command]
    public void CmdCast1()
    {
        Debug.Log("Preparing to cast");

        if (spellCooldown[0] == 0)
        {
			GameObject spellPrefab = Resources.Load(spellList[0], typeof(GameObject)) as GameObject;
			var spell = (GameObject)Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
			NetworkServer.Spawn(spell);
        }
    }
	[Command]
	public void CmdCast2()
	{
		Debug.Log("Preparing to cast");

		if (spellCooldown[0] == 0)
		{
            GameObject spellPrefab = Resources.Load(spellList[2], typeof(GameObject)) as GameObject;

            var spell = (GameObject)Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(spell);
		}
	}
	[Command]
	public void CmdCast3()
	{
		Debug.Log("Preparing to cast");

		if (spellCooldown[0] == 0)
		{
            GameObject spellPrefab = Resources.Load(spellList[3], typeof(GameObject)) as GameObject;
            var spell = (GameObject)Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(spell);
		}
	}


	public void Heal(float heal){
		hb.CurrentHealth += heal;
		if (hb.CurrentHealth > 100) {
			hb.CurrentHealth = 100;
		}
		Update();
	}


	[Client]
	public void GetNetworkIdentity(){
		playerNetID = GetComponent<NetworkIdentity>().netId;
		CmdInstantiateIdentityOnServer(MakeUniqueIdentity());
	}
	
	[Client]
	public void SetIdentity(){
		if(isLocalPlayer)
		{
			myTransform.name = playerUniqueIdentity;
		}
		else
		{
			myTransform.name = MakeUniqueIdentity();
		}
	}

	private string MakeUniqueIdentity()
	{
		return "Player" + playerNetID.ToString();
	}

	[Command]
	private void CmdInstantiateIdentityOnServer(string name){
		playerUniqueIdentity = name;
	}
		
	public void Swift(){
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

	public void Chilled(float value){
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


	// Checks and updates buffs on the user. 
	public void checkBuffs(){
		float oldTime = Time.time;
		foreach (IBuffable b in buffList){
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


	public bool TakeDamage(float damage){		
		Debug.Log ("damaged");
//		hb.CurrentHealth -= damage;
//		if (hb.CurrentHealth < 0) {
//			hb.CurrentHealth = 0;
//		}
		return true;
	}

}
