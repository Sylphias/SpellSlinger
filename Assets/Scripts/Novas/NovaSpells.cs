using UnityEngine;
using System.Collections;
namespace Spells{
	public class NovaSpells : MonoBehaviour,INova {

		private float damage, radius, cooldown, explosionForce;

		public float ExplosionForce{
			get{ return explosionForce; }
			set{ explosionForce = value; }
		}
		public float Radius {
			get{ return radius; }
			set{ radius = value; }
		}

		public float Damage {
			get { return damage;}
			set { damage  = value ;}
		}

		public float Cooldown{
			get { return cooldown;}
			set { cooldown = value;}
		}

			public void initialize(float cooldown, float damage, float radius, float explosionForce){
			Cooldown = cooldown;
			Damage = damage;
			Radius = radius;
			ExplosionForce = explosionForce;
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

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}