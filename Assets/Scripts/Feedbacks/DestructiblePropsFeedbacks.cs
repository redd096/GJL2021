using System.Collections;
using UnityEngine;

public class DestructiblePropsFeedbacks : MonoBehaviour
{
    [Header("Sprite to blink - if not setted get in children")]
    [SerializeField] SpriteRenderer spriteToChange = default;

    [Header("On Get Damage")]
    [SerializeField] float durationBlink = 0.2f;
    [SerializeField] bool ignoreIfAlreadyBlinking = true;

    DestructibleProps props;
    Animator anim;

    Coroutine blinkCoroutine;

    void OnEnable()
    {
        //get references
        props = GetComponent<DestructibleProps>();
        anim = GetComponentInChildren<Animator>();

        //be sure is setted sprite to change
        if (spriteToChange == null)
            spriteToChange = GetComponentInChildren<SpriteRenderer>();

        //add events
        if (props)
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

    #region private API

    void OnGetDamage()
    {
        //blink sprite
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(BlinkCoroutine());
        }
        //if already blinking, reset timer if necessary
        else if (ignoreIfAlreadyBlinking == false)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkCoroutine());
        }
    }

    void OnDie()
    {
        //start die animation
        anim.SetTrigger("Die");
    }

    IEnumerator BlinkCoroutine()
    {
        //set blink
        spriteToChange.material.SetFloat("_FlashAmount", 1);

        //wait
        yield return new WaitForSeconds(durationBlink);

        //reset sprite color
        spriteToChange.material.SetFloat("_FlashAmount", 0);

        blinkCoroutine = null;
    }

    #endregion
}
