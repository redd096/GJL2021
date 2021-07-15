using UnityEngine;
using redd096;

public class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("On Interact")]
    [SerializeField] string sceneToLoad = "Scena Game";

    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        //change scene
        SceneLoader.instance.LoadScene(sceneToLoad);
    }
}
