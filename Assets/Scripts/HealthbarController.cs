using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HealthbarController : NetworkBehaviour {

	public RawImage healthbar;

    private const float maxHealth =100;

    [SyncVar(hook = "OnChangeHealth")]
	private float currHealth ;
	public float CurrentHealth {
		get{ return currHealth; }
		set{ currHealth = value; }
	}

    public HealthbarController(){
        // Player spawned, set health to 100
        currHealth = 100;
    }
    public float recovery = 3;

    void OnChangeHealth(float health) {
        Debug.Log("change healthbar");
        healthbar.rectTransform.localScale = new Vector3(health / maxHealth, 1, 1);		
    }

    private void Start()
    {
        healthbar.rectTransform.localScale = new Vector3(currHealth / maxHealth, 1, 1);
    }


}
