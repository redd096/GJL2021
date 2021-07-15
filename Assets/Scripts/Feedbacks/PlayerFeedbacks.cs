using UnityEngine;
using redd096;

public class PlayerFeedbacks : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] GameObject aimPrefab = default;
    [SerializeField] bool mouseFree = true;
    [SerializeField] float minDistance = 1.3f;
    [SerializeField] float maxDistance = 2.3f;

    [Header("On Dash")]
    [SerializeField] ParticleSystem particlesToActivateOnDash = default;
    [SerializeField] AudioStruct audioOnDash = default;

    Player player;
    GameObject aimObject;

    void Awake()
    {
        //instantiate aim
        if(aimPrefab)
            aimObject = Instantiate(aimPrefab, transform);
    }

    void OnEnable()
    {
        //get references
        player = GetComponent<Player>();

        //add events
        if (player)
        {
            player.onDash += OnDash;
        }
    }

    void OnDisable()
    {
        //remove events
        if (player)
        {
            player.onDash -= OnDash;
        }
    }

    void Update()
    {
        //set aimObject position
        SetAimPosition();
    }

    void SetAimPosition()
    {
        if (aimObject == null)
            return;

        Vector2 inputPosition = Vector2.zero;

        if (InputRedd096.IsCurrentControlScheme(player.playerInput, "KeyboardAndMouse"))
        {
            //set mouse position
            inputPosition = player.AimPositionNotNormalized;

            //clamp position if mouse not free
            if (mouseFree == false)
            {
                if (inputPosition.magnitude > maxDistance)
                    inputPosition = player.DirectionAim * maxDistance;  //max
                if (inputPosition.magnitude < minDistance)
                    inputPosition = player.DirectionAim * minDistance;  //min
            }
        }
        else
        {
            //from 0 to 1, from min to max
            float value = Mathf.Lerp(minDistance, maxDistance, player.AimPositionNotNormalized.magnitude);

            //set gamepad position
            inputPosition = player.DirectionAim * value;
        }

        //set aimObject position
        aimObject.transform.position = new Vector2(transform.position.x, transform.position.y) + inputPosition;
    }

    void OnDash()
    {
        //activate particles + instantiate audio
        particlesToActivateOnDash?.Play();
        SoundManager.instance.Play(audioOnDash.audioClip, transform.position, audioOnDash.volume);
    }
}
