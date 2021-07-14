using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePropsFeedbacks : MonoBehaviour
{
    DestructibleProps props;

    void OnEnable()
    {
        //get references
        props = GetComponent<DestructibleProps>();

        //add events
        if(props)
        {
            props.onGetDamage += OnGetDamage;
            props.onDie += OnDie;
        }
    }

    void OnDisable()
    {
        //remove events
        if (props)
        {
            props.onGetDamage += OnGetDamage;
            props.onDie += OnDie;
        }
    }

    void OnGetDamage()
    {
        //TODO
        //blinka
    }

    void OnDie()
    {
        //TODO
        //animazione di esplosione
    }
}
