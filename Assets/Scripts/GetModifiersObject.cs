using UnityEngine;
using redd096;

public class GetModifiersObject : MonoBehaviour
{
    [Header("State Machine - if not setted try in this object (not childs)")]
    [SerializeField] Animator stateMachine = default;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] bool isFrozen;
    [ReadOnly] [SerializeField] bool isBurning;

    Shield shield;

    //modifiers
    public System.Action<bool> onFrozen { get; set; }
    public System.Action<bool> onBurn { get; set; }

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
    /// <param name="activateModifier"></param>
    public void GetFrozen(bool activateModifier)
    {
        //do nothing if don't change state
        if (isFrozen == activateModifier)
            return;

        //set state
        isFrozen = activateModifier;

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

    /// <summary>
    /// Hitted by Flame Shot
    /// </summary>
    /// <param name="activateModifier"></param>
    public void GetBurn(bool activateModifier)
    {
        //do nothing if don't change state
        if (isBurning == activateModifier)
            return;

        //set state
        isBurning = activateModifier;

        //call event
        onBurn?.Invoke(isBurning);
    }

    #endregion
}
