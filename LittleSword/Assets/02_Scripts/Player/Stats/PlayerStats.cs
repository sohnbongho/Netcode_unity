using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "LittleSword/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public int maxHp = 100;
    public float moveSpeed = 5;
    public int attackDamage = 20;
    public float fireForce = 10;

}
