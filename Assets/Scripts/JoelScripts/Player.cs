using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerSetup))]
public class Playerz : NetworkBehaviour {

    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    public string username = "Loading...";
    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;
    private bool firstSetup = true;

    public void SetupPlayer()
    {

    }
    private void Awake()
    {
        setDefault();
    }
    public void setDefault()
    {
        isDead = false;
        currentHealth = maxHealth;
    }
    [SyncVar]
    private bool _isDead = false;

    public bool isDead
    {
        get
        {
            return _isDead;
        }
        protected set
        {
            _isDead = value;
        }
    }
    public int kills;
    public int deaths;

    // completed
    [ClientRpc]
    public void RpcTakeDamage(int _damage, string _playerID)
    {
        if (isDead) return;
        currentHealth -= _damage;
        Debug.Log(transform.name + " has taken " + _damage + ", remaining health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die(_playerID);
        }
    }
    private void Die(string _playerID)
    {
        isDead = true;
        Player sourcePlayer = GameManager.GetPlayer(_playerID);
        if (sourcePlayer != null)
        {
            sourcePlayer.Kills++;
        }
        deaths++;
        Debug.Log(transform.name + " is dead.");
    }
}
