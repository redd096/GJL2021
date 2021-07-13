using UnityEngine;
using redd096;

public class PlayerFeedbacks : MonoBehaviour
{
    [Header("On Dash")]
    [SerializeField] ParticleSystem particlesToActivateOnDash = default;
    [SerializeField] AudioStruct audioOnDash = default;

    Player player;

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

    void OnDash()
    {
        //activate particles + instantiate audio
        particlesToActivateOnDash?.Play();
        SoundManager.instance.Play(audioOnDash.audioClip, transform.position, audioOnDash.volume);
    }
}
