using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftFootSteps : MonoBehaviour {
    public CharacterController m_CharacterController;
    public AudioSource m_AudioSource;
    [SerializeField] private AudioClip[] m_FootstepSounds;
    float LastStep = 0;
    // Use this for initialization
    void Start ()
    {
        //m_CharacterController = GetComponentInParent<CharacterController>();
        //m_AudioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        if (enabled)
        { if (collision.collider.CompareTag("floor"))
            {
                if (Time.time >= LastStep + .1f)
                {
                    LastStep = Time.time;
                    if (!m_CharacterController.isGrounded)
                    {
                        Debug.Log("falsepositive");
                        return;             
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
            }
        }
        }
}
