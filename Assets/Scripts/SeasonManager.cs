using UnityEngine;
using System.Collections;

public class SeasonManager : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	public ParticleEmitter snow;
	public Sprite winterBG;
	public Sprite summerBG;

	public const float seasonLength = 60.0f;
	public Transform wheel;

	public enum Season
	{
		SUMMER,
		WINTER
	}

	private Season currSeason;

	// Use this for initialization
	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		currSeason = Season.SUMMER;
		StartCoroutine(SeasonCycle());

		CharacterScript.SetSeason(currSeason);
	}

	void Update()
	{
		wheel.Rotate(Vector3.forward * 0.05f * seasonLength * Time.deltaTime);
	}

	private IEnumerator SeasonCycle()
	{
		yield return new WaitForSeconds(seasonLength - 5);
		snow.emit = !snow.emit;
		yield return new WaitForSeconds(5);
		if(currSeason == Season.SUMMER)
		{
			spriteRenderer.sprite = winterBG;
			currSeason = Season.WINTER;
		}
		else
		{
			spriteRenderer.sprite = summerBG;
			currSeason = Season.SUMMER;
		}

		CharacterScript.SetSeason(currSeason);
		StartCoroutine(SeasonCycle());
	}
}
