using System.Collections;
using UnityEngine;
using redd096;

public class ToiletPaper : MonoBehaviour, IDroppable
{
    [Header("On Pick")]
    [SerializeField] int layerOnPick = 101;
    [SerializeField] GameObject particleToActive = default;

    [Header("On Pick (VFX and SFX)")]
    [SerializeField] bool setCharacterAsParent = false;
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnPick = default;
    [SerializeField] ParticleSystem particlesOnPick = default;
    [SerializeField] AudioStruct audioOnPick = default;

    [Header("Animation Destruction (movement 1)")]
    [SerializeField] Vector3 movement1 = Vector3.up;
    [SerializeField] float sizeToReach1 = 1.5f;
    [SerializeField] AnimationCurve curveMovement1 = default;

    [Header("Animation Destruction (movement 2)")]
    [SerializeField] bool moveToToiletPaperInUI = true;
    [CanShow("moveToToiletPaperInUI", NOT = true)] [SerializeField] Vector3 movement2 = Vector3.down;
    [SerializeField] float sizeToReach2 = 0.3f;
    [SerializeField] AnimationCurve curveMovement2 = default;

    bool alreadyPicked;

    void Awake()
    {
        //be sure to deactivate particles on start
        if(particleToActive)
            particleToActive.SetActive(false);
    }

    public void Pick(Character character)
    {
        if (alreadyPicked)
            return;

        alreadyPicked = true;

        //add toilet paper and update UI
        GameManager.instance.CurrentToiletPaper++;
        GameManager.instance.uiManager.UpdateToiletPaper(GameManager.instance.CurrentToiletPaper);

        //call feedbacks
        VFXFeedbacks(character);
        Feedbacks();
    }

    void Feedbacks()
    {
        //set layer and active particle
        GetComponentInChildren<SpriteRenderer>().sortingOrder = layerOnPick;
        if (particleToActive)
            particleToActive.SetActive(true);

        //animation destruction
        StartCoroutine(AnimationDestroyCoroutine());
    }

    void VFXFeedbacks(Character character)
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnPick, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;

            //set parent
            if (setCharacterAsParent)
            {
                instantiatedGameObject.transform.SetParent(character.transform);
                instantiatedGameObject.transform.position = character.transform.position;
            }
        }

        //particles and set parent
        ParticleSystem instantiatedParticles = ParticlesManager.instance.Play(particlesOnPick, transform.position, transform.rotation);
        if (instantiatedParticles && setCharacterAsParent)
        {
            instantiatedParticles.transform.SetParent(character.transform);
            instantiatedParticles.transform.position = character.transform.position;
        }

        SoundManager.instance.Play(audioOnPick.audioClip, transform.position, audioOnPick.volume);
    }

    IEnumerator AnimationDestroyCoroutine()
    {
        float durationMovement1 = curveMovement1.keys[curveMovement1.length - 1].time;
        float durationMovement2 = curveMovement2.keys[curveMovement2.length - 1].time;

        //movement 1
        float currentTime = 0;
        float delta = 0;
        Vector2 startPosition = transform.position;
        Vector2 endPosition = transform.position + movement1;
        Vector2 startSize = transform.localScale;
        Vector2 endSize = new Vector2(sizeToReach1, sizeToReach1);
        while(delta < 1)
        {
            currentTime += Time.deltaTime;
            delta += Time.deltaTime / durationMovement1;

            //movement with curve, size with delta
            transform.position = Vector2.Lerp(startPosition, endPosition, curveMovement1.Evaluate(currentTime));
            transform.localScale = Vector2.Lerp(startSize, endSize, delta);

            yield return null;
        }

        //movement 2 (reach toilet paper image, or movement2)
        currentTime = 0;
        delta = 0;
        startPosition = transform.position;
        endPosition = moveToToiletPaperInUI && GameManager.instance.uiManager.ToiletPaperImage ? GameManager.instance.uiManager.ToiletPaperImage.transform.position : transform.position + movement2;
        startSize = transform.localScale;
        endSize = new Vector2(sizeToReach2, sizeToReach2);
        while (delta < 1)
        {
            currentTime += Time.deltaTime;
            delta += Time.deltaTime / durationMovement2;

            //if moving to UI, update end position
            if (moveToToiletPaperInUI && GameManager.instance.uiManager.ToiletPaperImage)
                endPosition = GameManager.instance.uiManager.ToiletPaperImage.transform.position;

            //movement with curve, size with delta
            transform.position = Vector2.Lerp(startPosition, endPosition, curveMovement2.Evaluate(currentTime));
            transform.localScale = Vector2.Lerp(startSize, endSize, delta);

            yield return null;
        }

        //on finish animation, destroy
        Destroy(gameObject);
    }
}
