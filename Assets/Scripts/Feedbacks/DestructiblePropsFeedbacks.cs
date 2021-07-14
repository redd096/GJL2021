using UnityEngine;

public class DestructiblePropsFeedbacks : MonoBehaviour
{
    DestructibleProps props;
    Animator anim;

    void OnEnable()
    {
        //get references
        props = GetComponent<DestructibleProps>();
        anim = GetComponentInChildren<Animator>();

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
        //start die animation
        anim.SetTrigger("Die");
    }
}
