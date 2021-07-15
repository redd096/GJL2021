using UnityEngine;
using redd096;

public abstract class WeaponBASEFeedbacks : MonoBehaviour
{
    [Header("Offset from center of player")]
    [SerializeField] Vector2 offset = Vector2.zero;

    [Header("Rotate Sprite (if not setted, rotate all)")]
    [SerializeField] Transform objectToRotate = default;

    [Header("On Drop")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnDrop = default;
    [SerializeField] ParticleSystem particlesOnDrop = default;
    [SerializeField] AudioStruct audioOnDrop = default;

    [Header("On Drop Camera Shake")]
    [SerializeField] bool cameraShakeOnDrop = true;
    [CanShow("cameraShakeOnDrop")] [SerializeField] bool customShakeOnDrop = false;
    [CanShow("cameraShakeOnDrop", "customShakeOnDrop")] [SerializeField] float shakeDurationOnDrop = 1;
    [CanShow("cameraShakeOnDrop", "customShakeOnDrop")] [SerializeField] float shakeAmountOnDrop = 0.7f;

    WeaponBASE weaponBASE;

    protected virtual void OnEnable()
    {
        //get references
        weaponBASE = GetComponent<WeaponBASE>();

        //add events
        if(weaponBASE)
        {
            weaponBASE.onPickWeapon += OnPickWeapon;
            weaponBASE.onDropWeapon += OnDropWeapon;
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
            weaponBASE.onDropWeapon -= OnDropWeapon;
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

    void OnDropWeapon()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnDrop, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnDrop, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnDrop.audioClip, transform.position, audioOnDrop.volume);

        //camera shake
        if (cameraShakeOnDrop)
        {
            //custom or default
            if (customShakeOnDrop)
                CameraShake.instance.StartShake(shakeDurationOnDrop, shakeAmountOnDrop);
            else
                CameraShake.instance.StartShake();
        }
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
            objectToRotate.localPosition = Quaternion.AngleAxis(angle, isLookingRight ? Vector3.forward : Vector3.back) * offset;
        }
    }

    #endregion
}
