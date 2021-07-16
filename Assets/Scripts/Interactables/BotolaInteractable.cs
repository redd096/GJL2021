using UnityEngine;
using redd096;

public class BotolaInteractable : MonoBehaviour, IInteractable
{
    [Header("On Open - by default get in children")]
    [SerializeField] Animator anim = default;
    [SerializeField] GameObject objectToActivate = default;
    [SerializeField] bool checkNoEnemiesInScene = true;
    [SerializeField] bool checkPlayerHasWeapon = true;

    [Header("On Interact")]
    [SerializeField] bool destroyWeapon = true;
    [SerializeField] float timeBeforeLoadNewScene = 1;
    [SerializeField] string sceneToLoad = "Scena Game";

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] bool isOpen;

    void Awake()
    {
        //get references
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        //by default, object to activate is hidden
        if (objectToActivate)
            objectToActivate.SetActive(false);
    }

    void FixedUpdate()
    {
        //if change state (can open or not)
        if (CheckCanOpen() != isOpen)
            OpenCloseBotola();
    }

    #region open or close

    bool CheckCanOpen()
    {
        bool canOpen = true;

        //check there are not enemies in scene
        if (canOpen && checkNoEnemiesInScene)
            canOpen = FindObjectsOfType<Enemy>().Length <= 0;

        //check player has weapon equipped
        if (canOpen && checkPlayerHasWeapon)
            canOpen = GameManager.instance.levelManager.Players[0].CurrentWeapon != null;

        return canOpen;
    }

    void OpenCloseBotola()
    {
        //open or close
        isOpen = !isOpen;

        //show animation
        anim?.SetTrigger(isOpen ? "Open" : "Close");

        //activate object
        if (objectToActivate)
            objectToActivate.SetActive(isOpen);
    }

    #endregion

    #region interact

    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        if (isOpen == false)
            return;

        //stop this script
        enabled = false;    //stop check open/close
        isOpen = false;     //can't interact anymore

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

    #endregion
}
