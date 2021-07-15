using UnityEngine;
using redd096;
using DG.Tweening;

public class ToiletPaper : MonoBehaviour, IDroppable
{
    [Header("Layer on Pick")]
    [SerializeField] int layerOnPick = 101;

    [Header("Animation Destruction (movement 1)")]
    [SerializeField] Vector3 movement = Vector3.up;
    [SerializeField] float durationMovement = 0.8f;
    [SerializeField] Ease easeMovement1 = Ease.Unset;

    [Header("Animation Destruction (movement 1)")]
    [SerializeField] bool moveToToiletPaperInUI = true;
    [CanShow("moveToToiletPaperInUI", NOT = true)] [SerializeField] Vector3 movement2 = Vector3.down;
    [SerializeField] float durationMovement2 = 0.5f;
    [SerializeField] Ease easeMovement2 = Ease.Unset;

    bool alreadyPicked;

    public void Pick()
    {
        if (alreadyPicked)
            return;

        alreadyPicked = true;

        //add toilet paper and set layer
        GameManager.instance.CurrentToiletPaper++;
        GetComponentInChildren<SpriteRenderer>().sortingOrder = layerOnPick;

        //animation destruction
        AnimationDestroy();
    }

    void AnimationDestroy()
    {
        Sequence sequence = DOTween.Sequence();

        //movement 1
        sequence.Join(transform.DOMove(transform.position + movement, durationMovement).SetEase(easeMovement1));

        //position movement 2 (or move to Toilet Paper in UI)
        Vector3 positionToReach = transform.position + movement2;
        if (moveToToiletPaperInUI)
            positionToReach = GameManager.instance.uiManager.ToiletPaperImage ? GameManager.instance.uiManager.ToiletPaperImage.transform.position : transform.position;

        //movement 2
        sequence.Append(transform.DOMove(positionToReach, durationMovement2).SetEase(easeMovement2));

        //and destroy it
        sequence.OnComplete(() => Destroy(gameObject));

        sequence.Play();
    }
}
