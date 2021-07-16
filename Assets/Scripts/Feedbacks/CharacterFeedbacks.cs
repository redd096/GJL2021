using System.Collections;
using UnityEngine;
using redd096;

public class CharacterFeedbacks : MonoBehaviour
{
    [Header("On Get Damage")]
    [SerializeField] Material blinkMaterial = default;
    [SerializeField] float durationBlink = 0.2f;
    [SerializeField] bool ignoreIfAlreadyBlinking = true;

    [Header("On Die")]
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnDie = default;
    [SerializeField] ParticleSystem particlesOnDie = default;
    [SerializeField] AudioStruct audioOnDie = default;

    [Header("Animator Animations - if not setted get in children")]
    [SerializeField] Animator anim;
    [SerializeField] float minSpeedToStartRun = 0.01f;

    [Header("Sprite in Order when rotate left - if not setted get in children")]
    [SerializeField] SpriteRenderer spriteToChange = default;
    [SerializeField] int spriteInOrder = default;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] float calculatedSpeed = 0;

    Character character;
    int defaultSpriteInOrder;
    Vector3 previousPosition;

    Material defaultMaterial;
    Coroutine blinkCoroutine;

    private void OnEnable()
    {
        //get references
        character = GetComponent<Character>();

        //add events
        if(character)
        {
            character.onGetDamage += OnGetDamage;
            character.onDie += OnDie;
        }
    }

    private void OnDisable()
    {
        //remove events
        if (character)
        {
            character.onGetDamage -= OnGetDamage;
            character.onDie -= OnDie;
        }
    }

    void Start()
    {
        //be sure is setted animator
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        //be sure is setted sprite to change
        if (spriteToChange == null) 
            spriteToChange = GetComponentInChildren<SpriteRenderer>();

        //and save its default order in layer
        defaultSpriteInOrder = spriteToChange.sortingOrder;

        //get references
        defaultMaterial = spriteToChange.material;
    }

    void Update()
    {
        //rotate left or right
        if (character.DirectionAim.x < 0 && transform.localScale.x >= 0)
            RotateObject(false);
        else if (character.DirectionAim.x > 0 && transform.localScale.x <= 0)
            RotateObject(true);
    }

    void FixedUpdate()
    {
        //calculate speed (don't use rigidbody, to not glitch when hit walls), and save previous position
        calculatedSpeed = (transform.position - previousPosition).magnitude / Time.fixedDeltaTime;
        previousPosition = transform.position;

        //set if running or idle
        if (calculatedSpeed > minSpeedToStartRun && anim.GetBool("Running") == false)
            SetRun(true);
        else if (calculatedSpeed <= minSpeedToStartRun && anim.GetBool("Running"))
            SetRun(false);
    }

    #region private API

    void RotateObject(bool toRight)
    {
        //rotate right and reset order in layer
        if (toRight)
        {
            transform.localScale = Vector3.one;
            spriteToChange.sortingOrder = defaultSpriteInOrder;
        }
        //rotate left and set new order in layer
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);

            defaultSpriteInOrder = spriteToChange.sortingOrder;     //save default order in layer
            spriteToChange.sortingOrder = spriteInOrder;
        }
    }

    void SetRun(bool isRunning)
    {
        //set running or idle animation
        anim.SetBool("Running", isRunning);
    }

    void OnGetDamage()
    {
        //blink sprite
        if(blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(BlinkCoroutine());
        }
        //if already blinking, reset timer if necessary
        else if(ignoreIfAlreadyBlinking == false)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkCoroutine());
        }
    }

    void OnDie()
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnDie, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;
        }
        ParticlesManager.instance.Play(particlesOnDie, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnDie.audioClip, transform.position, audioOnDie.volume);
    }

    IEnumerator BlinkCoroutine()
    {
        //check in case spriteRenderer is destroyed
        if (spriteToChange)
        {
            //set blink
            spriteToChange.material = blinkMaterial;

            //wait
            yield return new WaitForSeconds(durationBlink);

            //reset sprite color
            spriteToChange.material = defaultMaterial;
        }

        blinkCoroutine = null;
    }

    #endregion
}
