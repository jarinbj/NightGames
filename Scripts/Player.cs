using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

public class Player : NetworkBehaviour
{
    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] private AudioClip[] m_FootstepSounds;

    [SyncVar(hook = "OnNameChanged")] public string playerName;
    [SyncVar(hook = "OnColorChanged")] public Color playerColor;

    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;
    public float runspeed;
    public float speed;
    public CharacterController m_CharacterController;
    public AudioSource m_AudioSource;
    bool isCrouchTransitionInProgress = false;
    GameObject mainCamera;
    public GameObject playercam;
    NetworkAnimator anim;
    float daspeed;
    bool iscrouching = false;
    public GameObject position;
    public float xposition = 0;
    public float addedy = 0;
    public float addedz;
    float yposition;
    public float originaly = 1.3f;
    public float originalycrouch = .52f;
    bool tagging;
    bool running;
    public int fatigue;
    public int maxfatigue = 100;
    static List<Player> players = new List<Player>();
    float timer;
    bool fatigued = false;
    public Text energystatus;

    void Start()
    {
        anim = GetComponent<NetworkAnimator>();
        mainCamera = Camera.main.gameObject;
        daspeed = speed;
        EnablePlayer();
        yposition = originaly;
        timer = Time.time;
        runspeed = controller.m_RunSpeed;
        OnColorChanged(playerColor);
        OnNameChanged(playerName);
        energystatus.text = "Energy: " + fatigue.ToString();
    }

    [ServerCallback]
    void OnEnable()
    {
        if (!players.Contains(this))
            players.Add(this);
    }

    [ServerCallback]
    void OnDisable()
    {
        if (players.Contains(this))
            players.Remove(this);
    }


    void Update()
    {
        if (!isLocalPlayer)
            return;

        SetAnimSpeed();

        CheckFire();

        CheckCrouch();

        CheckRun();

        UpdateCam();

    }
    void SetAnimSpeed()
    {
        anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));
    }
    void CheckFire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            tagging = true;
            anim.animator.SetBool("Tag", true);
            anim.animator.SetBool("TagEnd", false);
            //xposition = .2f;
            iscrouching = false;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            tagging = false;
            anim.animator.SetBool("Tag", false);
            anim.animator.SetBool("TagEnd", true);
            iscrouching = false;
        }
    }

    void CheckCrouch()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (!tagging)
            {
                iscrouching = true;
                anim.animator.SetBool("IsCrouching", true);
            }

        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            iscrouching = false;
            anim.animator.SetBool("IsCrouching", false);
        }
    }

    void CheckRun()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift) || fatigue <= 0)
        {
            running = false;
            anim.animator.SetBool("IsRunning", false);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (anim.animator.GetFloat("Speed") != 0 || anim.animator.GetFloat("Strafe") != 0)
            {
                if (fatigue > 0 && !fatigued)
                {
                    running = true;
                    anim.animator.SetBool("IsRunning", true);
                    addedz = .2f;
                    if (Time.time >= (timer + 1.0f))
                    {
                        timer = Time.time;
                        fatigue--;
                        energystatus.text = "Energy: " + fatigue.ToString();
                    }
                }
                else
                {
                    fatigued = true;
                    controller.m_RunSpeed = controller.m_WalkSpeed;
                }
            }
        }
        if (!running && fatigue < maxfatigue && Time.time >= (timer + 1.0f))
        {
            timer = Time.time;
            fatigue++;
            energystatus.text = "Energy: " + fatigue.ToString();
            if (fatigued && fatigue >= 5)
            {
                fatigued = false;
                controller.m_RunSpeed = runspeed;
            }
        }
    }

    void UpdateCam()
    {
        if (iscrouching)
        {
            xposition = .2f;
            addedz = .2f;
        }
        if (!iscrouching)
        {
            if (!tagging && !running)
            {
                xposition = 0;
                Invoke("Changez", .1f);
            }
        }
        yposition = position.transform.position.y;
        if (yposition > originaly)
        {
            yposition = originaly;
        }
        if (yposition > originalycrouch && iscrouching)
        {
            yposition = originalycrouch;
        }
        playercam.transform.localPosition = new Vector3(xposition, yposition + addedy, .2f + addedz);

    }
    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        DisablePlayer();
    }
    void DisablePlayer()
    {
        if (isLocalPlayer)
        {
            mainCamera.SetActive(true);
        }

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        if (isLocalPlayer)
        {
            //PlayerCanvas.canvas.Initialize();
            mainCamera.SetActive(false);
        }

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);

        playercam.transform.localPosition += new Vector3(0, 0, .2f);
    }


    private void CamLerpToPosition(Vector3 currentPosition, Vector3 targetPosition)
    {
        playercam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * daspeed);
        daspeed += .5f;
        if (Mathf.Abs(playercam.transform.localPosition.y - targetPosition.y) <= 0.01f)
        {
            isCrouchTransitionInProgress = false;
            daspeed = speed;
        }
    }
    void Changez()
    {
        if (!running)
            addedz = 0f;
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        GetComponentInChildren<Text>(true).text = playerName;
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);
    }

    [Server]
    public void Won()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetComponent<HandleTagged>().HasWon && !players[i].GetComponent<PlayerTagging>().isit)
            {
                return;
            }
        }
        for (int i = 0; i < players.Count; i++)
            players[i].RpcGameOver(netId, name);

        Invoke("BackToLobby", 1f);
    }

    [ClientRpc]
    void RpcGameOver(NetworkInstanceId networkID, string name)
    {
        DisablePlayer();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
    }

    void PlayFootStep()
    {
        if (isLocalPlayer)
            return;
        if (!m_CharacterController.isGrounded)
        {
            Debug.Log("falsepositive");
            //return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);

        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }
    void PlaySideStepLeft()
    {
        if (isLocalPlayer)
            return;
        if (anim.animator.GetFloat("Strafe") < 0 && anim.animator.GetFloat("Speed") == 0)
        {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }
    }
    void PlaySideStepRight()
    {
        if (isLocalPlayer)
            return;
        if (anim.animator.GetFloat("Strafe") > 0 && anim.animator.GetFloat("Speed") == 0)
        {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }
    }

}