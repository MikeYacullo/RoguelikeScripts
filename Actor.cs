using UnityEngine;
using System.Collections;

public class Actor : Object {

	public string Name;
	public Address Location = new Address(0,0);
	public int MaxHealth;
	public int CurrentHealth;
	public int VisionRange;
	public int AttackPower;
	public int DefensePower;
	public int AttackMaxDamage;
	
}
