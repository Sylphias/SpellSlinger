using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Spells;
using System.Reflection;
public class PlayerHit:NetworkBehaviour
{
    Player player;
	public override void OnStartLocalPlayer(){
        Debug.Log("Player hit initialized");
        player = GetComponent<Player>();
		base.OnStartLocalPlayer ();
    }

    public void ApplyDebuffBuff(string buffName, float value){
		if(!isLocalPlayer) return;
        CmdAddEffectToPlayer(buffName,value);
    }
    public void OnHit(IDictionary hitMessages){
		if(!isLocalPlayer) return;
		if (hasAuthority) {
			Debug.Log ("Has Authority: " + gameObject.transform.name);
		} else {
			Debug.Log ("Does Not Have Authority" + gameObject.transform.name);
		}
        foreach (DictionaryEntry hitMessage in hitMessages){
            CmdAddEffectToPlayer(hitMessage.Key as string ,(float)hitMessage.Value);
        }
    }

	public void ApplyExplosiveKnockback(Vector3 explosionPoint, float explosionForce, float radius){
		if(!isLocalPlayer) return;
		CmdExplosionKnockback (explosionPoint, explosionForce, radius);
	
	}

    [Command]
	public void CmdExplosionKnockback(Vector3 explosionPoint, float explosionForce, float radius){
		RpcExplosionKnockback(explosionPoint,explosionForce,radius);
	}

    [Command]
    void CmdAddEffectToPlayer(string method, float value){
        PlayerHit player = GetComponent<PlayerHit>();
        System.Type type = player.GetType();
        object[] values = { value };
        MethodInfo meth = type.GetMethod(method);
        meth.Invoke(player,values);
    }

    [ClientRpc]
	public void RpcExplosionKnockback(Vector3 explosionPoint, float explosionForce, float radius ){
        if(!isLocalPlayer) return;
        Vector3 direction = (transform.position - explosionPoint);
//		GetComponent<Rigidbody> ().velocity = ((1/Vector3.Distance(transform.position,explosionPoint))*direction.normalized * explosionForce);
		GetComponent<Rigidbody> ().AddForce ((1/Vector3.Distance(transform.position,explosionPoint))*direction.normalized * explosionForce,ForceMode.Impulse);
	}


	[ClientRpc]
	public void RpcKnockback(Vector3 direction, float force){
        if(!isLocalPlayer) return;
        
		Debug.Log ("Knockback");
//		GetComponent<Rigidbody> ().velocity = (direction * force);
		GetComponent<Rigidbody> ().AddForce ((direction * force));

	}

	[ClientRpc]	
	public void RpcSwift(float value){
        if(!isLocalPlayer) return;
		Debug.Log(player.MovementMultiplier);
		SwiftBuff sb = new SwiftBuff(player.MovementMultiplier,player.RotationMultiplier);
		if (player.BuffList.Count == 0) {
			player.BuffList.Add (sb);
			return;
		}
		// Check if there is another swift debuf in the bufflist, if yes then replace with the new debuff to refresh the time
		replaceOldDebuff("BurnDebuff",sb);
	}
    // Can Reduce the number of buff methods by changing the structure of the Buff class
	[ClientRpc]
	public void RpcChilled(float value){
        if(!isLocalPlayer) return;
		Debug.Log ("Chilled");
		FrostDebuff fd = new FrostDebuff(player.MovementMultiplier,player.RotationMultiplier);
		if (player.BuffList.Count == 0) {
			player.BuffList.Add (fd);
			return;
		}

		// Check if there is another chilled debuf in the bufflist, if yes then replace with the new debuff to refresh the time
		replaceOldDebuff("BurnDebuff",fd);
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

	void replaceOldDebuff(string buffTypeString,IBuffable newBuff){
		foreach (IBuffable b in player.BuffList) {
			if (b.Type == buffTypeString) {
				if (b.ComparableValue < newBuff.ComparableValue) {
					b.Reset(player);
					player.BuffList.Remove (b);
					player.BuffList.Add (newBuff);
				} else {
					b.FinishTime = newBuff.FinishTime;
				}
			}
		}
	}

	// Heal and Damage through this function. If negative is a heal
	[ClientRpc]
	public void RpcTakeDamage(float damage){
        if(!isLocalPlayer) return;        	
		GetComponent<Player>().TakeDamage(damage);
		Debug.Log ("damaged" + damage);
	}


}


