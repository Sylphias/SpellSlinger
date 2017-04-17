using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Spells;
using System.Reflection;
public class PlayerHit : NetworkBehaviour
{
    Player player;
    PlayerController controller;
    delegate void MethodDelegate(float value);
    public override void OnStartLocalPlayer() {
        if (!isLocalPlayer) return;
        Debug.Log(gameObject.transform.name + "Playerhit Initialized");
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
        base.OnStartLocalPlayer();
    }

    public void ApplyDebuffBuff(string buffName, float value) {
        if (!isLocalPlayer) return;
        if (!Network.isServer)
            CmdAddEffectToPlayer(buffName, value);
        else
            GetComponent<PlayerHit>().Invoke(buffName, value);
    }

    public void OnHit(IDictionary hitMessages) {
        if (!isLocalPlayer) return;
        Debug.Log(transform.name + " has been hit!");
        foreach (DictionaryEntry hitMessage in hitMessages) {
            if (!Network.isServer)
                CmdAddEffectToPlayer(hitMessage.Key as string, (float)hitMessage.Value);
            else
            {
                GetComponent<PlayerHit>().Invoke(hitMessage.Key as string, (float)hitMessage.Value);
            }
        }
    }

    public void ApplyExplosiveKnockback(Vector3 explosionPoint, float explosionForce, float radius) {
        if (!isLocalPlayer) return;
        CmdExplosionKnockback(explosionPoint, explosionForce, radius);

    }

    public void ApplyKnockback(Vector3 direction, float force) {
        if (!isLocalPlayer) return;
        CmdKnockback(direction, force);
    }

    [Command]
    public void CmdKnockback(Vector3 direction, float force) {
        RpcKnockback(direction, force);
    }

    [Command]
    public void CmdExplosionKnockback(Vector3 explosionPoint, float explosionForce, float radius) {
        RpcExplosionKnockback(explosionPoint, explosionForce, radius);
    }
    [Command]
    public void CmdAddEffectToPlayer(string method, float value) {
        Type type = GetComponent<PlayerHit>().GetType();
        object[] values = { value };
        MethodInfo meth = type.GetMethod(method);
        meth.Invoke(GetComponent<PlayerHit>(), values);
    }

    [ClientRpc]
	public void RpcExplosionKnockback(Vector3 explosionPoint, float explosionForce, float radius ){
        if(!isLocalPlayer) return;
        Debug.Log("Knockback");
        Vector3 direction = (transform.position - explosionPoint);
        // ((transform.position,explosionPoint))* for later use
		GetComponent<Rigidbody> ().AddForce((1/Vector3.Distance(explosionPoint,transform.position))*(Vector3.Normalize(direction) * explosionForce), ForceMode.Impulse);
    }

	[ClientRpc]
	public void RpcKnockback(Vector3 direction, float force){
        if(!isLocalPlayer) return;
		Debug.Log ("Knockback");
		GetComponent<Rigidbody> ().AddForce ((direction * force),ForceMode.Impulse);
	}
	[ClientRpc]
	public void RpcSwift(float value){
		SwiftBuff sb = new SwiftBuff(controller.speed,controller.lookSensitivity);
		if (player.BuffList.Count == 0) {
			player.BuffList.Add (sb);
			return;
		}
		// Check if there is another swift debuf in the bufflist, if yes then replace with the new debuff to refresh the time
		replaceOldDebuff("BurnDebuff",sb);
	}


    [ClientRpc]
    public void RpcChilled(float value)
    {
        if (!isLocalPlayer) return;
        Debug.Log("Chilled");
        FrostDebuff fd = new FrostDebuff(controller.speed, controller.lookSensitivity);
        if (player.BuffList.Count == 0)
        {
            player.BuffList.Add(fd);
            return;
        }
        // Check if there is another chilled debuf in the bufflist, if yes then replace with the new debuff to refresh the time
        replaceOldDebuff("BurnDebuff", fd);
    }

	[ClientRpc]
	public void RpcBurned(float value){
        if(!isLocalPlayer) return;
		Debug.Log ("Burned");
		BurnDebuff br = new BurnDebuff(value);
		if (player.BuffList.Count == 0) {
			player.BuffList.Add (br);
			return;
		}
		replaceOldDebuff("BurnDebuff",br);
	}

    public void TakeDamage(float damage)
    {
        Debug.Log("damaged" + damage);
        GetComponent<HealthbarController>().CmdTakeDamage(damage);
    }

	public void replaceOldDebuff(string buffTypeString,IBuffable newBuff){
		foreach (IBuffable b in player.BuffList) {
			if (b.Type == buffTypeString) {
				if (b.ComparableValue < newBuff.ComparableValue) {
					b.Reset(gameObject);
					player.BuffList.Remove (b);
					player.BuffList.Add (newBuff);
				} else {
					b.FinishTime = newBuff.FinishTime;
				}
			}
		}
	}



}


