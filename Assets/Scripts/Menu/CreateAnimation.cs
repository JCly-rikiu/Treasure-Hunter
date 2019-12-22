using UnityEngine;

public class CreateAnimation : StateMachineBehaviour
{

    

   


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Debug.Log("out state");
      animator.GetComponent<OpenClose>().endanimation = true;
    }

    
}