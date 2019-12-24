using UnityEngine;

public class OpenAnimation : StateMachineBehaviour
{
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      	animator.GetComponent<Close>().endanimation = true;
    }

    
}