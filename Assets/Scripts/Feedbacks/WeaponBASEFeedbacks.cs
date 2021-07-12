using UnityEngine;

public abstract class WeaponBASEFeedbacks : MonoBehaviour
{
    [Header("Offset from center of player")]
    [SerializeField] Vector2 offset = Vector2.zero;
    [SerializeField] bool moveBasedOnOffset = true;

    [Header("Rotate Sprite (if not setted, rotate all)")]
    [SerializeField] Transform objectToRotate = default;

    WeaponBASE weaponBASE;

    protected virtual void OnEnable()
    {
        //get references
        weaponBASE = GetComponent<WeaponBASE>();

        //add events
        if(weaponBASE)
        {
            weaponBASE.onPickWeapon += OnPickWeapon;
        }

        //by default rotate all the object
        if (objectToRotate == null)
            objectToRotate = transform;
    }

    protected virtual void OnDisable()
    {
        //remove events
        if (weaponBASE)
        {
            weaponBASE.onPickWeapon -= OnPickWeapon;
        }
    }

    void Update()
    {
        //rotate weapon with aim
        RotateWeapon();
    }

    #region private API

    void OnPickWeapon()
    {
        //set local scale (to rotate left or right)
        transform.localScale = Vector3.one;
    }

    void RotateWeapon()
    {
        //rotate weapon with aim
        if (weaponBASE.Owner)
        {
            bool isLookingRight = weaponBASE.Owner.DirectionAim.x > 0;

            float angle = Vector2.SignedAngle(isLookingRight ? Vector2.right : Vector2.left, weaponBASE.Owner.DirectionAim);
            objectToRotate.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //rotate also offset and use it to set new position
            if (moveBasedOnOffset)
                objectToRotate.localPosition = Quaternion.AngleAxis(angle, isLookingRight ? Vector3.forward : Vector3.back) * offset;
        }
    }

    #endregion
}
