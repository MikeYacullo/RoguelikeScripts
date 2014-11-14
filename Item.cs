using UnityEngine;
using System.Collections;

public abstract class Item
{
	public int LevelFound;
	public int Value = 0;
	public string Name;
	public string SpriteName;
	public int DefensePowerAdj = 0;
	public int AttackPowerAdj = 0;
	public int AttackMaxDamageAdj = 0;
	public int HealthAdj = 0;
	public int VisionRangeAdj = 0;
}

public abstract class ItemConsumable : Item
{
	public bool IsConsumedOnPIckup = false;
	public abstract string Consume ();
}

public class Gold_S : ItemConsumable
{
	public Gold_S ()
	{
		SpriteName = "Gold_S";
		Value = Random.Range (1, 10);
		IsConsumedOnPIckup = true;
	}
	public override string Consume ()
	{
		return "You're rich!";
	}
}

public class PotionRed_S : ItemConsumable
{
	public PotionRed_S ()
	{
		SpriteName = "Small Red Potion";
	}
	public override string Consume ()
	{
		throw new System.NotImplementedException ();
	}
}
