using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryController : MonoBehaviour
{
	public GameObject inventorySlot;
	// Use this for initialization
	void Start ()
	{
		int size = 60;
		int startX = -120;
		int startY = 125;
		for (int r=0; r<5; r++) {
			for (int c=0; c<5; c++) {
				GameObject slot = (GameObject)Instantiate (inventorySlot);
				slot.transform.parent = this.gameObject.transform;
				slot.name = "backpackSlot" + (r * 5 + c);
				slot.GetComponent<RectTransform> ().localPosition = new Vector3 (startX + (c * size), startY - (r * size), 0);
			}
		}
	}
	
}
