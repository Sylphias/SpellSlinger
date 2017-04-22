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
    GameObject fireExp,frostExp;

    public override void OnStartLocalPlayer() {
        if (!isLocalPlayer) return;
        player = GetComponent<Player>();
        controller = GetComponent<PlayerController>();
        fireExp = Resources.Load ("Spells/Firedot", typeof(GameObject))as GameObject;
        frostExp = Resources.Load ("Spells/Frostdot", typeof(GameObject))as GameObject;
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
    public void CmdAddEffectToPlayer(string method, float value) {
        Type type = GetComponent<PlayerHit>().GetType();
        object[] values = { value };
        MethodInfo meth = type.GetMethod(method);
        meth.Invoke(GetComponent<PlayerHit>(), values);
    }

    [Command]
    public void CmdKnockback(Vector3 direction, float force) {
        RpcKnockback(direction, force);
    }

    [Command]
    public void CmdExplosionKnockback(Vector3 explosionPoint, float explosionForce, float radius) {
        RpcExplosionKnockback(explosionPoint, explosionForce, radius);
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
        float duration = 5.0f;
        FrostDebuff fd = new FrostDebuff(controller.speed, controller.lookSensitivity,duration);
		GameObject exp =(GameObject) Instantiate (frostExp, transform.position, transform.rotation);
		spawnDoTEffect (exp, duration);
		if (player.BuffList.Count == 0)
        {
            player.BuffList.Add(fd);
            return;
        }
        // Check if there is another chilled debuf in the bufflist, if yes then replace with the new debuff to refresh the time
        replaceOldDebuff("FrostDebuff", fd);
    }

	[ClientRpc]
	public void RpcBurned(float value){
        if(!isLocalPlayer) return;
		Debug.Log ("Burned");
        float duration = 10.0f;
		GameObject exp =(GameObject) Instantiate (fireExp, transform.position, transform.rotation);
		spawnDoTEffect (exp, duration);
		BurnDebuff br = new BurnDebuff(value,duration);
		if (player.BuffList.Count == 0) {
			player.BuffList.Add (br);
			return;
		}

		replaceOldDebuff("BurnDebuff",br);
	}

	void spawnDoTEffect(GameObject exp, float duration){
		exp.GetComponent<DamageOverTimeEffect>().player = gameObject;
		Destroy (exp, duration);
	}


    public void TakeDamage(float damage)
    {
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


