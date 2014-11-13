using UnityEngine;
using System.Collections;

public class Enemy_Bat : Enemy {
	public override void Move(Map map){
		if(!Chase (map)){
			//flap around randomly
			int dx = Random.Range(-1,2);
			int dy = Random.Range(-1,2);
			MoveToLocation(map,new Address(Location.x+dx, Location.y+dy));
		}
	}
	
	public Enemy_Bat(Address address){
		this.Location = address;
		Name = "Bat";
		SpriteName = "enemy_bat";
		MaxHealth = 5;
		CurrentHealth = MaxHealth;
		AttackPower = 1;
		DefensePower = 1;
		AttackMaxDamage = 1;
	}
}
