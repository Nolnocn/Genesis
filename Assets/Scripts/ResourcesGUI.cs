using UnityEngine;
using System.Collections;

public class ResourcesGUI : MonoBehaviour 
{
    public Texture2D resource_Wood;
    public Texture2D resource_Food;

    private readonly Vector2 WOOD_ICON_POSITION = new Vector2(100.0f, 1.5f);
    private readonly Vector2 FOOD_ICON_POSITION = new Vector2(0.5f, 1.5f);
    private readonly Vector2 FOOD_LABEL_POSITION = new Vector2(50.0f, 10.0f);
    private readonly Vector2 WOOD_LABEL_POSITION = new Vector2(150.0f, 10.0f);

    private const float ICON_WIDTH = 40.0f;
    private const float ICON_HEIGHT = 40.0f;
    private const float LABEL_WIDTH = 40.0f;
    private const float LABEL_HEIGHT = 100.0f;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnGUI()
    {
        GUIContent contentFood;
        GUIContent contentWood;

        // Food -----------------------------------------------------------------

        // draw the food icon
        GUI.BeginGroup(new Rect(FOOD_ICON_POSITION.x, FOOD_ICON_POSITION.y, ICON_WIDTH, ICON_HEIGHT));
        GUI.DrawTexture(new Rect(0, 0, ICON_WIDTH, ICON_HEIGHT), resource_Food);
        GUI.EndGroup();

        // draw the label for the food
        contentFood = new GUIContent( ResourceManager.foodStorageAmt.ToString() );

        GUI.Label(new Rect(FOOD_LABEL_POSITION.x, FOOD_LABEL_POSITION.y, LABEL_WIDTH, LABEL_HEIGHT), contentFood);

        //text_Food.text = ResourceManager.foodStorageAmt.ToString();
        //text_Food.transform.position = new Vector3(FOOD_LABEL_POSITION.y, FOOD_LABEL_POSITION.x, 0.0f);

        // Wood ------------------------------------------------------------------
        GUI.BeginGroup(new Rect(WOOD_ICON_POSITION.x, WOOD_ICON_POSITION.y, ICON_WIDTH, ICON_HEIGHT));
        GUI.DrawTexture(new Rect(0, 0, ICON_WIDTH, ICON_HEIGHT), resource_Wood);
        GUI.EndGroup();

        // draw the label for the food
        contentWood = new GUIContent(ResourceManager.woodStorageAmt.ToString());

        GUI.Label(new Rect(WOOD_LABEL_POSITION.x, WOOD_LABEL_POSITION.y, LABEL_WIDTH, LABEL_HEIGHT), contentWood);
    }
}
