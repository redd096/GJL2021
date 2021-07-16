using UnityEngine;
using redd096;

public class BotolaInteractable : MonoBehaviour, IInteractable
{
    [Header("On Interact")]
    [SerializeField] bool destroyWeapon = true;
    [SerializeField] float timeBeforeLoadNewScene = 1;
    [SerializeField] string sceneToLoad = "Scena Game";

    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        //destroy weapon
        if (destroyWeapon)
            player.DropWeapon();

        //deactive player
        player.GetComponent<Animator>().enabled = false;

        //load scene after few seconds
        Invoke("LoadScene", timeBeforeLoadNewScene);
    }

    void LoadScene()
    {
        //change scene
        SceneLoader.instance.LoadScene(sceneToLoad);
    }
}
