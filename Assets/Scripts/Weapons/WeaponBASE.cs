using UnityEngine;

public class WeaponBASE : MonoBehaviour
{
    [Header("Base Weapon")]
    [SerializeField] Vector2 offset = Vector2.zero;

    protected Player owner;

    #region public API

    public void PickWeapon(Player player)
    {
        owner = player;

        //set parent
        transform.SetParent(owner.transform);
    }

    public void DropWeapon()
    {
        owner = null;
    }

    #endregion
}
