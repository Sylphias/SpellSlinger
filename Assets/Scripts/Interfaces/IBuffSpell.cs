using System;

namespace Spells
{
	public interface IBuffSpell
	{
		float Duration{get;set;}
		float Cooldown{ get; set; }
	}
}

