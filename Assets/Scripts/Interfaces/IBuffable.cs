using System;
using UnityEngine;
using System.Collections;

namespace Spells{
public interface IBuffable
	{
		string Type{ get; }
		float ComparableValue{get;}
		float TickTime{get;set;}
		float TimeElapsed{get;set;}
		void Apply(GameObject victim);
		void Reset(GameObject victim); // Resets player to original state before debuff/buff
		bool Finished { get; }
		float FinishTime{ get; set;}
	}
}


