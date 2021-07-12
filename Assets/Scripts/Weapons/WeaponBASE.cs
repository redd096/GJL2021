using UnityEngine;
using redd096;

public abstract class WeaponBASE : MonoBehaviour
{
    [Header("DEBUG")]
    [ReadOnly] public Character Owner;

    //events
    public System.Action onPickWeapon { get; set; }

    #region public API

    /// <summary>
    /// Set owner to look at - and set parent
    /// </summary>
    /// <param name="character"></param>
    public void PickWeapon(Character character)
    {
        Owner = character;

        //set parent
        transform.SetParent(Owner.transform);

        //call event
        onPickWeapon?.Invoke();
    }

    /// <summary>
    /// Remove owner and remove parent
    /// </summary>
    public void DropWeapon()
    {
        Owner = null;

        //remove parent
        transform.SetParent(null);
    }

    #endregion

    #region abstracts

    public abstract void PressAttack();
    public abstract void ReleaseAttack();

    #endregion
}
