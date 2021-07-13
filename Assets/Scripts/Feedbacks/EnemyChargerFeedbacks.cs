using UnityEngine;

public class EnemyChargerFeedbacks : MonoBehaviour
{
    [Header("Animator Animations - if not setted get in children")]
    [SerializeField] Animator anim;

    Enemy enemy;

    void OnEnable()
    {
        //get references
        enemy = GetComponent<Enemy>();

        //add events
        if (enemy)
        {
            enemy.onNextState += OnNextState;
        }

        //be sure is setted animator
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    void OnDisable()
    {
        //remove events
        if (enemy)
        {
            enemy.onNextState -= OnNextState;
        }
    }

    void OnNextState()
    {
        //set next state
        anim.SetTrigger("Next State");
    }
}
