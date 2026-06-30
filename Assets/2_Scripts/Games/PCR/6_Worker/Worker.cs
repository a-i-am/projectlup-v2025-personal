using UnityEngine;

namespace LUP.PCR
{


    public class Worker : MonoBehaviour
    {
        private Animator anim;
        private UnitMover mover;

        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int IsClimbingHash = Animator.StringToHash("IsClimbing");

        private static readonly int ActionStateHash = Animator.StringToHash("ActionState");
        private void Awake()
        {
            mover = GetComponent<UnitMover>();
        }

        public void InitAnimator()
        {
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {


            if (mover == null || anim == null)
            {
                return;
            }

            int currentAction = anim.GetInteger(ActionStateHash);

            if (currentAction == 0)
            {
                anim.SetBool(IsMovingHash, mover.IsMoving);
            }
            else
            {
                anim.SetBool(IsMovingHash, false);
            }

            anim.SetBool(IsClimbingHash, mover.IsClimbing);
        }
        public void SetActionState(WorkerActionState state)
        {
            if (anim != null)
            {
                anim.SetInteger(ActionStateHash, (int)state);
            }
        }
    }


}
