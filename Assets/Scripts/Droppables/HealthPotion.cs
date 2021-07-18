using UnityEngine;
using redd096;

public class HealthPotion : MonoBehaviour, IDroppable
{
    [Header("Health")]
    [SerializeField] float healthGiven = 10;

    [Header("On Pick")]
    [SerializeField] bool setCharacterAsParent = false;
    [SerializeField] InstantiatedGameObjectStruct gameObjectOnPick = default;
    [SerializeField] ParticleSystem particlesOnPick = default;
    [SerializeField] AudioStruct audioOnPick = default;

    bool alreadyPicked;

    public void Pick(Character character)
    {
        if (alreadyPicked)
            return;

        alreadyPicked = true;

        //give health
        character.GetHealth(healthGiven);

        //feedbacks
        Feedbacks(character);

        //and destroy
        Destroy(gameObject);
    }

    void Feedbacks(Character character)
    {
        //instantiate vfx and sfx
        GameObject instantiatedGameObject = InstantiateGameObjectManager.instance.Play(gameObjectOnPick, transform.position, transform.rotation);
        if (instantiatedGameObject)
        {
            //rotate left/right
            instantiatedGameObject.transform.localScale = transform.lossyScale;

            //set parent
            if (setCharacterAsParent)
                instantiatedGameObject.transform.SetParent(character.transform);
        }

        //particles and set parent
        ParticleSystem instantiatedParticles = ParticlesManager.instance.Play(particlesOnPick, transform.position, transform.rotation);
        if (instantiatedParticles && setCharacterAsParent)
            instantiatedParticles.transform.SetParent(character.transform);

        SoundManager.instance.Play(audioOnPick.audioClip, transform.position, audioOnPick.volume);
    }
}
