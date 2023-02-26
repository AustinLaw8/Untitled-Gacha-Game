using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates and maintains a pool of particle systems for when player interacts with a note
public class ParticleManager : MonoBehaviour
{
    public static ParticleManager particleManager { get; private set;  }

    private static int POOL_SIZE=10;
    private static float TAP_EMISSION_TIME = .25f;

    [SerializeField] public GameObject targetParticleSystem;

    private GameObject[] particles;
    private bool[] bound = {false, false};
    protected Transform lane;

    void Awake()
    {
        if (particleManager != null && particleManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            particleManager = this;
        }
    }

    void Start()
    {
        lane = GameObject.Find("Lane").transform;
        particles = new GameObject[POOL_SIZE];
        for(int i = 0; i < POOL_SIZE; i++)
        {
            particles[i] = GameObject.Instantiate(targetParticleSystem,this.transform);
        }
    }
    
    /**
     * Interface for notes to tell particle manager to play a particle
     * This interface is for tap and flick notes, who just need the particles played at a certain location once
     */
    public void EmitParticlesOnPress(Vector3 location)
    {
        ParticleSystem currentParticles = GetAvailableParticles();
        currentParticles.transform.position = new Vector3(
            location.x,
            lane.position.y,
            0f
        );
        currentParticles.Play();
        StartCoroutine(PauseParticles(currentParticles, TAP_EMISSION_TIME));
    }

    /**
     * Interface for hold notes
     * Hold notes have special interactions, where they need the particle system for longer
     * And may have to turn the particles on or off
     * Instead of constantly telling the particle manager to move the particle system and turn them on or off,
     * hand off the particle system to the note itself
     */
    public ParticleSystem AttachParticlesForHold(Transform note)
    {
        ParticleSystem particles = GetAvailableParticles();
        particles.transform.SetParent(note);
        return particles;
    }

    /**
     * Once the hold note is finished with the particle system, it should return it back to the manager
     * This essentially like freeing memory in C/C++; the hold note asks for memory and has to return it back
     */
    public void DetachParticlesForHold(ParticleSystem particles)
    {
        particles.transform.SetParent(this.transform);
        particles.gameObject.SetActive(false);
    }

    // Gets the first available particle system
    private ParticleSystem GetAvailableParticles()
    {
        foreach(GameObject obj in particles)
        {
            if(!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj.GetComponent<ParticleSystem>();
            }
        }
        return null;
        // Debug.LogWarning("No free particle system");
    }

    private IEnumerator PauseParticles(ParticleSystem particles, float time=.2f)
    {
        yield return new WaitForSeconds(time);
        particles.gameObject.SetActive(false);
        particles.Stop();
    }

    // public void 
}
