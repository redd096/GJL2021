﻿using UnityEngine;
using redd096;

public class FrozenModifier : MonoBehaviour
{
    [Header("Ice")]
    [SerializeField] float duration = 2;
    [ReadOnly] [SerializeField] float remainingTime;

    float timer = 0;
    IGetModifiers frozenObject;

    public void Init(float duration)
    {
        this.duration = duration;

        //set timer
        OnStartTimer();
    }

    void Update()
    {
        //debug
        remainingTime = timer - Time.time;

        //on finish timer
        if(timer > 0 && Time.time > timer)
        {
            OnFinishTImer();
        }
    }

    void OnStartTimer()
    {
        //set timer
        timer = Time.time + duration;

        //and set frozen
        frozenObject = GetComponentInParent<IGetModifiers>();
        frozenObject?.GetFrozen(true);
    }

    void OnFinishTImer()
    {
        //reset timer
        timer = 0;

        //and set frozen
        frozenObject?.GetFrozen(false);
        frozenObject = null;

        //remove this component
        Destroy(this);
    }
}
