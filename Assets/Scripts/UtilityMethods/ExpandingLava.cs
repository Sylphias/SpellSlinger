using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingLava : MonoBehaviour {
	private float rate;
	private float timeElapsed;
	// Update is called once per frame
	void Start(){
		rate = 0;
	}
	void FixedUpdate () {
		timeElapsed += Time.deltaTime;

		if (timeElapsed > 60 && timeElapsed<=140) 
			rate = 0.2f * Time.deltaTime;
		else if (timeElapsed > 140 && timeElapsed<=160) 
			rate = 0.5f * Time.deltaTime;
		else if (timeElapsed > 160)
			rate = 3f * Time.deltaTime;
			
		transform.localScale += new Vector3 (rate, 0, rate);
	}

}
