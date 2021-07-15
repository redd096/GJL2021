using UnityEngine.InputSystem;
using UnityEngine;
using redd096;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character
{
    [Header("Interact")]
    public float RadiusInteract = 1.5f;

    public PlayerInput playerInput { get; private set; }

    Animator stateMachine;

    //animation events
    public System.Action onDash { get; set; }

    void OnDrawGizmos()
    {
        //draw radius interact
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, RadiusInteract);
    }

    protected override void Awake()
    {
        base.Awake();

        //get references
        playerInput = GetComponent<PlayerInput>();
        stateMachine = GetComponent<Animator>();

        //add to level manager list
        if (GameManager.instance.levelManager)
            GameManager.instance.levelManager.Players.Add(this);
    }

    void OnDestroy()
    {
        //remove from level manager list
        if (GameManager.instance.levelManager)
            GameManager.instance.levelManager.Players.Remove(this);
    }

    protected override void Update()
    {
        base.Update();

        //pause
        if (InputRedd096.GetButtonDown("Pause"))
        {
            if (Time.timeScale > 0)
            {
                SceneLoader.instance.PauseGame();
            }
            else
            {
                SceneLoader.instance.ResumeGame();
            }
        }
    }

    /// <summary>
    /// Set state to pause or resume
    /// </summary>
    /// <param name="pause"></param>
    public void SetPauseState(bool pause)
    {
        stateMachine.SetTrigger(pause ? "Pause" : "Resume");
    }
}
