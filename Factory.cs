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
	
	
	static Factory ()
	{
		
	}
	
	public static Enemy GetEnemyForLevel (int level)
	{
		List<EnemyType> enemies = new List<EnemyType> ();
		switch (level) {
		case 0:
			enemies = new List<EnemyType>{EnemyType.Bat,EnemyType.Spider,EnemyType.GreenSlime};
			break;
		default:
			break;
		}
		EnemyType type = enemies [Random.Range (0, enemies.Count)];
		return CreateEnemy (type, level);
	}

	public static Enemy CreateEnemy (EnemyType eType, int level)
	{
		Enemy enemy = new Enemy ();
		switch (eType) {
		case EnemyType.Bat:
			enemy = NewBat ();
			break;
		case EnemyType.GreenSlime:
			enemy = NewGreenSlime ();
			break;
		case EnemyType.Spider:
			enemy = NewSpider ();
			break;
		default:
			//this should never happen!
			break;
		}
		if (Random.Range (0, 10) == 0) {
			enemy = Mutator.Mutate (enemy, level + 1);
		}
		return enemy;
	}

	public static Enemy NewBat ()
	{
		Enemy enemy = new Enemy ();
		enemy.Name = "Bat";
		enemy.SpriteName = "enemy_bat";
		enemy.Stats.MaxHealth = 5;
		enemy.Stats.CurrentHealth = enemy.Stats.MaxHealth;
		enemy.Stats.AttackPower = 1;
		enemy.Stats.DefensePower = 1;
		enemy.Stats.AttackMaxDamage = 1;
		return enemy;
	}
	
	public static Enemy NewGreenSlime ()
	{
		Enemy enemy = new Enemy ();
		enemy.Name = "Green Slime";
		enemy.SpriteName = "enemy_greenslime";
		enemy.Stats.MaxHealth = 5;
		enemy.Stats.CurrentHealth = enemy.Stats.MaxHealth;
		enemy.Stats.AttackPower = 1;
		enemy.Stats.DefensePower = 1;
		enemy.Stats.AttackMaxDamage = 1;
		return enemy;
	}
	
	public static Enemy NewSpider ()
	{
		Enemy enemy = new Enemy ();
		enemy.Name = "Spider";
		enemy.SpriteName = "enemy_spider";
		enemy.Stats.MaxHealth = 5;
		enemy.Stats.CurrentHealth = enemy.Stats.MaxHealth;
		enemy.Stats.AttackPower = 1;
		enemy.Stats.DefensePower = 1;
		enemy.Stats.AttackMaxDamage = 1;
		return enemy;
	}



}
