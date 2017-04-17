using System;
using UnityEngine;
using UnityEngine.UI;
namespace Spells{
	public interface ISpell
	{
		float Cooldown { get; set; }
		float Damage{ get; set; }
		string GetSpellType{get;}
		void Init ();
	}
}

