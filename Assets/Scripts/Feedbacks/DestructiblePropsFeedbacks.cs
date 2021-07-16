using System.Collections;
using UnityEngine;

public class DestructiblePropsFeedbacks : MonoBehaviour
{
    [Header("Sprite to blink - if not setted get in children")]
    [SerializeField] SpriteRenderer spriteToChange = default;

    [Header("On Get Damage")]
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float durationBlink = 0.2f;
    [SerializeField] bool ignoreIfAlreadyBlinking = true;

    BASEDestructibleProps props;
    Animator anim;

    Material defaultMaterial;
    Coroutine blinkCoroutine;

    void OnEnable()
    {
        //get references
        props = GetComponent<BASEDestructibleProps>();
        anim = GetComponentInChildren<Animator>();

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

    void Start()
    {
        //be sure is setted sprite to change
        if (spriteToChange == null)
            spriteToChange = GetComponentInChildren<SpriteRenderer>();

        //get references
        defaultMaterial = spriteToChange.material;
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
        //check in case spriteRenderer is destroyed
        if (spriteToChange)
        {
            //set blink
            spriteToChange.material = blinkMaterial;

            //wait
            yield return new WaitForSeconds(durationBlink);

            //reset sprite color
            spriteToChange.material = defaultMaterial;
        }

        blinkCoroutine = null;
    }

    #endregion
}
