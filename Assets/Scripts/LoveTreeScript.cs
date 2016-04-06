using UnityEngine;
using System.Collections;

public class LoveTreeScript : MonoBehaviour {
	
	static CharacterScript maleSlot;
	static CharacterScript femaleSlot;

	private bool spawning;

	public Transform kidSpawnPoint;
	public GameObject kidPrefab;
	public ParticleSystem[] loveParticles;

	void Start ()
	{
		loveParticles[0].GetComponent<Renderer>().sortingLayerName = "foreground";
		loveParticles[0].GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 50;

		loveParticles[1].GetComponent<Renderer>().sortingLayerName = "foreground";
		loveParticles[1].GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 50;
	}
	
	void Update ()
	{
		CheckSlots();

		if(maleSlot != null)
		{
			if(maleSlot.Objective != CharacterScript.CHARACTER_OBJECTIVE.MATE)
			{
				RemoveCharacter(maleSlot);
			}
		}

		if(femaleSlot != null)
		{
			if(femaleSlot.Objective != CharacterScript.CHARACTER_OBJECTIVE.MATE)
			{
				RemoveCharacter(femaleSlot);
			}
		}
	}

	private void CheckSlots()
	{
		if(maleSlot != null && femaleSlot != null)
		{
			if(maleSlot.ActionState == CharacterScript.CHARACTER_ACTION_STATE.WAITING)
			{
				if(femaleSlot.ActionState == CharacterScript.CHARACTER_ACTION_STATE.WAITING)
				{
					if(!spawning)
					{
						DoTheDeed();
					}
				}
			}
		}
	}

	private void DoTheDeed()
	{
		spawning = true;

		loveParticles[0].Play();
		loveParticles[1].Play();

		maleSlot.StartMating();
		femaleSlot.StartMating();

		StartCoroutine(SpawnDelay());
	}

	private IEnumerator SpawnDelay()
	{
		yield return new WaitForSeconds(3.0f);
		maleSlot.DoneMating();
		femaleSlot.DoneMating();
		int rand = Random.Range(0, 2);
		GameObject kid = Instantiate(kidPrefab, kidSpawnPoint.position, Quaternion.identity) as GameObject;
		if(rand == 0)
		{
			kid.GetComponent<CharacterScript>().Init(CharacterScript.CHARACTER_GENDER.MALE);
		}
		else
		{
			kid.GetComponent<CharacterScript>().Init(CharacterScript.CHARACTER_GENDER.FEMALE);
		}

		maleSlot = null;
		femaleSlot = null;

		spawning = false;

		ManagerScript.CharacterBorn();
	}

	public static bool IsOccupied(CharacterScript.CHARACTER_GENDER gender)
	{
		if(gender == CharacterScript.CHARACTER_GENDER.MALE)
		{
			return maleSlot != null;
		}
		else
		{
			return femaleSlot != null;
		}
	}

	public static void AddCharacter(CharacterScript cs)
	{
		if(cs.Gender == CharacterScript.CHARACTER_GENDER.MALE)
		{
			maleSlot = cs;
		}
		else
		{
			femaleSlot = cs;
		}
	}

	public static void RemoveCharacter(CharacterScript cs)
	{
		if(cs == maleSlot)
		{
			maleSlot = null;
		}
		else if(cs == femaleSlot)
		{
			femaleSlot = null;
		}
	}

	public static bool IsTreeFull()
	{
		return maleSlot != null && femaleSlot != null;
	}
}
