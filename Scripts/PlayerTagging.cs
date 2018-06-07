using UnityEngine;
using UnityEngine.Networking;

public class PlayerTagging : NetworkBehaviour
{
    [SyncVar(hook = "isitchanged")] public bool isit = false;
    [SyncVar(hook = "playerwastagged")] public string playertagged;
    [SerializeField] Transform firePosition;
    public float shotdistance;
    bool canShoot;


    void Start()
    {
        if (isLocalPlayer)
            canShoot = true;
    }

    void Update()
    {
        if (!canShoot)
            return;
        if (Input.GetButton("Fire1"))
        {
            if (isit)
            {
                CmdFireShot(firePosition.position, firePosition.forward);
            }
        }
    }
    [Command]
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * shotdistance, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, shotdistance);

        if (result)
        {
            HandleTagged enemy = hit.transform.GetComponent<HandleTagged>();

            if (enemy != null && enemy.taggable)
            {
                playertagged = enemy.name;
                Alertallplayers(playertagged);
                enemy.TakeDamage();

            }
        }
    }
    void isitchanged(bool value)
    {
        isit = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("itstart") && !other.GetComponent<StartPlayerAsIt>().playerisit)
        {
            other.GetComponent<StartPlayerAsIt>().playerisit = true;
            isit = true;
            GetComponent<HandleTagged>().CanWin = false;
            GetComponent<HandleTagged>().taggable = false;
            GetComponent<HandleTagged>().Status.text = "You are it!";
        }
    }
    void playerwastagged(string value)
    {
        playertagged = value;

        GetComponent<HandleTagged>().StatusUpdate.text = playertagged + " was tagged!";
        GetComponent<HandleTagged>().Invoke("clearstatus", 3.0f);
    }
    void Alertallplayers(string name)
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.name != name)
            {
                p.GetComponent<PlayerTagging>().playertagged = (name);
            }
        }
    }
}