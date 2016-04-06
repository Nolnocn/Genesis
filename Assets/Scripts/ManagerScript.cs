using UnityEngine;
using System.Collections;

public class ManagerScript : MonoBehaviour {
	
	static int charCount;

	private CharacterScript selectedCharacter;

	public Texture2D moveCursor;
	public Texture2D selCursor;
	public Texture2D defCursor;
	public Texture2D colFoodCursor;
	public Texture2D dropFoodCursor;
	public Texture2D colWoodCursor;
	public Texture2D dropWoodCursor;
	public Texture2D loveCursor;
	public Texture2D eatCursor;
	public Texture2D lightCursor;
	public Texture2D unlightCursor;

	public Transform foodCollect;
	public Transform foodDrop;
	public Transform loveMalePos;
	public Transform loveFemalePos;
	public Transform woodCollect;
	public Transform woodDrop;
	public Transform fire;

	public CharacterUIScript uiScript;

	private Vector2 cursorHotSpot = new Vector2(16, 16);
	private CursorMode cursorMode = CursorMode.ForceSoftware;

	private enum LeftClickState
	{
		NONE,
		SELECT,
		DESELECT
	}

	private enum RightClickState
	{
		NONE,
		COLLECT_WOOD,
		COLLECT_FOOD,
		DROPOFF_WOOD,
		DROPOFF_FOOD,
		EAT_FOOD,
		FIND_LOVE,
		LIGHT_FIRE,
		MOVE
	}

	private LeftClickState currLeftClickState;
	private RightClickState currRightClickState;

	// Use this for initialization
	void Start ()
	{
		charCount = 2;

		currLeftClickState = LeftClickState.NONE;
		currRightClickState = RightClickState.NONE;
		Cursor.SetCursor(defCursor, cursorHotSpot, cursorMode);

		CharacterScript adamScript = GameObject.Find("Adam").GetComponent<CharacterScript>();
		CharacterScript eveScript = GameObject.Find("Eve").GetComponent<CharacterScript>();

		adamScript.Init(CharacterScript.CHARACTER_GENDER.MALE, CharacterScript.CHARACTER_AGE.ADULT);
		eveScript.Init(CharacterScript.CHARACTER_GENDER.FEMALE, CharacterScript.CHARACTER_AGE.ADULT);

		SetScenerySortingOrder();

		/*if(Application.platform == RuntimePlatform.OSXWebPlayer)
		{
			cursorHotSpot = new Vector2(-148, 16);
		}*/
	}


	private void SetScenerySortingOrder()
	{
		GameObject ft = GameObject.Find("Food Tree");
		ft.GetComponent<SpriteRenderer>().sortingOrder =
			Mathf.FloorToInt(100 - ft.transform.position.y * 10);

		GameObject lt = GameObject.Find("Love Tree");
		lt.GetComponent<SpriteRenderer>().sortingOrder =
			Mathf.FloorToInt(100 - lt.transform.position.y * 10);

		GameObject wt = GameObject.Find("Wood Tree");
		wt.GetComponent<SpriteRenderer>().sortingOrder =
			Mathf.FloorToInt(100 - wt.transform.position.y * 10);

		GameObject fs = GameObject.Find("Food Storage");
		fs.GetComponent<SpriteRenderer>().sortingOrder =
			Mathf.FloorToInt(100 - fs.transform.position.y * 10);

		GameObject ws = GameObject.Find("Wood Storage");
		ws.GetComponent<SpriteRenderer>().sortingOrder =
			Mathf.FloorToInt(100 - ws.transform.position.y * 10);

		GameObject fire = GameObject.Find("Fire");
		fire.GetComponent<SpriteRenderer>().sortingOrder =
			Mathf.FloorToInt(100 - fire.transform.position.y * 10);
	}

	// Update is called once per frame
	void Update ()
	{
		HandleMouse();

		if(selectedCharacter != null)
		{
			if(selectedCharacter.Age == CharacterScript.CHARACTER_AGE.DEATH ||
			   selectedCharacter.ActionState == CharacterScript.CHARACTER_ACTION_STATE.MATING)
			{
				selectedCharacter = null;
				uiScript.UpdateSourceCharacter(null);
			}
		}
	}

