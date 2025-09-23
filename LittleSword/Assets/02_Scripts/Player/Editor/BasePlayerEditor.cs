using LittelSword.Player;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Warrior))]
public class BasePlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // target�� Warrior Ŭ������ �ǹ�
        BasePlayer basePlayer = target as BasePlayer;

        // �⺻ �ν����� �׸���
        DrawDefaultInspector();

        // PlayerStats �ʵ�
        basePlayer.playerStats.maxHp =
            EditorGUILayout.IntField("Max HP", basePlayer.playerStats.maxHp);

        // ���� HP �ʵ�
        EditorGUILayout.LabelField("Current HP", basePlayer.CurrentHP.ToString());

        // ��ư 
        if (GUILayout.Button("�ǰ�"))
        {
            basePlayer.TakeDamage(10);
        }

        if (GUILayout.Button("�ʱ�ȭ"))
        {
            basePlayer.CurrentHP = basePlayer.playerStats.maxHp;
        }

    }
}
