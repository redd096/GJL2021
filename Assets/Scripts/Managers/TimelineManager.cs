using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using redd096;

public class TimelineManager : MonoBehaviour
{
    [Header("Timeline Slider - by default get in children")]
    [SerializeField] Slider timelineSlider = default;
    [SerializeField] float[] valuesSliderBasedOnCurrentRoom = default;

    [Header("Animation")]
    [SerializeField] float durationAnimation = 3;
    [SerializeField] Ease ease = Ease.Unset;

    void OnEnable()
    {
        //get references
        if (timelineSlider == null)
            timelineSlider = GetComponentInChildren<Slider>();

        //reset slider
        if (timelineSlider)
            timelineSlider.value = 0;

        //animation slider
        AnimationSlider();
    }

    void AnimationSlider()
    {
        if (timelineSlider == null)
            return;

        //get current room or last index if room is greater
        int index = Mathf.Min(GameManager.instance.CurrentRoom, valuesSliderBasedOnCurrentRoom.Length - 1);

        //set value based on current room
        timelineSlider.DOValue(valuesSliderBasedOnCurrentRoom[index], durationAnimation).SetEase(ease);
    }
}
