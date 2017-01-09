using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strategy : MonoBehaviour {
	public Hero parent;

	void Awake()
	{
		if (parent == null)
			parent = GetComponent<Hero>();
		if (parent == null && transform.parent != null)
			parent = transform.parent.GetComponent<Hero> ();
	}

	public virtual Block GetTarget()
	{
		return parent.TemBlock;
	}

	public virtual Direction GetDirection()
	{
		return Direction.Left;
	}
}
