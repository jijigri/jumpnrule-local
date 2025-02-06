using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy), true)]
[CanEditMultipleObjects]
public class EnemyEditor : Editor
{
    private SerializedProperty _maxHealth;
    private SerializedProperty _weight;
    private SerializedProperty _moneyDrop;
    private SerializedProperty canRespawn;
    private SerializedProperty enemyType;
    private SerializedProperty _falterInvulnerabilityTime;
    private SerializedProperty _damageNeededToFalter;

    private SerializedProperty _falterLockTime;
    private SerializedProperty _hitstunTime;
    private SerializedProperty _timeBeforeResettingFalter;

    private SerializedProperty _damageSound;
    private SerializedProperty _deathSound;

    private string[] _tabs = { "General Fields", "Falter Fields", "Effects Fields", "Other Fields"};
    private int _tabSelected = 0;

    public override void OnInspectorGUI()
    {
        Enemy enemy = (Enemy)target;

        FindProperties();

        EditorGUILayout.BeginVertical();
        _tabSelected = GUILayout.Toolbar(_tabSelected, _tabs);
        EditorGUILayout.EndVertical();

        serializedObject.Update();

        if(_tabSelected >= 0 && _tabSelected < _tabs.Length)
        {
            switch (_tabSelected)
            {
                case 0:
                    DisplayEnemyOptions(enemy);
                    break;
                case 1:
                    DisplayFalterOptions();
                    break;
                case 2:
                    DisplayEffectsOptions();
                    break;
                case 3:
                    DisplayOtherOptions();
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    void FindProperties()
    {
        _maxHealth = serializedObject.FindProperty("_maxHealth");
        _weight = serializedObject.FindProperty("_weight");
        _moneyDrop = serializedObject.FindProperty("_moneyDrop");
        canRespawn = serializedObject.FindProperty("canRespawn");
        enemyType = serializedObject.FindProperty("enemyType");

        _falterInvulnerabilityTime = serializedObject.FindProperty("_falterInvulnerabilityTime");
        _damageNeededToFalter = serializedObject.FindProperty("_damageNeededToFalter");
        _falterLockTime = serializedObject.FindProperty("_falterLockTime");
        _hitstunTime = serializedObject.FindProperty("_hitStopTime");
        _timeBeforeResettingFalter = serializedObject.FindProperty("_timeBeforeResettingFalter");

        _damageSound = serializedObject.FindProperty("_damageSound");
        _deathSound = serializedObject.FindProperty("_deathSound");
    }

    void DisplayEnemyOptions(Enemy enemy)
    {
        EditorGUILayout.PropertyField(_maxHealth, new GUIContent("Max Health", "Enemy's max health"));
        EditorGUILayout.PropertyField(_weight, new GUIContent("Weight", "Enemy's weight"));
        EditorGUILayout.PropertyField(_moneyDrop, new GUIContent("Money Drop", "Number of Coins the enemy will drop"));
        EditorGUILayout.PropertyField(canRespawn, new GUIContent("Can Respawn", "Can the enemy respawn during the round"));
        EditorGUILayout.PropertyField(enemyType, new GUIContent("Enemy Type", "The type of the enemy"));
        enemy.tier = EditorGUILayout.IntField("Enemy Tier", enemy.tier);
    }

    void DisplayFalterOptions()
    {
        EditorGUILayout.PropertyField(_falterInvulnerabilityTime, new GUIContent("Falter invulnerability Time", "Time until being able to falter enemy after a falter"));
        EditorGUILayout.PropertyField(_damageNeededToFalter, new GUIContent("Damage needed to falter", "Percentage of the enemy health needed to falter"));
        EditorGUILayout.PropertyField(_falterLockTime, new GUIContent("Falter stun time", "Time the enemy stays in falter state"));
        EditorGUILayout.PropertyField(_hitstunTime, new GUIContent("Falter hitstop time", "Time the enemy stays in hitstun after a falter"));
        EditorGUILayout.PropertyField(_timeBeforeResettingFalter, new GUIContent("Falter damage time window", "Time before the enemy falter's value resets"));
    }

    void DisplayEffectsOptions()
    {
        EditorGUILayout.PropertyField(_damageSound, new GUIContent("Damage Sound", "The sound the enemy makes when taking damage"));
        EditorGUILayout.PropertyField(_deathSound, new GUIContent("Death Sound", "The sound the enemy makes when dying"));
    }

    public virtual void DisplayOtherOptions()
    {
        DrawDefaultInspector();
    }
}
