using UnityEngine;
using System.Collections;
namespace Spells
{
	// This is a static sealed class that holds utility methods and should not be inherited by any other classes.
	public sealed class ExplosionUtilities{
		public static void ExplosionScan(IExplosion spell,IDictionary messages,Collider[] colliders, Vector3 explosionPoint, bool ignorePlayer= false){
            foreach (Collider c in colliders) {
                if (c.GetComponent<Rigidbody>() == null) {
                    continue;
                }
                if (c.tag.Equals ("Player")) {
                    PlayerHit playerHit = c.GetComponent<PlayerHit>();
                    Rigidbody playerBody = c.GetComponent<Rigidbody> ();
                    RaycastHit rch;
                    // Check if the player object is in line of sight of the explosion. if yes apply damage/effects.
					if (!Physics.Linecast (explosionPoint, playerBody.position, out rch))
						continue;// Check if RCH has any return values, if no then go to the next iteration
					bool x = rch.distance > (!ignorePlayer?0:2);
					Debug.DrawLine (playerBody.position, explosionPoint, Color.red,4);
					if (rch.collider.tag == "Player" && x ) { 
							playerHit.ApplyExplosiveKnockback (explosionPoint,spell.ExplosionForce,spell.Radius);
                            playerHit.OnHit(messages);
                    }
                }
            }
        }
    }
}