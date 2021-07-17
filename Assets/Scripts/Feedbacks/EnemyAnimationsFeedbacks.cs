using UnityEngine;

public class EnemyAnimationsFeedbacks : MonoBehaviour
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
            enemy.onTargetSetted += OnTargetSetted;
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
            enemy.onTargetSetted -= OnTargetSetted;
        }
    }

    void OnNextState()
    {
        //set next state
        anim.SetTrigger("Next State");
    }

    public void OnTargetSetted(bool isSetted)
    {
        //set bool target setted
        anim.SetBool("Target Setted", isSetted);
    }
}
