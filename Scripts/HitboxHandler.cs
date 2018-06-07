using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HitboxHandler : NetworkBehaviour
{
    NetworkAnimator anim;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<NetworkAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.animator.GetBool("IsCrouching"))
        {
            GetComponent<CapsuleCollider>().height = .75f;
            GetComponent<CapsuleCollider>().radius = .4f;
            GetComponent<CharacterController>().height = .75f;
            GetComponent<CharacterController>().radius = .4f;
            Vector3 oldcenter = GetComponent<CharacterController>().center;
            Vector3 newcenter = new Vector3(oldcenter.x, .5f, oldcenter.z);
            GetComponent<CharacterController>().center = newcenter;
            GetComponent<CapsuleCollider>().center = newcenter;
        }
        if (!anim.animator.GetBool("IsCrouching"))
        {
            GetComponent<CapsuleCollider>().height = 1.4f;
            GetComponent<CapsuleCollider>().radius = .19f;
            GetComponent<CharacterController>().height = 1.4f;
            GetComponent<CharacterController>().radius = .19f;
            Vector3 oldcenter = GetComponent<CharacterController>().center;
            Vector3 newcenter = new Vector3(oldcenter.x, .8f, oldcenter.z);
            GetComponent<CharacterController>().center = newcenter;
            GetComponent<CapsuleCollider>().center = newcenter;
        }
    }
}

