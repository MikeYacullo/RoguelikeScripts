using UnityEngine;
using System.Collections;

public abstract class Item
{
	public int Value = 0;
	public Address Location;
	public string Name;
	public string SpriteName;
	public int DefensePowerAdj = 0;
	public int AttackPowerAdj = 0;
	public int AttackMaxDamageAdj = 0;
	public int HealthAdj = 0;
	public int VisionRangeAdj = 0;
}

public enum ItemSlotType
{
	Weapon,
	Armor,
	Boots,
	Helm,
	Ring
}

public abstract class ItemEquippable
{
	public ItemSlotType EquipSlot;
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
		SpriteName = "gold_s";
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
		SpriteName = "potionRed_s";
	}
	public override string Consume ()
	{
		throw new System.NotImplementedException ();
	}
}
