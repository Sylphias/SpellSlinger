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

	// Generate Player ID only once player has joined the server
	public override void OnStartClient(){
		base.OnStartClient();
		string _netID =  GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();
		GameManager.RegisterPlayer(_netID,_player);
	}

	// Set default fields for player when player object is created (Handled by the machine)
	public override void OnStartLocalPlayer(){
		if (!isLocalPlayer)
		{
			return;
		}

		// Initialize Player stats
		moveSpeed = 30;
		rotationSpeed = 5;
		joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();


		spawnPoint = gameObject.transform.Find("SpawnPoint");
		playerRigidBody = GetComponent<Rigidbody>();
		//		playerAnimations = gameObject.GetComponent<Animation> ();

		#region declareButtons
		button0 = GameObject.Find("Cast0").GetComponent<Button>();
		button0.onClick.AddListener(delegate () { this.CmdCast(0); });
        button1 = GameObject.Find("Cast1").GetComponent<Button>();
        button1.onClick.AddListener(delegate () { this.CmdCast(1); });
        button2 = GameObject.Find("Cast2").GetComponent<Button>();
		button2.onClick.AddListener(delegate () { this.CmdCast(2); });
        button3 = GameObject.Find("Cast3").GetComponent<Button>();
		button3.onClick.AddListener(delegate () { this.CmdCast(3); });
		#endregion 
	}

	void OnDisable()
	{
		GameManager.UnRegisterPlayer(transform.name);
	}

    void Update ()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Move();
		checkBuffs ();
    }

	public void CmdCastTest(string theString){
		Debug.Log(theString);
	}

    public void Move()
    {		
		
        moveInput = joystick.getInput();
        moveVelocity = moveInput * moveSpeed;
        playerRigidBody.velocity = moveVelocity;
        //make the player look at the right direction
        transform.LookAt(transform.position + 100 * moveInput);
    }



    [Command]
    public void CmdCast(int spellNumber)
    {
        if (spellCooldown[spellNumber]== 0)
        {
			Debug.Log("Casting spell");
			Object[] test = Resources.LoadAll("");
            GameObject spellPrefab = Resources.Load(spellList[spellNumber], typeof(GameObject)) as GameObject;
            var spell = (GameObject)Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(spell);
        }
    }


	public void Heal(float heal){
		hb.CurrentHealth += heal;
		if (hb.CurrentHealth > 100) {
			hb.CurrentHealth = 100;
		}
	}


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
