﻿using System;
using UnityEngine;
namespace Spells
{
	public sealed class SpellModifier
	{
		// We can do this because objects are pass by reference in C#.
		public static void ModifySpell(ISpell spell,GameObject player, float damageModifier, float projectileSpeedModifier, float radiusModifier, float forceModifier)
		{
			
			switch (spell.GetSpellType) {
			case "projectile":
				ProjectileSpell pSpell = spell as ProjectileSpell;	
				pSpell.Damage *= damageModifier;
				pSpell.ProjectileSpeed *= projectileSpeedModifier;
				pSpell.Radius *= radiusModifier;
				pSpell.ExplosionForce *= forceModifier;
				break;
			case "nova":
				Debug.Log ("nova");
				NovaSpell nSpell = spell as NovaSpell;					
				nSpell.Radius *= radiusModifier;
				nSpell.ExplosionForce *= forceModifier;
				nSpell.Damage *= damageModifier;
				break;
			case "buff":
				Debug.Log ("buff");
				BuffSpell bSpell = spell as BuffSpell;
				bSpell.Player = player;
				break;
			}

		}


	}
}
