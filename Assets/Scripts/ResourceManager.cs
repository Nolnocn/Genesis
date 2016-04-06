using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {

	public static int foodStorageAmt;
	public static int woodStorageAmt;
	static bool fireLit;

	static bool updateFire;
	private SpriteRenderer fireSpriteRenderer;

	public Sprite[] fireSprites;

	// Use this for initialization
	void Start ()
	{
		fireLit = false;
		foodStorageAmt = 10;
		woodStorageAmt = 5;

		updateFire = false;

		fireSpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(updateFire)
		{
			if(fireLit)
			{
				StartCoroutine(BurnWood());
				updateFire = false;
			}

			UpdateFireSprite();
		}
	}

	public static bool IsFoodEmpty()
	{
		return foodStorageAmt <= 0;
	}

	public static bool IsWoodEmpty()
	{
		return woodStorageAmt <= 0;
	}

	public static void ChangeFood(int amt)
	{
		foodStorageAmt += amt;
	}
	
	public static void ChangeWood(int amt)
	{
		woodStorageAmt += amt;
	}

	public static bool IsFireLit()
	{
		return fireLit;
	}

	public static void ToggleFire()
	{
		fireLit = !fireLit;
		updateFire = true;
	}

	private IEnumerator BurnWood()
	{
		ChangeWood(-1);
		yield return new WaitForSeconds(20);
		if(fireLit)
		{
			if(!IsWoodEmpty())
			{
				StartCoroutine(BurnWood());
			}
			else
			{
				ToggleFire();
			}
		}
	}

	private void UpdateFireSprite()
	{
		if(fireLit)
		{
			fireSpriteRenderer.sprite = fireSprites[1];
		}
		else
		{
			fireSpriteRenderer.sprite = fireSprites[0];
		}
	}
}
