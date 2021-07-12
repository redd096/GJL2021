using UnityEngine;

public class CharacterFeedbacks : MonoBehaviour
{
    Character character;

    void Start()
    {
        //get references
        character = GetComponent<Character>();
    }

    void Update()
    {
        //rotate left or right
        if (character.DirectionAim.x < 0 && transform.localScale.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (character.DirectionAim.x > 0 && transform.localScale.x < 0)
            transform.localScale = Vector3.one;
    }
}
