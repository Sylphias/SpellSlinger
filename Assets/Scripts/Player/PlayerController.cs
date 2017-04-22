
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : NetworkBehaviour,IPlayer{
    [SyncVar]
    public float speed = 1f;
    [SyncVar]
    public float lookSensitivity = 1f;
    private PlayerMotor motor;
    private VirtualJoystick joystick;
    private Vector3 moveInput;

    [SerializeField]
    GameObject scoreBoard;

	// [Command]
    // public void CmdInit()
    // {
    //   Init();
    // }

    public void Init(){
        speed = 10f;
		lookSensitivity = 1f;
    }

    [Command]
    public void CmdUpdateSpeed(float speed){
		this.speed = speed;
    }

    [Command]
    public void CmdUpdateLookSensitivity(float sensitivity){
        lookSensitivity = sensitivity;
    }

    public override void OnStartLocalPlayer()
    {
		Init ();
        motor = GetComponent<PlayerMotor>();
        joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();
        base.OnStartLocalPlayer();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        moveInput = joystick.getInput();
        Vector3 velocity = (moveInput).normalized * speed;
        motor.Move(velocity);
        Vector3 rotation = new Vector3(moveInput.x, 0f, moveInput.z).normalized * lookSensitivity;
        motor.Rotate(rotation);
    }
}
