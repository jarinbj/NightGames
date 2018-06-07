using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StartPlayerAsIt : NetworkBehaviour
{

    [SyncVar(hook = "playerisitnow")] public bool playerisit = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void playerisitnow(bool value)
    {
        playerisit = value;
    }
}