using LittelSword.Enemy;
using LittelSword.Enemy.FSM;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // target은 Warrior 클래스를 의미
        Enemy enemy = target as Enemy;

        // 기본 인스팩터 그리기
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        GUI.enabled = Application.isEditor;
        //GUI.enabled = true;

        // 현재 상태 표시
        EditorGUILayout.LabelField("현재 상태", enemy.CurrentStateName);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Idle 상태"))
        {
            enemy.ChangeState<IdleState>();
        }
        if (GUILayout.Button("ChaseState 상태"))
        {
            enemy.ChangeState<ChaseState>();
        }
        if (GUILayout.Button("Attack 상태"))
        {
            enemy.ChangeState<AttackState>();
        }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
    }
}
