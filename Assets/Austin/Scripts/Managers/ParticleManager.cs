using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ParticleSystem AttachParticlesForHold(Transform note)
    {
        ParticleSystem particles = GetAvailableParticles();
        particles.transform.SetParent(note);
        return particles;
    }

    public void DetachParticlesForHold(ParticleSystem particles)
    {
        particles.transform.SetParent(this.transform);
        particles.gameObject.SetActive(false);
    }

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
