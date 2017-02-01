using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strategy : HeroComponent {

	public virtual SimBlock GetTarget()
	{
		return parent.TemSimpleBlock;
	}

	public virtual Direction GetDirection()
	{
		return Direction.Left;
	}
}
