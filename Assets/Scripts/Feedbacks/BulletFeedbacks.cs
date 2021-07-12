using UnityEngine;
using redd096;

public class BulletFeedbacks : MonoBehaviour
{
    [Header("On Hit")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnHit = default;
    [SerializeField] ParticleSystem particlesOnHit = default;
    [SerializeField] AudioStruct audioOnHit = default;

    Bullet bullet;

    void OnEnable()
    {
        //get references
        bullet = GetComponent<Bullet>();

        //add events
        if(bullet)
        {
            bullet.onHit += OnHit;
        }
    }

    void OnDisable()
    {
        //remove events
        if (bullet)
        {
            bullet.onHit -= OnHit;
        }
    }

    void OnHit()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnHit, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right and set parent
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnHit, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnHit.audioClip, transform.position, audioOnHit.volume);
    }
}
