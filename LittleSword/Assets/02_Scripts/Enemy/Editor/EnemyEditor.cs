using LittelSword.Enemy;
using LittelSword.Enemy.FSM;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // target�� Warrior Ŭ������ �ǹ�
        Enemy enemy = target as Enemy;

        // �⺻ �ν����� �׸���
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        GUI.enabled = Application.isEditor;
        //GUI.enabled = true;

        // ���� ���� ǥ��
        EditorGUILayout.LabelField("���� ����", enemy.CurrentStateName);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Idle ����"))
        {
            enemy.ChangeState<IdleState>();
        }
        if (GUILayout.Button("ChaseState ����"))
        {
            enemy.ChangeState<ChaseState>();
        }
        if (GUILayout.Button("Attack ����"))
        {
            enemy.ChangeState<AttackState>();
        }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
    }
}
