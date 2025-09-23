using UnityEngine;

namespace LittelSword.Player.Controller
{
    public class AnimationController
    {
        private readonly Animator animator;

        // �ִϸ��̼� �Ķ���� �ؽ�
        private static readonly int hashIsRun = Animator.StringToHash("IsRun");
        private static readonly int hashAttack = Animator.StringToHash("Attack");
        private static readonly int hashDie = Animator.StringToHash("Die");
        private static readonly int hashHit = Animator.StringToHash("Hit");

        // ������(Depency Injection)
        public AnimationController(Animator animator)
        {
            this.animator = animator;
        }

        // �ִϸ��̼� ���� ���� �޼ҵ�
        public void Move(bool isMoving)
        {
            animator.SetBool(hashIsRun, isMoving);
        }
        public void Attack()
        {
            animator.SetTrigger(hashAttack);
        }
        public void Die()
        {
            animator.SetTrigger(hashDie);
        }
        public void Hit()
        {
            animator.SetTrigger(hashHit);
        }
    }

}
