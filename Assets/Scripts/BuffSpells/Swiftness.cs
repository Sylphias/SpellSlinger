using UnityEngine;
using System.Collections;
namespace Spells{
	public class Swiftness : BuffSpell {
		private float speedModifier;

		public float SpeedModifier{
			get{ return speedModifier; }
			set{ speedModifier = value; }
		}

		public void Init(float duration, float cooldown,GameObject player,float speedModifier){
			Duration = duration;
			Cooldown = cooldown;
			Player = player;
			this.speedModifier = speedModifier;
		}

		public override void Init(){
			Cooldown = 10;
		}

		// Use this for initialization
		void Start () {
			gameObject.transform.Rotate (-90, 0, 0);
			GameObject swiftnessEnchantPrefab = Resources.Load ("StormEnchant", typeof(GameObject))as GameObject;
			GameObject go = Instantiate (swiftnessEnchantPrefab,gameObject.transform.position,gameObject.transform.rotation) as GameObject;
			Player.GetComponent<Player>().RpcSwift ();
			Destroy (go,Duration);
			Destroy (gameObject,5);
		}

		// Update is called once per frame
		void Update () {
			gameObject.transform.position = Player.transform.position;
		}
	}
}
