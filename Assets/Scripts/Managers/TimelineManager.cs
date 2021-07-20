using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using redd096;

public class TimelineManager : MonoBehaviour
{
    [Header("Timeline Slider - by default get in children")]
    [SerializeField] Slider timelineSlider = default;
    [SerializeField] float[] valuesSliderBasedOnCurrentRoom = default;

    [Header("Animation")]
    [SerializeField] AnimationCurve animationCurve = default;

    void OnEnable()
    {
        //get references
        if (timelineSlider == null)
            timelineSlider = GetComponentInChildren<Slider>();

        //reset slider and start coroutine
        if (timelineSlider)
        {
            timelineSlider.value = 0;
            StartCoroutine(AnimationCoroutine());
        }
    }

    IEnumerator AnimationCoroutine()
    {
        //get current room or last index if room is greater
        int index = Mathf.Min(GameManager.instance.CurrentRoom, valuesSliderBasedOnCurrentRoom.Length - 1);

        float currentTime = 0;
        while(currentTime < animationCurve.keys[animationCurve.length - 1].time)
        {
            currentTime += Time.deltaTime;

            //set value based on current room
            if (timelineSlider)
                timelineSlider.value = Mathf.Lerp(0, valuesSliderBasedOnCurrentRoom[index], animationCurve.Evaluate(currentTime));

            yield return null;
        }
    }
}
