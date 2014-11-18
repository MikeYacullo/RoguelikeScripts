using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Factory
{

	public enum EnemyType
	{
		Bat
	,
		GreenSlime
	,
		Spider	
	}
/*
	public static Enemy CreateEnemy (EnemyType eType)
	{
		switch (eType) {
		case EnemyType.Bat:
			return NewBat ();
			break;
		default:
			break;
		}
	}

	public static Enemy NewBat ()
	{
		Enemy enemy = new Enemy ();
		enemy.Name = "Bat";
		enemy.SpriteName = "enemy_bat";
		enemy.MaxHealth = 5;
		enemy.CurrentHealth = enemy.MaxHealth;
		enemy.AttackPower = 1;
		enemy.DefensePower = 1;
		enemy.AttackMaxDamage = 1;
		return enemy;
	}
*/


}
