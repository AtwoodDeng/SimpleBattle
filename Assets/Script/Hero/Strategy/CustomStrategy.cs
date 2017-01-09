using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomStrategy : Strategy {

	public Block target;
	public float angle;
	public override Block GetTarget ()
	{

		if (target.state == Block.BlockState.Empty) {
			return target;
		}

		return parent.TemBlock;
	}

	public override Direction GetDirection ()
	{
		return HeroInfo.Angle2Direction (angle);
	}
}
