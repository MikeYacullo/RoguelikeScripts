using UnityEngine;
using System.Collections;

public class Enemy_Spider : Enemy
{
	public override void Move (Map map)
	{
		if (!Chase (map)) {
			//flap around randomly
			int dx = Random.Range (-1, 2);
			int dy = Random.Range (-1, 2);
			MoveToLocation (map, new Address (Location.x + dx, Location.y + dy));
		}
	}
	
	public Enemy_Spider (Address address)
	{
		Name = "Spider";
		MaxHealth = 5;
		CurrentHealth = MaxHealth;
		SpriteName = "enemy_spider";
		AttackPower = 1;
		DefensePower = 2;
		AttackMaxDamage = 1;
	}
}
