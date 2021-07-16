using System.Collections;
using UnityEngine;

public class TrapFeedbacks : MonoBehaviour
{
    [Header("On Hit")]
    [SerializeField] float durationAnimation = 1f;
    [SerializeField] bool ignoreIfAlreadyActive = true;

    PropAndTrap prop;
    Animator anim;

    Coroutine deactiveCoroutine;

    void OnEnable()
    {
        //get references
        prop = GetComponent<PropAndTrap>();
        anim = GetComponentInChildren<Animator>();

        //add events
        if(prop)
        {
            prop.onHit += OnHit;
            prop.onActiveByTimer += OnActiveByTimer;
        }
    }

    void OnDisable()
    {
        //remove events
        if (prop)
        {
            prop.onHit -= OnHit;
            prop.onActiveByTimer -= OnActiveByTimer;
        }
    }

    #region private API

    void OnHit()
    {
        //start coroutine only if already deactivated
        if(deactiveCoroutine == null)
        {
            //active trap
            anim.SetTrigger("Active");

            deactiveCoroutine = StartCoroutine(DeactiveCoroutine());
        }
        //if already active, reset timer if necessary
        else if (ignoreIfAlreadyActive == false)
        {
            StopCoroutine(deactiveCoroutine);
            deactiveCoroutine = StartCoroutine(DeactiveCoroutine());
        }
    }

    void OnActiveByTimer(bool isActive)
    {
        //active or deactive trap
        if (isActive)
            anim.SetTrigger("Active");
        else
            anim.SetTrigger("Deactive");
    }

    IEnumerator DeactiveCoroutine()
    {
        //wait
        yield return new WaitForSeconds(durationAnimation);

        //deactive trap
        if(anim)
            anim.SetTrigger("Deactive");

        deactiveCoroutine = null;
    }

    #endregion
}
