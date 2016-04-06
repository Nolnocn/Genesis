using UnityEngine;
using System.Collections;

public class CharacterAnimationScript : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator>();
	}

	public void Init()
	{
		animator = GetComponent<Animator>();
	}

	public void SetGender(bool male)
	{
		animator.SetBool("male", male);
	}

	public void SetFacing(float charPosX, float targetPosX)
	{
		if(charPosX < targetPosX)
		{
			transform.localScale = new Vector3(-1, 1, 1);
		}
		else
		{
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public void SetToYoung()
	{
		animator.SetBool("young", true);
		animator.SetBool("adult", false);
		animator.SetBool("old", false);
	}

	public void SetToAdult()
	{
		animator.SetBool("young", false);
		animator.SetBool("adult", true);
		animator.SetBool("old", false);
	}

	public void SetToOld()
	{
		animator.SetBool("young", false);
		animator.SetBool("adult", false);
		animator.SetBool("old", true);
	}

	public void SetToMoving()
	{
		animator.SetBool("moving", true);
	}

	public void SetToIdle()
	{
		animator.SetBool("moving", false);
	}

	public void SetToDeath()
	{
		animator.SetTrigger("dead");
	}

	private void Die()
	{
		ManagerScript.CharacterDied();
		Destroy(gameObject);
	}
}
