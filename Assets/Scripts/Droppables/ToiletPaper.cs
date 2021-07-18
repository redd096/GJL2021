using UnityEngine;
using redd096;
using DG.Tweening;

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
    [SerializeField] Vector3 movement = Vector3.up;
    [SerializeField] float sizeToReach = 1.5f;
    [SerializeField] float durationMovement = 0.8f;
    [SerializeField] Ease easeMovement1 = Ease.Unset;

    [Header("Animation Destruction (movement 2)")]
    [SerializeField] bool moveToToiletPaperInUI = true;
    [CanShow("moveToToiletPaperInUI", NOT = true)] [SerializeField] Vector3 movement2 = Vector3.down;
    [SerializeField] float sizeToReach2 = 0.3f;
    [SerializeField] float durationMovement2 = 0.5f;
    [SerializeField] Ease easeMovement2 = Ease.Unset;

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
        AnimationDestroy();
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
                instantiatedGameObject.transform.SetParent(character.transform);
        }

        //particles and set parent
        ParticleSystem instantiatedParticles = ParticlesManager.instance.Play(particlesOnPick, transform.position, transform.rotation);
        if (instantiatedParticles && setCharacterAsParent)
            instantiatedParticles.transform.SetParent(character.transform);

        SoundManager.instance.Play(audioOnPick.audioClip, transform.position, audioOnPick.volume);
    }

    void AnimationDestroy()
    {
        Sequence sequence = DOTween.Sequence();

        //movement 1
        sequence.Join(transform.DOMove(transform.position + movement, durationMovement).SetEase(easeMovement1));
        sequence.Join(transform.DOScale(sizeToReach, durationMovement));

        //position movement 2 (or move to Toilet Paper in UI)
        Vector3 positionToReach = transform.position + movement2;
        if (moveToToiletPaperInUI)
            positionToReach = GameManager.instance.uiManager.ToiletPaperImage ? GameManager.instance.uiManager.ToiletPaperImage.transform.position : transform.position;

        //movement 2
        sequence.Append(transform.DOMove(positionToReach, durationMovement2).SetEase(easeMovement2));
        sequence.Join(transform.DOScale(sizeToReach2, durationMovement2));

        //and destroy it
        sequence.OnComplete(() => Destroy(gameObject));

        sequence.Play();
    }
}
