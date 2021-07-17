using UnityEngine;
using redd096;

public class WeaponRangeFeedbacks : WeaponBASEFeedbacks
{
    [Header("On Instantiate Bullet")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnInstantiateBullet = default;
    [SerializeField] ParticleSystem particlesOnInstantiateBullet = default;
    [SerializeField] AudioStruct audioOnInstantiateBullet = default;

    [Header("On Shoot - main barrel by default is transform")]
    [SerializeField] Transform mainBarrel = default;
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnShoot = default;
    [SerializeField] ParticleSystem particlesOnShoot = default;
    [SerializeField] AudioStruct audioOnShoot = default;

    [Header("On Shoot (camera shake)")]
    [SerializeField] bool cameraShake = true;
    [CanShow("cameraShake")] [SerializeField] bool customShake = false;
    [CanShow("cameraShake", "customShake")] [SerializeField] float shakeDuration = 1;
    [CanShow("cameraShake", "customShake")] [SerializeField] float shakeAmount = 0.7f;

    WeaponRange weaponRange;

    protected override void OnEnable()
    {
        base.OnEnable();

        //get references
        weaponRange = GetComponent<WeaponRange>();
        if (mainBarrel == null)
            mainBarrel = transform;

        //add events
        if(weaponRange)
        {
            weaponRange.onInstantiateBullet += OnInstantiateBullet;
            weaponRange.onShoot += OnShoot;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        //remove events
        if (weaponRange)
        {
            weaponRange.onInstantiateBullet -= OnInstantiateBullet;
            weaponRange.onShoot -= OnShoot;
        }
    }

    void OnInstantiateBullet(Transform barrel)
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnInstantiateBullet, barrel.position, barrel.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right and set parent
            instantiatedGameObject.transform.localScale = transform.lossyScale;
            instantiatedGameObject.transform.SetParent(transform);
        }
        ParticlesManager.instance.Play(particlesOnInstantiateBullet, barrel.position, barrel.rotation);
        SoundManager.instance.Play(audioOnInstantiateBullet.audioClip, barrel.position, audioOnInstantiateBullet.volume);
    }

    void OnShoot()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnShoot, mainBarrel.position, mainBarrel.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right and set parent
            instantiatedGameObject.transform.localScale = transform.lossyScale;
            instantiatedGameObject.transform.SetParent(transform);
        }
        ParticlesManager.instance.Play(particlesOnShoot, mainBarrel.position, mainBarrel.rotation);
        SoundManager.instance.Play(audioOnShoot.audioClip, mainBarrel.position, audioOnShoot.volume);

        //camera shake
        if (cameraShake)
        {
            //custom or default
            if(customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }
}