	private void HandleMouse()
	{
		RaycastHit2D hit =
			Physics2D.Raycast((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if(hit.transform != null)
		{
			SetClickStates(hit.collider.tag);
		}
		else 
		{
			SetClickStates();
		}

		if(Input.GetMouseButtonDown(0))
		{
			if(currLeftClickState != LeftClickState.NONE)
			{
				if(currLeftClickState == LeftClickState.SELECT)
				{
					CharacterScript cs = hit.collider.GetComponent<CharacterScript>();
					if(cs.ActionState != CharacterScript.CHARACTER_ACTION_STATE.MATING
					   && cs.Age != CharacterScript.CHARACTER_AGE.DEATH)
					{
						selectedCharacter = cs;
						uiScript.UpdateSourceCharacter(cs);
					}
				}
				else if(currLeftClickState == LeftClickState.DESELECT)
				{
					selectedCharacter = null;
					Cursor.SetCursor(defCursor, cursorHotSpot, cursorMode);
				}
			}
		}
		else if(Input.GetMouseButtonDown(1))
		{
			if(currRightClickState != RightClickState.NONE && selectedCharacter != null)
			{
				switch(currRightClickState)
				{
				case RightClickState.MOVE:
				{
					selectedCharacter.SetTargetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition),
					                               CharacterScript.CHARACTER_OBJECTIVE.NONE);
					break;
				}
				case RightClickState.COLLECT_FOOD:
				{
					selectedCharacter.SetTargetPos(foodCollect.position,
					                               CharacterScript.CHARACTER_OBJECTIVE.FOOD_GATHER);
					break;
				}
				case RightClickState.DROPOFF_FOOD:
				{
					selectedCharacter.SetTargetPos(foodDrop.position,
					                               CharacterScript.CHARACTER_OBJECTIVE.FOOD_DROPOFF);
					break;
				}
				case RightClickState.EAT_FOOD:
				{
					selectedCharacter.SetTargetPos(foodDrop.position,
					                               CharacterScript.CHARACTER_OBJECTIVE.FOOD_CONSUME);
					break;
				}
				case RightClickState.COLLECT_WOOD:
				{
					selectedCharacter.SetTargetPos(woodCollect.position,
					                               CharacterScript.CHARACTER_OBJECTIVE.WOOD_GATHER);
					break;
				}
				case RightClickState.DROPOFF_WOOD:
				{
					selectedCharacter.SetTargetPos(woodDrop.position,
					                               CharacterScript.CHARACTER_OBJECTIVE.WOOD_DROPOFF);
					break;
				}
				case RightClickState.FIND_LOVE:
				{
					if(selectedCharacter.Gender == CharacterScript.CHARACTER_GENDER.MALE)
					{
						selectedCharacter.SetTargetPos(loveMalePos.position,
						                               CharacterScript.CHARACTER_OBJECTIVE.MATE);
					}
					else
					{
						selectedCharacter.SetTargetPos(loveFemalePos.position,
						                               CharacterScript.CHARACTER_OBJECTIVE.MATE);
					}
					break;
				}
				case RightClickState.LIGHT_FIRE:
				{
					selectedCharacter.SetTargetPos(fire.position,
					                               CharacterScript.CHARACTER_OBJECTIVE.KINDLE);
					break;
				}
				}
			}
		}
	}

	private void SetClickStates(string tag = null)
	{
		if(tag != null)
		{
			if(tag == "Character")
			{
				if(currLeftClickState != LeftClickState.SELECT)
				{
					Cursor.SetCursor(selCursor, cursorHotSpot, cursorMode);
					currLeftClickState = LeftClickState.SELECT;
				}
			}
			else if(selectedCharacter != null)
			{
				switch(tag)
				{
				case "FoodCollectible":
				{
					if(selectedCharacter.CanGatherFood())
					{
						Cursor.SetCursor(colFoodCursor, cursorHotSpot, cursorMode);
						currRightClickState = RightClickState.COLLECT_FOOD;
					}
					break;
				}
				case "FoodStorage":
				{
					if(selectedCharacter.ActionState == CharacterScript.CHARACTER_ACTION_STATE.CARRYING_FOOD)
					{
						Cursor.SetCursor(dropFoodCursor, cursorHotSpot, cursorMode);
						currRightClickState = RightClickState.DROPOFF_FOOD;
					}
					else
					{
						Cursor.SetCursor(eatCursor, cursorHotSpot, cursorMode);
						currRightClickState = RightClickState.EAT_FOOD;
					}
					break;
				}
				case "LoveTree":
				{
					if(!LoveTreeScript.IsTreeFull() && selectedCharacter.CanMate())
					{
						Cursor.SetCursor(loveCursor, cursorHotSpot, cursorMode);
						currRightClickState = RightClickState.FIND_LOVE;
					}
					break;
				}
				case "WoodCollectible":
				{
					if(selectedCharacter.CanGatherWood())
					{
						Cursor.SetCursor(colWoodCursor, cursorHotSpot, cursorMode);
						currRightClickState = RightClickState.COLLECT_WOOD;
					}
					break;
				}
				case "WoodStorage":
				{
					if(selectedCharacter.ActionState == CharacterScript.CHARACTER_ACTION_STATE.CARRYING_WOOD)
					{
						Cursor.SetCursor(dropWoodCursor, cursorHotSpot, cursorMode);
						currRightClickState = RightClickState.DROPOFF_WOOD;
					}
					break;
				}
				case "Fire":
				{
					if(selectedCharacter.ActionState == CharacterScript.CHARACTER_ACTION_STATE.IDLE)
					{
						if(ResourceManager.IsFireLit())
						{
							Cursor.SetCursor(unlightCursor, cursorHotSpot, cursorMode);
							currRightClickState = RightClickState.LIGHT_FIRE;
						}
						else if(!ResourceManager.IsWoodEmpty())
						{
							Cursor.SetCursor(lightCursor, cursorHotSpot, cursorMode);
							currRightClickState = RightClickState.LIGHT_FIRE;
						}
					}
					break;
				}
				}
			}
		}
		else
		{
			if(selectedCharacter != null)
			{
				if(currLeftClickState != LeftClickState.DESELECT)
				{
					Cursor.SetCursor(defCursor, cursorHotSpot, cursorMode);
					currLeftClickState = LeftClickState.DESELECT;
				}
				
				if(currLeftClickState == LeftClickState.DESELECT || currRightClickState != RightClickState.MOVE)
				{
					Cursor.SetCursor(moveCursor, cursorHotSpot, cursorMode);
					currRightClickState = RightClickState.MOVE;
				}
			}
			else
			{
				if(currLeftClickState != LeftClickState.NONE)
				{
					Cursor.SetCursor(defCursor, cursorHotSpot, cursorMode);
					currLeftClickState = LeftClickState.NONE;
					currRightClickState = RightClickState.NONE;
				}
			}
		}
	}

	public static void CharacterBorn()
	{
		++charCount;
	}

	public static void CharacterDied()
	{
		--charCount;
		if(charCount <= 0)
		{
			Application.LoadLevel(2);
		}
	}
}
