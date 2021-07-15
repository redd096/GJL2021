using System.Collections.Generic;
using UnityEngine;
using redd096;

[RequireComponent(typeof(PropAndTrap))]
public class TimerTrap : MonoBehaviour
{
    [Header("Timer Trap")]
    [SerializeField] float timeToActive = 1;
    [SerializeField] float timeToDeactive = 1;
    [SerializeField] float delayBetweenHits = 0.3f;
    [Space]
    [ReadOnly] [SerializeField] bool isActive = false;
    [ReadOnly] [SerializeField] float remainingTime = 0;

    PropAndTrap prop;
    float timeEndTimer;
    float timeNextHit;

    void Start()
    {
        prop = GetComponent<PropAndTrap>();
    }

    void Update()
    {
        //check when finish timer
        remainingTime = timeEndTimer - Time.time;
        if (Time.time > timeEndTimer)
        {
            FinishTimer();
        }

        //if active, check hits every tot seconds
        if (isActive && Time.time > timeNextHit)
            CheckHits();
    }

    void FinishTimer()
    {
        //change state + set timer
        isActive = !isActive;
        timeEndTimer = Time.time + (isActive ? timeToDeactive : timeToActive);

        //check hits and call event on prop
        prop.ActiveByTimer(isActive);
    }

    void CheckHits()
    {
        //set delay
        timeNextHit = Time.time + delayBetweenHits;

        List<Collider2D> hits = new List<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();

        //foreach collider, overlap and add to hits
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        {
            List<Collider2D> cols = new List<Collider2D>();
            Physics2D.OverlapCollider(collider, contactFilter2D, cols);
            hits.AddRange(cols);
        }

        //damage every game object
        foreach (Collider2D hit in hits)
            prop.OnHit(hit.gameObject, false);
    }
}
