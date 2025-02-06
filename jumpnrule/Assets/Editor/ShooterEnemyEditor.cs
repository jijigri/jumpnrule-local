using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShooterEnemy), true)]
[CanEditMultipleObjects]
public class ShooterEnemyEditor : EnemyEditor
{
    public override void DisplayOtherOptions()
    {
        base.DisplayOtherOptions();

        ShooterEnemy shooterEnemy = (ShooterEnemy)target;

        if (shooterEnemy.bulletToShoot != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Bullet Settings", EditorStyles.boldLabel);

            shooterEnemy._bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", shooterEnemy._bulletSpeed);
            shooterEnemy._bulletDamage = EditorGUILayout.FloatField("Bullet Damage", shooterEnemy._bulletDamage);
            shooterEnemy._bulletLifetime = EditorGUILayout.FloatField("Bullet Lifetime", shooterEnemy._bulletLifetime);

            GUILayout.Space(10);
            GUILayout.Label("Shooter Settings", EditorStyles.boldLabel);

            shooterEnemy.aimsAtPlayers = EditorGUILayout.Toggle("Aims at players", shooterEnemy.aimsAtPlayers);

            GUILayout.Space(10);

            shooterEnemy.numberOfBulletsPerShot = EditorGUILayout.IntField("Number of Bullets per Shot", shooterEnemy.numberOfBulletsPerShot);

            shooterEnemy.bulletSpreadInDegrees = EditorGUILayout.FloatField("Bullet Spread in Degrees", shooterEnemy.bulletSpreadInDegrees);
            shooterEnemy.shotsInaccuracy = EditorGUILayout.FloatField("Shots Inaccuracy", shooterEnemy.shotsInaccuracy);

            GUILayout.Space(10);

            shooterEnemy.numberOfBulletsBeforeReloading = EditorGUILayout.IntField("Number of Bullets Before Reloading", shooterEnemy.numberOfBulletsBeforeReloading);
            shooterEnemy.timeBetweenShots = EditorGUILayout.FloatField("Time Between Shots", shooterEnemy.timeBetweenShots);

            shooterEnemy.stopsAfterEmptyingMagazine = EditorGUILayout.Toggle("Stops After Emptying Magazine", shooterEnemy.stopsAfterEmptyingMagazine);

            if (shooterEnemy.stopsAfterEmptyingMagazine == false)
            {
                shooterEnemy.timeToReload = EditorGUILayout.FloatField("Time to Reload", shooterEnemy.timeToReload);
            }
        }

        if (!shooterEnemy.aimsAtPlayers && shooterEnemy.bulletToShoot != null)
        {
            shooterEnemy.shootDirection = EditorGUILayout.FloatField("Shoot Direction", shooterEnemy.shootDirection);
        }
    }
}
