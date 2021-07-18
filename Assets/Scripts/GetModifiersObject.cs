using UnityEngine;
using redd096;

public class GetModifiersObject : MonoBehaviour
{
    [Header("State Machine - if not setted try in this object (not childs)")]
    [SerializeField] Animator stateMachine = default;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] bool isFrozen;

    Shield shield;

    //modifiers
    public System.Action<bool> onFrozen { get; set; }

    void Start()
    {
        //get references
        if (stateMachine == null)
            stateMachine = GetComponent<Animator>();

        shield = GetComponentInChildren<Shield>();
    }

    #region IGetModifiers

    /// <summary>
    /// Return if can apply modifiers (do not hit shield or ignore it)
    /// </summary>
    /// <param name="ignoreShield"></param>
    /// <param name="hitPosition"></param>
    /// <returns></returns>
    public bool CanApplyModifiers(bool ignoreShield, Vector2 hitPosition)
    {
        //do nothing if hit shield (at least ignoreShield is true)
        if (shield && shield.HitShield(hitPosition) && ignoreShield == false)
            return false;

        return true;
    }

    /// <summary>
    /// Hitted by Ice Shot
    /// </summary>
    /// <param name="activateFrozen"></param>
    public void GetFrozen(bool activateFrozen)
    {
        //do nothing if don't change state
        if (isFrozen == activateFrozen)
            return;

        //set if frozen
        isFrozen = activateFrozen;

        //set statemachine to frozen
        if (stateMachine)
        {
            if (isFrozen)
                stateMachine.SetTrigger("Frozen");
            else
                stateMachine.SetTrigger("Next State");
        }

        //call event
        onFrozen?.Invoke(isFrozen);
    }

    #endregion
}
