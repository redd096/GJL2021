using UnityEngine;
using redd096;

public class BulletFeedbacks : MonoBehaviour
{
    [Header("On Hit")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnHit = default;
    [SerializeField] ParticleSystem particlesOnHit = default;
    [SerializeField] AudioStruct audioOnHit = default;

    [Header("On AutoDestruction")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnAutodestruction = default;
    [SerializeField] ParticleSystem particlesOnAutodestruction = default;
    [SerializeField] AudioStruct audioOnAutodestruction = default;

    [Header("Camera Shake")]
    [SerializeField] bool cameraShakeOnHit = false;
    [SerializeField] bool cameraShakeOnAutoDestruction = false;
    [CanShow("cameraShakeOnHit", "cameraShakeOnAutoDestruction", checkAND = false)] [SerializeField] bool customShake = false;
    [CanShow("cameraShakeOnHit", "cameraShakeOnAutoDestruction", checkAND = false)] [SerializeField] float shakeDuration = 1;
    [CanShow("cameraShakeOnHit", "cameraShakeOnAutoDestruction", checkAND = false)] [SerializeField] float shakeAmount = 0.7f;

    Bullet bullet;

    void OnEnable()
    {
        //get references
        bullet = GetComponent<Bullet>();

        //add events
        if(bullet)
        {
            bullet.onHit += OnHit;
            bullet.onAutodestruction += OnAutodestruction;
        }
    }

    void OnDisable()
    {
        //remove events
        if (bullet)
        {
            bullet.onHit -= OnHit;
            bullet.onAutodestruction -= OnAutodestruction;
        }
    }

    void OnHit(GameObject hit)
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnHit, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnHit, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnHit.audioClip, transform.position, audioOnHit.volume);

        //camera shake
        if (cameraShakeOnHit)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }

    void OnAutodestruction()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnAutodestruction, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnAutodestruction, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnAutodestruction.audioClip, transform.position, audioOnAutodestruction.volume);

        //camera shake
        if (cameraShakeOnAutoDestruction)
        {
            //custom or default
            if (customShake)
                CameraShake.instance.StartShake(shakeDuration, shakeAmount);
            else
                CameraShake.instance.StartShake();
        }
    }
}
