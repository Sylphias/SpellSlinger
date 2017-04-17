using UnityEngine.Networking;

public class UIScript : NetworkBehaviour {
    void Awake()
    {
        DontDestroyOnLoad(gameObject.transform);
    }
}
