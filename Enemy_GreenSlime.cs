using UnityEngine;
using System.Collections;

public class Enemy_GreenSlime : Enemy {
	public override void Move(Map map){
		if(!Chase (map)){
			//flap around randomly
			int dx = Random.Range(-1,2);
			int dy = Random.Range(-1,2);
			MoveToLocation(map,new Address(Location.x+dx, Location.y+dy));
		}
	}
	
	public Enemy_GreenSlime(Address address){
		this.Location = address;
		Name = "Green Slime";
		MaxHealth = 10;
		CurrentHealth = MaxHealth;
		SpriteName = "enemy_greenslime";
		AttackPower = 2;
		DefensePower = 1;
		AttackMaxDamage = 1;
	}
}
