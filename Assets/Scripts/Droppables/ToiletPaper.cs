using UnityEngine;
using redd096;
using DG.Tweening;

public class ToiletPaper : MonoBehaviour, IDroppable
{
    [Header("Animation Destruction")]
    [SerializeField] Vector3 movement = Vector3.up;
    [SerializeField] float durationMovement = 0.8f;
    [SerializeField] float durationSecondMovement = 0.5f;

    bool alreadyPicked;

    public void Pick()
    {
        if (alreadyPicked)
            return;

        alreadyPicked = true;

        //add toilet paper
        GameManager.instance.CurrentToiletPaper++;

        //animation to currentToiletPaper in UI
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOMove(transform.position + movement, durationMovement));
        sequence.Append(transform.DOMove(GameManager.instance.uiManager.ToiletPaperText ? GameManager.instance.uiManager.ToiletPaperText.transform.position : transform.position, durationSecondMovement));

        //and destroy it
        sequence.OnComplete(() => Destroy(gameObject));

        sequence.Play();
    }
}
