using UnityEngine;
using UnityEngine.UI;
using redd096;

public class BossFeedbacks : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Slider healthSlider = default;

    [Header("Animator Animations - if not setted get in children")]
    [SerializeField] Animator anim;

    [Header("On Spawn")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnSpawn = default;
    [SerializeField] ParticleSystem particlesOnSpawn = default;
    [SerializeField] AudioStruct[] audiosOnSpawn = default;

    [Header("On Bomb State")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnBomb = default;
    [SerializeField] ParticleSystem particlesOnBomb = default;
    [SerializeField] AudioStruct[] audiosOnBomb = default;

    [Header("On Shoot State")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnShoot = default;
    [SerializeField] ParticleSystem particlesOnShoot = default;
    [SerializeField] AudioStruct[] audiosOnShoot = default;

    [Header("On Idle State")]
    [SerializeField] InstantiatedGameObjectStruct[] gameObjectsOnIdle = default;
    [SerializeField] ParticleSystem particlesOnIdle = default;
    [SerializeField] AudioStruct[] audiosOnIdle = default;

    EnemyBoss boss;

    void OnEnable()
    {
        //get references
        boss = GetComponent<EnemyBoss>();

        //be sure is setted animator
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        //add events
        if (boss)
        {
            boss.onGetDamage += OnGetDamage;

            boss.onSpawnState += OnSpawnState;
            boss.onBombState += OnBombState;
            boss.onShootState += OnShootState;
            boss.onIdleState += OnIdleState;
            boss.onEndIdleState += OnEndIdleState;
        }
    }

    void OnDisable()
    {
        //remove events
        if (boss)
        {
            boss.onGetDamage -= OnGetDamage;

            boss.onSpawnState -= OnSpawnState;
            boss.onBombState -= OnBombState;
            boss.onShootState -= OnShootState;
            boss.onIdleState -= OnIdleState;
            boss.onEndIdleState -= OnEndIdleState;
        }
    }

    void OnGetDamage()
    {
        //update slider health
        if (healthSlider)
            healthSlider.value = boss.Health / boss.MaxHealth;
    }

    void OnSpawnState()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectsOnSpawn, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnSpawn, transform.position, transform.rotation);
        SoundManager.instance.Play(audiosOnSpawn, transform.position);

        //set animator
        anim.SetTrigger("Spawn");
    }

    void OnBombState()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectsOnBomb, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnBomb, transform.position, transform.rotation);
        SoundManager.instance.Play(audiosOnBomb, transform.position);


        //set animator
        anim.SetTrigger("Bomb");
    }

    void OnShootState()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectsOnShoot, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnShoot, transform.position, transform.rotation);
        SoundManager.instance.Play(audiosOnShoot, transform.position);


        //set animator
        anim.SetTrigger("Shoot");
    }

    void OnIdleState()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectsOnIdle, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnIdle, transform.position, transform.rotation);
        SoundManager.instance.Play(audiosOnIdle, transform.position);


        //set animator
        anim.SetTrigger("Idle");
    }

    void OnEndIdleState()
    {
        //set animator
        anim.SetTrigger("End Idle");
    }
}
