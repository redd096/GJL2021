using UnityEngine;

public class GetModifiersObject : MonoBehaviour, IGetModifiers
{
    [Header("State Machine - if not setted try in this object (not childs)")]
    [SerializeField] Animator stateMachine = default;

    //modifiers
    public System.Action<bool> onFrozen { get; set; }

    void Start()
    {
        //get references
        if (stateMachine == null)
            stateMachine = GetComponent<Animator>();
    }

    #region IGetModifiers

    /// <summary>
    /// Hitted by Ice Shot
    /// </summary>
    /// <param name="activateFrozen"></param>
    public void GetFrozen(bool activateFrozen)
    {
        //set statemachine to frozen
        if (stateMachine)
        {
            if (activateFrozen)
                stateMachine.SetTrigger("Frozen");
            else
                stateMachine.SetTrigger("Next State");
        }

        //call event
        onFrozen?.Invoke(activateFrozen);
    }

    #endregion
}
