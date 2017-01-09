using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	public Hero parent;

	void Awake()
	{
		if (parent == null)
			parent = GetComponent<Hero>();
		if (parent == null && transform.parent != null)
			parent = transform.parent.GetComponent<Hero> ();
	}

	public virtual float MoveTo( Block target)
	{
		return LogicManager.moveDuration;
	}
}
