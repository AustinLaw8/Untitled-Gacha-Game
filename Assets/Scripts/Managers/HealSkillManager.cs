using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkillManager : MonoBehaviour
{
    [SerializeField] HealthManager healthManager;
    float timeElapsed;
    int healAmt;
    int healInterval = 10;
    void Start()
    {
        timeElapsed = 0f;
        healAmt = 10; //TO DO: will be determined by team formation
    }

    void Update()
    {
        if (timeElapsed <= healInterval)
        {
            healthManager.IncreaseHealth(healAmt);
            timeElapsed = 0f;
        }
        else
        {
            timeElapsed += Time.deltaTime;
        }
    }
}
