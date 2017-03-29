using System;
using UnityEngine;
using System.Collections;

namespace Spells{
public interface IBuffable
	{
		string Type{ get; }
		void Apply(Component victim);
		void Reset(Component victim); // Resets player to original state before debuff/buff
		bool Finished { get; }
		float FinishTime{ get; set;}
		float TickTime{ get; set; } // apply debuff every X seconds
	}
}


