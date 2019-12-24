using UnityEngine;

public class CreateAnimation : StateMachineBehaviour
{

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      animator.GetComponent<OpenClose>().endanimation = true;
    }

    
}