using UnityEngine;

public class CloseCloud : StateMachineBehaviour
{

	 override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Debug.Log("out state");
      animator.GetComponent<CloseC>().endcloud = true;
    }

    
}