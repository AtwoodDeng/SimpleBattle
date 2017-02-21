using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualHero : Hero {

	public override void Init ()
	{
		base.Init ();
		transform.position = new Vector3 (999f, 999f, 999f);
	}
//	public override void SetBlock (SimBlock block)
//	{
//		base.SetBlock (block);
//
//		if ( TemBlock != null )
//			transform.position = TemBlock.GetCenterPosition();
//	}
}
