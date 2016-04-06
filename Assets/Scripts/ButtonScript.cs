using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Application.LoadLevel(1);
		}
	}
}
