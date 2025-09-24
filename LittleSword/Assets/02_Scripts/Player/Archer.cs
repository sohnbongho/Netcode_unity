using LittleSword.Player.Weapon;
using UnityEngine;

namespace LittelSword.Player
{
    public class Archer : BasePlayer
    {
        // 화살 프리팹
        [SerializeField] private GameObject arrowPrefab;
        // 화살 발사 위치
        [SerializeField] private Transform firePoint;

        //애니메이션 이벤트 호출할 메소드
        public void OnArcherAttackEvent()
        {
            FireArrow();
        }

        private void FireArrow()
        {
            float yValue = spriteRenderer.flipX ? 180 : 0;
            Quaternion rot = Quaternion.Euler(0, yValue, 0);
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, rot);
            arrow.GetComponent<Arrow>().Init(playerStats.fireForce, playerStats.attackDamage);
        }
    }
}
