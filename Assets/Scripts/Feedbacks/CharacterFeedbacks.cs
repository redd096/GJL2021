using UnityEngine;

public class CharacterFeedbacks : MonoBehaviour
{
    [Header("Animator Animations - if not setted get in children")]
    [SerializeField] Animator anim;
    [SerializeField] float minSpeedToStartRun = 0.01f;

    [Header("Sprite in Order when rotate left - if not setted get in children")]
    [SerializeField] SpriteRenderer spriteToChange = default;
    [SerializeField] int spriteInOrder = default;

    Character character;
    int defaultSpriteInOrder;

    void Start()
    {
        //get references
        character = GetComponent<Character>();

        //be sure is setted animator
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        //be sure is setted sprite to change
        if (spriteToChange == null) 
            spriteToChange = GetComponentInChildren<SpriteRenderer>();

        //and save its default order in layer
        defaultSpriteInOrder = spriteToChange.sortingOrder;
    }

    void Update()
    {
        //rotate left or right
        if (character.DirectionAim.x < 0 && transform.localScale.x >= 0)
            RotateObject(false);
        else if (character.DirectionAim.x > 0 && transform.localScale.x <= 0)
            RotateObject(true);

        //set if running or idle
        if (character.Rb.velocity.magnitude > minSpeedToStartRun && anim.GetBool("Running") == false)
            SetRun(true);
        else if (character.Rb.velocity.magnitude <= minSpeedToStartRun && anim.GetBool("Running"))
            SetRun(false);

    }

    #region private API

    void RotateObject(bool toRight)
    {
        //rotate right and reset order in layer
        if (toRight)
        {
            transform.localScale = Vector3.one;
            spriteToChange.sortingOrder = defaultSpriteInOrder;
        }
        //rotate left and set new order in layer
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);

            defaultSpriteInOrder = spriteToChange.sortingOrder;     //save default order in layer
            spriteToChange.sortingOrder = spriteInOrder;
        }
    }

    void SetRun(bool isRunning)
    {
        //set running or idle animation
        anim.SetBool("Running", isRunning);
    }

    #endregion
}
