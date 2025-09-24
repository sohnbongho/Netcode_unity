using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "LittleSword/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Basic Stats")]
    public int maxHp = 100;
    public float moveSpeed = 3f;
    
    [Header("Detection Stats")]
    public float detectInterval = 0.3f;

    [Header("Combat Stats")]
    public float chaseDistance = 5f;
    public float attackDistance = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

}
