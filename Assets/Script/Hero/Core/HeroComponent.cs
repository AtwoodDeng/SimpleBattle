using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroComponent : MBehavior {

	public Hero parent;

	protected override void MStart ()
	{
		base.MStart ();
		if (parent == null)
			parent = GetComponent<Hero>();
		if (parent == null && transform.parent != null)
			parent = transform.parent.GetComponent<Hero> ();
	}

	public virtual void Init( Hero hero )
	{
		parent = hero;
	}
}
