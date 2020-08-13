using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : StateMachineBehaviour
{
    private const float PLAYER_MOVE_SPEED = 10.0f;

    private enum FACING_DIRECTION
    {
        LEFT, RIGHT
    }

    private SpriteRenderer playerSpriteRenderer;

    private FACING_DIRECTION currentFacingDirection = FACING_DIRECTION.RIGHT;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerSpriteRenderer = animator.gameObject.GetComponent<SpriteRenderer>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            animator.transform.position = new Vector3(animator.transform.position.x - PLAYER_MOVE_SPEED * Time.deltaTime, animator.transform.position.y, animator.transform.position.z);

            if (currentFacingDirection == FACING_DIRECTION.RIGHT)
            {
                playerSpriteRenderer.flipX = true;
                currentFacingDirection = FACING_DIRECTION.LEFT;
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            animator.transform.position = new Vector3(animator.transform.position.x + PLAYER_MOVE_SPEED * Time.deltaTime, animator.transform.position.y, animator.transform.position.z);

            if (currentFacingDirection == FACING_DIRECTION.LEFT)
            {
                playerSpriteRenderer.flipX = false;
                currentFacingDirection = FACING_DIRECTION.RIGHT;
            }
        }

        if ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            animator.SetBool("isWalking", false);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
