using UnityEngine;

namespace LittelSword.Player.Controller
{
    public class MovementController
    {
        private readonly Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;

        // 생성자
        public MovementController(Rigidbody2D rb, SpriteRenderer spriteRenderer)
        {
            this.rb = rb;
            this.spriteRenderer = spriteRenderer;
        }

        // 이동 메서드
        public void Move(Vector2 direction, float moveSpeed)
        {
            rb.linearVelocity = direction * moveSpeed;
            //  방향 전환
            if (direction != Vector2.zero)
            {
                spriteRenderer.flipX = direction.x < 0; // 현재 오른쪽을 바라보고 있음
            }
        }
    }

}
