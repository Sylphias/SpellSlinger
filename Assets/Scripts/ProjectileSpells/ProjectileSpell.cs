using UnityEngine;
using System.Collections;
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
				RaycastHit rch;
				Physics.Linecast (gameObject.transform.position, otherPlayerBody.position, out rch);
				if(rch.collider.gameObject.tag == "Player"){
					otherPlayerBody.AddExplosionForce (ExplosionForce, explosionPoint, Radius, 1, ForceMode.Impulse);
					foreach(DictionaryEntry message in messages){
						string method = message.Key as string;
						float value = (float)message.Value;
						c.SendMessage (method, value);
					}
				}
			}
		}
	}
}


