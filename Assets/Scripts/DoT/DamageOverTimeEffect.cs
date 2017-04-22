using System;
using UnityEngine;
namespace Spells
{
	public class DamageOverTimeEffect:MonoBehaviour
	{
		public GameObject player;
		void FixedUpdate(){
			if(player != null)
				transform.position = player.transform.position;
		}
	}
}

