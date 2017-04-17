using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
namespace Spells{
	public class Swiftness : BuffSpell,ISpell {
		[SyncVar]
		private float speedModifier;

		public float SpeedModifier{
			get{ return speedModifier; }
			set{ speedModifier = value; }
		}

		public void Init(float duration, float cooldown,float speedModifier){
			Duration = duration;
			Cooldown = cooldown;
			this.speedModifier = speedModifier;
		}

		public override void Init(){
			Duration = 10;
			Cooldown = 10;
			Damage = 0;
		}

		// Use this for initialization
		void Start () {
			gameObject.transform.Rotate (-90, 0, 0);
			GameObject swiftnessEnchantPrefab = Resources.Load ("Spells/StormEnchant", typeof(GameObject))as GameObject;
			GameObject go = Instantiate (swiftnessEnchantPrefab,gameObject.transform.position,gameObject.transform.rotation) as GameObject;
			// Buff self at location
			Collider[] colliders = Physics.OverlapSphere (gameObject.transform.position,4);
			foreach(Collider c in colliders){
				if(c.tag.Equals("Player")){
					c.GetComponent<PlayerHit>().ApplyDebuffBuff("RpcSwift",2f);
				}
			}
			Destroy (go,Duration);
			Destroy (gameObject,Duration);
		}

		// Update is called once per frame
		void Update () {
			gameObject.transform.position = Player.transform.position;
		}
	}
}
