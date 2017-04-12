using System;
using UnityEngine;
using System.Collections;

namespace Spells{
public interface IBuffable
	{
		string Type{ get; }
		float ComparableValue{get;}
		float TickTime{get;set;}
		void Apply(Component victim);
		void Reset(Component victim); // Resets player to original state before debuff/buff
		bool Finished { get; }
		float FinishTime{ get; set;}
	}
}


