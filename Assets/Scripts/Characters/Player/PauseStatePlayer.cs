using UnityEngine;
using redd096;

public class PauseStatePlayer : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //resume
        ResumeGame(InputRedd096.GetButtonDown("Pause"));
    }

    void ResumeGame(bool inputResume)
    {
        //resume
        if (inputResume)
        {
            SceneLoader.instance.ResumeGame();
        }
    }
}
