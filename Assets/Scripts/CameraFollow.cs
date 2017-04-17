using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject player;
    public VirtualJoystick joystick;
    private void Start()
    {
        joystick = GameObject.Find("Joystick").GetComponent<VirtualJoystick>();
    }
    private void Update()
    {
        if (player != null)
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        else
            FreeCamMove();
    }

    void FreeCamMove()
    {
        Vector3 moveInput = joystick.getInput();
        Vector3 velocity = (moveInput).normalized ;
        transform.position += velocity;
    }
}
