using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class BotolaFeedbacks : MonoBehaviour
{
    [Header("On Open - by default get in children")]
    [SerializeField] Animator anim = default;
    [SerializeField] GameObject objectToActivate = default;

    [Header("Animation Change Scene")]
    [SerializeField] bool fadeInFromBotola = false;
    [SerializeField] bool fadeOutFromBotola = false;
    [SerializeField] Animator animPrefab = default;
    [SerializeField] AudioStruct soundFadeIn = default;
    [SerializeField] AudioStruct soundFadeOut = default;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] Animator animChangeScene;

    BotolaInteractable botolaInteractable;
    Camera cam;

    void Start()
    {
        //get references
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        cam = Camera.main;

        //by default, object to activate is hidden
        if (objectToActivate)
            objectToActivate.SetActive(false);

        //call fade in
        FadeIn();
    }

    void OnEnable()
    {
        //get references
        botolaInteractable = GetComponent<BotolaInteractable>();

        //add events
        if(botolaInteractable)
        {
            botolaInteractable.onCloseOpen += OnCloseOpen;
            botolaInteractable.onInteract += OnInteract;
        }
    }

    void OnDisable()
    {
        //remove events
        if (botolaInteractable)
        {
            botolaInteractable.onCloseOpen -= OnCloseOpen;
            botolaInteractable.onInteract -= OnInteract;
        }
    }

    void OnCloseOpen(bool isOpen)
    {
        //show open/close animation
        if(anim)
            anim.SetTrigger(isOpen ? "Open" : "Close");

        //activate object
        if (objectToActivate)
            objectToActivate.SetActive(isOpen);
    }

    void FadeIn()
    {
        //instantiate animation Fade In
        if (animPrefab)
        {
            Vector3 position = fadeInFromBotola ? transform.position : new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);   //from botola or center camera
            animChangeScene = Instantiate(animPrefab, transform);
            animChangeScene.transform.position = position;
        }

        //sfx
        SoundManager.instance.Play(soundFadeIn.audioClip, transform.position, soundFadeIn.volume);
    }

    //fade out
    void OnInteract()
    {
        //start animation Fade Out
        if (animChangeScene)
        {
            animChangeScene.transform.position = fadeOutFromBotola ? transform.position : new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);    //from botola or center camera
            animChangeScene.SetTrigger("Fade Out");
        }

        //sfx
        SoundManager.instance.Play(soundFadeOut.audioClip, transform.position, soundFadeOut.volume);
    }
}
