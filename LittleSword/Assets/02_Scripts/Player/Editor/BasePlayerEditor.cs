using LittelSword.Player;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Warrior))]
public class BasePlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // target은 Warrior 클래스를 의미
        BasePlayer basePlayer = target as BasePlayer;

        // 기본 인스팩터 그리기
        DrawDefaultInspector();

        // PlayerStats 필드
        basePlayer.playerStats.maxHp =
            EditorGUILayout.IntField("Max HP", basePlayer.playerStats.maxHp);

        // 현재 HP 필드
        EditorGUILayout.LabelField("Current HP", basePlayer.CurrentHP.ToString());

        // 버튼 
        if (GUILayout.Button("피격"))
        {
            basePlayer.TakeDamage(10);
        }

        if (GUILayout.Button("초기화"))
        {
            basePlayer.CurrentHP = basePlayer.playerStats.maxHp;
        }

    }
}
