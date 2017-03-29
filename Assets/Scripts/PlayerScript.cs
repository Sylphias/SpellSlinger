using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Spells;
public class PlayerScript : NetworkBehaviour {
	HealthbarController hb;

    public GameObject bulletPrefab;
	private List<IBuffable> buffList= new List<IBuffable> ();

    //stats (movement speed, spells)
    private float moveSpeed = 5;
    public string[] spellList = new string[4] { "Fireball", "Iceball", "Earthwall", "Debuff" };
    public float[] spellCooldown = new float[4] { 0, 0, 0, 0 };
	private float rotationSpeed= 100;
    //private List<IBuffable> buffList = new List<IBuffable>();

    //other supporting vars
    private Vector3 moveVelocity;
 
    //objects (for reference to objects in game)
    public Rigidbody playerRigidBody;
    public Transform spawnPoint;
    public Button button0;
    public Button button1;
    public Button button2;
    public Button button3;

	private Animation playerAnimations;

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
    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        hb = gameObject.GetComponent<HealthbarController> ();
        joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();
        playerRigidBody = GetComponent<Rigidbody>();
		spawnPoint = transform.Find("SpawnPoint");

		playerAnimations = gameObject.GetComponent<Animation> ();
        button0 = GameObject.Find("Cast0").GetComponent<Button>();
        Debug.Log("AddingListener");
        button0.onClick.AddListener(delegate () { this.CmdCast0(); });

//        button1 = GameObject.Find("Cast1").GetComponent<Button>();
//        button1.onClick.AddListener(delegate () { this.CmdCast1(); });
//
//        button2 = GameObject.Find("Cast2").GetComponent<Button>();
//		button2.onClick.AddListener(delegate () { this.CmdCast2(); });
//        
//
//        button3 = GameObject.Find("Cast3").GetComponent<Button>();
//		button3.onClick.AddListener(delegate () { this.CmdCast3(); });
//        
    }


    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Move();

    }

    public void Move()
    {	
		
        moveInput = joystick.getInput();

		if (Mathf.Abs (moveInput.x) > 0.1 || Mathf.Abs (moveInput.z) > 0.1) {
			playerAnimations.CrossFade ("Walk");
		}
		else
			playerAnimations.CrossFade("Idle1");
        moveVelocity = moveInput * moveSpeed;
        playerRigidBody.velocity = moveVelocity;
        //make the player look at the right direction
        transform.LookAt(transform.position + 100 * moveInput);
    }

    [Command]
    public void CmdCast0()
    {
		playerAnimations.CrossFade("Attack");
        if (spellCooldown[0]==0)
        {
            Debug.Log("Casting spell 1");
            GameObject spellPrefab = Resources.Load(spellList[1], typeof(GameObject)) as GameObject;

            var spell = (GameObject)Instantiate(spellPrefab, spawnPoint.position, spawnPoint.rotation);

            if (!NetworkServer.localClientActive)
            {
                Debug.Log("It is not active on server");
            }
            //spellCooldown[0] = 1;
            NetworkServer.Spawn(spell);
        }
    }
 
//    [Command]
//    public void CmdCast1()
//    {
//        Debug.Log("Preparing to cast");
//
//        if (spellCooldown[0] == 0)
//        {
//            Debug.Log("Casting spell 2");
//            //GameObject spellPrefab = Resources.Load("Spell", typeof(GameObject)) as GameObject;
//
//			var spell = (GameObject)Instantiate(spellList[1], spawnPoint.position, spawnPoint.rotation);
//            spell.GetComponent<Rigidbody>().velocity = spell.transform.forward * 6;
//
//            //spellCooldown[0] = 1;
//            NetworkServer.Spawn(spell);
//            Destroy(spell, 2.0f);
//        }
//    }
//
//	public void CmdCast2()
//	{
//		Debug.Log("Preparing to cast");
//
//		if (spellCooldown[0] == 0)
//		{
//			Debug.Log("Casting spell 2");
//			//GameObject spellPrefab = Resources.Load("Spell", typeof(GameObject)) as GameObject;
//
//			var spell = (GameObject)Instantiate(spellList[2], spawnPoint.position, spawnPoint.rotation);
//			spell.GetComponent<Rigidbody>().velocity = spell.transform.forward * 6;
//
//			//spellCooldown[0] = 1;
//			NetworkServer.Spawn(spell);
//			Destroy(spell, 2.0f);
//		}
//	}
//
//	public void CmdCast3()
//	{
//		Debug.Log("Preparing to cast");
//
//		if (spellCooldown[0] == 0)
//		{
//			Debug.Log("Casting spell 2");
//			//GameObject spellPrefab = Resources.Load("Spell", typeof(GameObject)) as GameObject;
//
//			var spell = (GameObject)Instantiate(spellList[3], spawnPoint.position, spawnPoint.rotation);
//			spell.GetComponent<Rigidbody>().velocity = spell.transform.forward * 6;
//
//			//spellCooldown[0] = 1;
//			NetworkServer.Spawn(spell);
//			Destroy(spell, 2.0f);
//		}
//	}

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


	public void TakeDamage(float damage){		
		Debug.Log ("damaged");
		hb.CurrentHealth -= damage;
		if (hb.CurrentHealth < 0) {
			hb.CurrentHealth = 0;
		}
		Update();
	}


	public void Heal(float heal){
		hb.CurrentHealth += heal;
		if (hb.CurrentHealth > 100) {
			hb.CurrentHealth = 100;
		}
		Update();
	}



	public void Swift(float speedMultiplier){
		SwiftBuff sb = new SwiftBuff(5,0,speedMultiplier,moveSpeed,rotationSpeed);
		if (buffList.Count == 0) {
			buffList.Add (sb);
			return;
		}
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


	public void Chilled(float speedMultiplier){
		FrostDebuff fd = new FrostDebuff(5,0,speedMultiplier,moveSpeed,rotationSpeed);
		if (buffList.Count == 0) {
			buffList.Add (fd);
			return;
		}
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
}
