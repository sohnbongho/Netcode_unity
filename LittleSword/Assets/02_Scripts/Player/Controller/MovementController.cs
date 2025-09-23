using UnityEngine;

namespace LittelSword.Player.Controller
{
    public class MovementController
    {
        private readonly Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;

        // ������
        public MovementController(Rigidbody2D rb, SpriteRenderer spriteRenderer)
        {
            this.rb = rb;
            this.spriteRenderer = spriteRenderer;
        }

        // �̵� �޼���
        public void Move(Vector2 direction, float moveSpeed)
        {
            rb.linearVelocity = direction * moveSpeed;
            //  ���� ��ȯ
            if (direction != Vector2.zero)
            {
                spriteRenderer.flipX = direction.x < 0; // ���� �������� �ٶ󺸰� ����
            }
        }
    }

}
