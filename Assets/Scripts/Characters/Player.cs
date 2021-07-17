using UnityEngine.InputSystem;
using UnityEngine;
using redd096;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character
{
    [Header("Interact")]
    public float RadiusInteract = 1.5f;

    [Header("Camera Follow")]
    [SerializeField] bool cameraFollowPlayer = true;
    [CanShow("cameraFollowPlayer")] [SerializeField] Vector3 offset = Vector3.back * 10;

    public PlayerInput playerInput { get; private set; }

    Animator stateMachine;
    Camera cam;

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
        cam = Camera.main;

        //if follow camera, set parent
        if (cameraFollowPlayer)
        {
            cam.transform.SetParent(transform);
            cam.transform.position = transform.position + offset;
        }

        //if there a weapon saved in game manager, set it (or use weapon prefab for first room)
        GameManager.instance.PickWeaponSaved(this, weaponPrefab);

        //get also life saved in game manager (or save in game manager)
        if (GameManager.instance.CurrentLife > 0)
            health = GameManager.instance.CurrentLife;
        else
            GameManager.instance.CurrentLife = health;

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        //check pick droppables
        CheckPickDroppables(collision);
    }

    #region overrides

    public override void GetDamage(float damage, bool ignoreShield = true, Vector2 hitPosition = default)
    {
        base.GetDamage(damage, ignoreShield, hitPosition);

        //update health UI
        GameManager.instance.CurrentLife = health;
        GameManager.instance.uiManager.UpdateHealth(health, MaxHealth);
    }

    public override void PickWeapon(WeaponBASE prefab)
    {
        base.PickWeapon(prefab);

        //save it and update UI
        GameManager.instance.CurrentWeapon = prefab;
        GameManager.instance.CurrentWeaponSprite = CurrentWeapon?.GetComponentInChildren<SpriteRenderer>().sprite;
        GameManager.instance.uiManager.UpdateWeaponImage(CurrentWeapon?.GetComponentInChildren<SpriteRenderer>().sprite);

        //save also in already seen
        GameManager.instance.WeaponsAlreadyUsed.Add(prefab);
    }

    public override void DropWeapon()
    {
        base.DropWeapon();

        //save it and update UI
        GameManager.instance.CurrentWeapon = null;
        GameManager.instance.CurrentWeaponSprite = null;
        GameManager.instance.uiManager.UpdateWeaponImage(null);
    }

    public override void Die()
    {
        //if follow camera, remove parent
        if (cam.transform.parent == transform)
            cam.transform.SetParent(null);

        base.Die();

        //call end game
        GameManager.instance.levelManager.EndGame();
    }

    #endregion

    #region private API

    void CheckPickDroppables(Collider2D collision)
    {
        //pick if is droppable
        collision.GetComponentInParent<IDroppable>()?.Pick();
    }

    #endregion

    /// <summary>
    /// Set state to pause or resume
    /// </summary>
    /// <param name="pause"></param>
    public void SetState(string state)
    {
        stateMachine.SetTrigger(state);
    }
}
