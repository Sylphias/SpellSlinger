using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using Spells;
public class ProjectileSpell: NetworkBehaviour,ISpell,IProjectile{
	private bool isCooldown, isDamage;
	private float duration,cooldown,damage,radius,explosionForce,projectileSpeed;

	public float Duration{
		get{return duration;}
		set{this.duration = value;}
	}

	public float Cooldown{
		get{ return cooldown; }
		set{ this.cooldown = value; }
	}

	public bool IsDamage{
		get{ return isDamage;}
		set{ this.isDamage = value; }
	}

	public float Damage{
		get{return damage;}
		set{ this.damage = value; }
	}

	public float Radius{
		get{ return radius; }
		set{ this.radius = value; }
	}

	public float ExplosionForce{
		get{ return explosionForce; }
		set{ this.explosionForce = value; }
	}

	public float ProjectileSpeed{
		get{ return projectileSpeed; }
		set{ this.projectileSpeed = value; }
	}

	public void initialize(float cooldown, float duration, float radius, float dmg, float speed, float force){
		Cooldown = cooldown;
		Duration = duration;
		Radius = radius;
		Damage = dmg;
		ProjectileSpeed = speed	;
		ExplosionForce = force;
	}

	public void explosionScan(IDictionary messages,Collider[] colliders, Vector3 explosionPoint){
		foreach (Collider c in colliders) {
			if (c.GetComponent<Rigidbody>() == null) {
				continue;
			}

			if (c.tag.Equals ("Player")) {
				Rigidbody otherPlayerBody = c.GetComponent<Rigidbody> ();
				Player player = c.GetComponent<Player>();
				RaycastHit rch;
				Physics.Linecast (gameObject.transform.position, otherPlayerBody.position, out rch);
				if(rch.collider.gameObject.tag == "Player"){
					otherPlayerBody.AddExplosionForce (ExplosionForce, explosionPoint, Radius, 1, ForceMode.Impulse);
					foreach(DictionaryEntry message in messages){
						string method = message.Key as string;
						float value = (float)message.Value;	
						Debug.Log(player.transform.name);
						CmdUpdatePlayerWithMessage(player.transform.name, method,value);
					}
				}
			}
		}
	}
	// Think about refactoring later. complete core features first.
	private void CmdUpdatePlayerWithMessage(string uniqueID, string method, float value)
	{
		Debug.Log ("Sending message");
		GameObject[] rawr = GameObject.FindGameObjectsWithTag ("Player");
		GameObject go = GameObject.Find(uniqueID);
		Player player = go.GetComponent<Player>();
		System.Type type = player.GetType();
		object[] values = { value };
		MethodInfo meth = type.GetMethod(method);
		meth.Invoke(player,values);
	}
}


