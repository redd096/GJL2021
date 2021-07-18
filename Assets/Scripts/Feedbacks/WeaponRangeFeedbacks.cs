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

    [Header("On Press Attack (only one object, not pooling, deactivate on release)")]
    [SerializeField] Transform barrelOnPress = default;
    [SerializeField] GameObject gameObjectOnPress = default;
    [SerializeField] ParticleSystem particlesOnPress = default;
    [SerializeField] AudioSource audioSourcePrefab = default;
    [SerializeField] AudioStruct audioOnPress = default;

    //on release attack
    GameObject instantiatedGameObjectOnPress;
    ParticleSystem instantiatedParticlesOnPress;
    AudioSource instantiatedAudioOnPress;

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
            weaponRange.onPressAttack += OnPressAttack;
            weaponRange.onReleaseAttack += OnReleaseAttack;
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
            weaponRange.onPressAttack -= OnPressAttack;
            weaponRange.onReleaseAttack -= OnReleaseAttack;
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

    void OnPressAttack()
    {
        //if first time, instantiate prefabs
        {
            //instantiate game object
            if (gameObjectOnPress && instantiatedGameObjectOnPress == null)
            {
                instantiatedGameObjectOnPress = Instantiate(gameObjectOnPress);
            }
            //instantiate particles
            if (particlesOnPress && instantiatedParticlesOnPress == null)
            {
                instantiatedParticlesOnPress = Instantiate(particlesOnPress);
            }
            //instantiate audiosource
            if (audioOnPress.audioClip && audioSourcePrefab && instantiatedAudioOnPress == null)
            {
                instantiatedAudioOnPress = Instantiate(audioSourcePrefab);
                instantiatedAudioOnPress.clip = audioOnPress.audioClip;
                instantiatedAudioOnPress.volume = audioOnPress.volume;
            }
        }

        //set game object
        if(instantiatedGameObjectOnPress)
        {
            instantiatedGameObjectOnPress.transform.position = barrelOnPress.position;
            instantiatedGameObjectOnPress.transform.rotation = barrelOnPress.rotation;

            //rotate left/right and set parent
            instantiatedGameObjectOnPress.transform.localScale = transform.lossyScale;
            instantiatedGameObjectOnPress.transform.SetParent(transform);

            instantiatedGameObjectOnPress.SetActive(true);
        }
        //set particles
        if(instantiatedParticlesOnPress)
        {
            instantiatedParticlesOnPress.transform.position = barrelOnPress.position;
            instantiatedParticlesOnPress.transform.rotation = barrelOnPress.rotation;

            //play
            instantiatedParticlesOnPress.gameObject.SetActive(true);
            instantiatedParticlesOnPress.Play();
        }
        //set audiosource
        if(instantiatedAudioOnPress)
        {
            instantiatedAudioOnPress.transform.position = barrelOnPress.position;

            //play
            instantiatedAudioOnPress.gameObject.SetActive(true);
            instantiatedAudioOnPress.Play();
        }
    }

    void OnReleaseAttack()
    {
        //deactive game object
        if (instantiatedGameObjectOnPress)
        {
            instantiatedGameObjectOnPress.SetActive(false);
        }
        //deactive particles
        if(instantiatedParticlesOnPress)
        {
            instantiatedParticlesOnPress.gameObject.SetActive(false);
        }
        //deactive sound
        if(instantiatedAudioOnPress)
        {
            instantiatedAudioOnPress.gameObject.SetActive(false);
        }
    }
}
