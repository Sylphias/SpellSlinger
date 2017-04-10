using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Spells
{

	// This is a static sealed class that holds utility methods and should not be inherited by any other classes.
	public sealed class ExplosionUtilities{
		public static void ExplosionScan(IExplosion spell,IDictionary messages,Collider[] colliders, Vector3 explosionPoint){
            foreach (Collider c in colliders) {
                if (c.GetComponent<Rigidbody>() == null) {
                    continue;
                }
                if (c.tag.Equals ("Player")) {
                    Player player = c.GetComponent<Player>();
                    Rigidbody playerBody = c.GetComponent<Rigidbody> ();
                    RaycastHit rch;
                    // Check if the player object is in line of sight of the explosion. if yes apply damage/effects.
					Physics.Linecast (explosionPoint, playerBody.position, out rch);// Check if RCH has any return values
					if (rch.collider.tag == "Player") { 
						AddExplosionForceToPlayer (player, explosionPoint, spell);
						foreach (DictionaryEntry message in messages) {
							string method = message.Key as string;
							float value = (float)message.Value;	
							UpdatePlayerWithMessage (player, method, value);
						}
					}
                }
            }
        }

		private static void AddExplosionForceToPlayer(Player player, Vector3 explosionPoint,IExplosion spell){
			player.RpcExplosionKnockback (explosionPoint,spell.ExplosionForce,spell.Radius);
        }

        // This method will send back the effects to be called on the client
        private static void UpdatePlayerWithMessage(Player player, string method, float value)
        {
            System.Type type = player.GetType();
            object[] values = { value };
            MethodInfo meth = type.GetMethod(method);
            meth.Invoke(player,values);
        }
    }

}