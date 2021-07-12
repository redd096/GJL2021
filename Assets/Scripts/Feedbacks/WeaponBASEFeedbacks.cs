using UnityEngine;

public class WeaponBASEFeedbacks : MonoBehaviour
{
    [Header("Rotate Sprite (if not setted, rotate all)")]
    [SerializeField] Transform objectToRotate = default;

    WeaponBASE weaponBASE;

    void OnEnable()
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

    void OnDisable()
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
        //set owner local scale (to rotate left or right)
        transform.localScale = weaponBASE.Owner.transform.localScale;
    }

    void RotateWeapon()
    {
        //rotate weapon with aim
        if (weaponBASE.Owner)
        {
            float angle = Vector2.SignedAngle(Vector2.right, weaponBASE.Owner.DirectionAim);
            objectToRotate.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            objectToRotate.localPosition = Quaternion.AngleAxis(angle, Vector3.forward) * weaponBASE.Offset;    //rotate also offset and use it to set new position
        }
    }

    #endregion
}
