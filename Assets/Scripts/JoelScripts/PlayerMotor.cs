using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour {

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Rigidbody rigidBody;

    private Player player;

    public override void OnStartLocalPlayer()
    {
		if (!isLocalPlayer)
		{
			return;
		}
        player = GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody>();
		base.OnStartLocalPlayer ();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }


    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime* player.MovementMultiplier);
        }
    }

    void PerformRotation()
    {
        rigidBody.MoveRotation(rigidBody.rotation * Quaternion.Euler(rotation));
    }
}
