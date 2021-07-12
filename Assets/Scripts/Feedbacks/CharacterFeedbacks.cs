using UnityEngine;

public class CharacterFeedbacks : MonoBehaviour
{
    [Header("Change Sprite in Order when Rotate")]
    [SerializeField] SpriteRenderer spriteToChange = default;
    [SerializeField] int spriteInOrder = default;

    Character character;
    int defaultSpriteInOrder;

    void Start()
    {
        //get references
        character = GetComponent<Character>();

        //be sure is setted sprite to change
        if (spriteToChange == null) 
            spriteToChange = GetComponentInChildren<SpriteRenderer>();

        //and save its default order in layer
        defaultSpriteInOrder = spriteToChange.sortingOrder;
    }

    void Update()
    {
        //rotate left or right
        if (character.DirectionAim.x < 0 && transform.localScale.x > 0)
            RotateObject(false);
        else if (character.DirectionAim.x > 0 && transform.localScale.x < 0)
            RotateObject(true);
    }

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
}
