using UnityEngine;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{

	public string ClassName;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void CreatePC (string className)
	{	
		PlayerPrefs.SetString ("className", className);
		Application.LoadLevel ("Main");
	}
}
