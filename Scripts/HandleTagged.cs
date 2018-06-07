using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleTagged : NetworkBehaviour
{

    [SyncVar(hook = "isitchanged")] public bool isit = false;
    [SyncVar(hook = "playerhaswon")] public bool taggable = true;
    [SyncVar(hook = "CanPlayerWin")] public bool CanWin = true;
    [SyncVar(hook = "HasPlayerWon")] public bool HasWon = false;
    public Text StatusUpdate;
    public Text Status;

    Player player;


    void Awake()
    {
        player = GetComponent<Player>();
    }

    [ServerCallback]
    void OnEnable()
    {

    }
    private void Start()
    {
        StatusUpdate.text = "";
        StatusUpdate.color = Color.red;
        Status.text = "Don't Get Caught!";
        if (isit == true)
        {
            Status.text = "You are it!";
        }
    }
    private void Update()
    {

    }
    [Server]
    public bool TakeDamage()
    {
        player.GetComponent<PlayerTagging>().isit = true;
        CanWin = false;
        taggable = false;
        RpcTakeDamage(true);
        Cmdcheckwin();
        return true;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        player.GetComponent<PlayerTagging>().isit = true;
        StatusUpdate.text = "You were Tagged!";
        Status.text = "You are it!";
        Invoke("clearstatus", 3.0f);
    }

    void isitchanged(bool value)
    {
        isit = value;
        if (value == true)
        {
            CanWin = false;
            Status.text = "You are it!";
        }
    }
    void playerhaswon(bool value)
    {
        taggable = value;
    }
    void CanPlayerWin(bool value)
    {
        CanWin = value;
    }
    void HasPlayerWon(bool value)
    {
        HasWon = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination") && CanWin)
        {
            HasWon = true;
            StatusUpdate.text = "You Win!";
            Status.text = "You won! waiting for game to finish...";
            taggable = false;
            Invoke("clearstatus", 3.0f);
            if (isLocalPlayer)
            {
                Cmdcheckwin();
            }
        }
    }
    public void clearstatus()
    {
        StatusUpdate.text = "";
    }
    [Command]
    void Cmdcheckwin()
    {
        player.Invoke("Won", 5.0f);
    }
}
