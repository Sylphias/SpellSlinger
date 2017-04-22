using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour,IPlayer {

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Rigidbody rigidBody;
    private Animation playerAnimations;
    private Player player;

    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer) return;
		Init ();
        player = GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody>();
		base.OnStartLocalPlayer ();
    }
	public void Init(){
		playerAnimations = GetComponent<Animation>();
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
		if (playerAnimations == null)
			playerAnimations = GetComponent<Animation> ();
//		|| Mathf.Abs (velocity.z) > 0.2)
		if (Mathf.Abs (GetComponent<Rigidbody>().velocity.magnitude) > 0.2 || Mathf.Abs (velocity.z) > 0.2||Mathf.Abs (velocity.x) > 0.2)
		{
			playerAnimations.CrossFade("Run");
		} else if (!playerAnimations.IsPlaying("Idle2")&& !playerAnimations.IsPlaying("Death1")) {
			playerAnimations.CrossFade("Idle2");
		} 
        if (!isLocalPlayer) return;
        if (player.state == "alive" || player.state == "respawned") 
            PerformMovement();
    }

    void PerformMovement()
    {

        if (velocity != Vector3.zero)
        {
            PerformRotation();
            rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
        }
    }

    [Command]
    void CmdPlayMovementAnimations(string animation)
    {
        RpcPlayMovement(animation);
    }

    [ClientRpc]
    void RpcPlayMovement(string animation)
    {
        if (isServer)
            return;
        //playerAnimations.CrossFade(animation);
    }

    void PerformRotation()
    {
            transform.rotation = Quaternion.LookRotation(rotation);
    }
}
