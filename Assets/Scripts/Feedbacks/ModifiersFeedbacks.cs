using UnityEngine;

public class ModifiersFeedbacks : MonoBehaviour
{
    [Header("Animator and SpriteRenderer - if not setted get in children")]
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer spriteToChange = default;

    [Header("On Frozen")]
    [SerializeField] Color colorOnFrozen = Color.cyan;

    IGetModifiers modiferObject;

    Color defaultColor;

    void OnEnable()
    {
        //get references
        modiferObject = GetComponent<IGetModifiers>();

        //add events
        if(modiferObject != null)
        {
            modiferObject.onFrozen += OnFrozen;
        }
    }

    void OnDisable()
    {
        //remove events
        if (modiferObject != null)
        {
            modiferObject.onFrozen -= OnFrozen;
        }
    }

    void Start()
    {
        //be sure is setted animator
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        //be sure is setted sprite to change
        if (spriteToChange == null)
            spriteToChange = GetComponentInChildren<SpriteRenderer>();

        //save default color
        if (spriteToChange)
            defaultColor = spriteToChange.color;
    }

    #region modifiers

    void OnFrozen(bool activateFrozen)
    {
        //set animator
        if (anim)
        {
            if (activateFrozen)
            {
                anim.SetTrigger("Frozen");
            }
            else
            {
                anim.SetTrigger("Next State");
            }
        }

        //change sprite color
        if(spriteToChange)
            spriteToChange.color = activateFrozen ? colorOnFrozen : defaultColor;
    }

    #endregion
}
