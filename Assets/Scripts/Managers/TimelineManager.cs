using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using redd096;

public class TimelineManager : MonoBehaviour
{
    [Header("Timeline Slider - by default get in children")]
    [SerializeField] Slider timelineSlider = default;
    [SerializeField] float startValue = 0.13f;
    [SerializeField] int maxRooms = 5;

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

        //set value based on current room
        float endValue = ((float)GameManager.instance.CurrentRoom / maxRooms) + startValue;
        timelineSlider.DOValue(endValue, durationAnimation).SetEase(ease);
    }
}
