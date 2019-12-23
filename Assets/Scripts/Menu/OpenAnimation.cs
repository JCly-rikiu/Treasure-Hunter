using UnityEngine;

public class OpenAnimation : StateMachineBehaviour
{
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      	Debug.Log("out state");
      	animator.GetComponent<Close>().endanimation = true;
    }

    
}