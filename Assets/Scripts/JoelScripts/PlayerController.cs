
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : NetworkBehaviour{
    [SerializeField]
    private float speed = 100f;
    private float lookSensitivity = 3f;
    private PlayerMotor motor;

    private VirtualJoystick joystick;
    private Vector3 moveInput;

    [SerializeField]
    GameObject scoreBoard;


    public override void OnStartLocalPlayer()
    {
        motor = GetComponent<PlayerMotor>();
        joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();
    }

    void Update()
    {
        // code for keyboard
        //float xMovement = Input.GetAxisRaw("Horizontal");
        //float yMovement = Input.GetAxisRaw("Vertical");
        // Vector3 moveHorizontal = transform.right * xMovement;
        //Vector3 moveVertical = transform.forward * yMovement;
        //Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed; // ensure that you wont get varying speed for x and y axis

        moveInput = joystick.getInput();
        Vector3 velocity = (moveInput).normalized * speed;
        motor.Move(velocity);
        // turning around
        float yRotate = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0f, yRotate, 0f) * lookSensitivity;
        motor.Rotate(rotation);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }
}
