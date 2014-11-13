using UnityEngine;
using System.Collections;

public class PlayerCharacter : Actor
{

	public enum ClassType
	{
		None = 0
		,
		Cleric = 1
		,
		Fighter = 2
		,
		Rogue = 3
		,
		Wizard = 4
	}
		
	public ClassType classType;
	
	public bool IsMale;
		
	public PlayerCharacter ()
	{
		Name = "Player";
		AttackPower = 1;
		DefensePower = 1;
		AttackMaxDamage = 5;
		MaxHealth = 10;
		CurrentHealth = 10;
		this.VisionRange = 6;
	}
	
	public PlayerCharacter (ClassType classType, bool isMale)
	{
		this.classType = classType;
		IsMale = isMale;
		Name = classType.ToString ();
		switch (this.classType) {
		case ClassType.Cleric:
			AttackPower = 3;
			DefensePower = 4;
			AttackMaxDamage = 5;
			MaxHealth = 10;
			CurrentHealth = 10;
			VisionRange = 6;
			break;
		case ClassType.Fighter:
			AttackPower = 2;
			DefensePower = 2;
			AttackMaxDamage = 5;
			MaxHealth = 10;
			CurrentHealth = 10;
			VisionRange = 6;
			break;
		case ClassType.Rogue:
			AttackPower = 2;
			DefensePower = 2;
			AttackMaxDamage = 5;
			MaxHealth = 10;
			CurrentHealth = 10;
			VisionRange = 8;
			break;
		case ClassType.Wizard:
			AttackPower = 4;
			DefensePower = 2;
			AttackMaxDamage = 5;
			MaxHealth = 6;
			CurrentHealth = 6;
			VisionRange = 6;
			break;
		default:
			break;
		}
	}
	
		
}
