using UnityEngine;

public class CloseCloud : StateMachineBehaviour
{

	 override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      animator.GetComponent<CloseC>().endcloud = true;
    }

    
}