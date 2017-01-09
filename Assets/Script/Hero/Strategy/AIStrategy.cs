using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStrategy : Strategy {

	public override Block GetTarget ()
	{
		Block to = null;
		while (to == null) {
			to = BattleField.GetRandomBlock ();
			if (to.GetDistance (parent.TemBlock) == 0) {
				break;
			}
			if (to.state != Block.BlockState.Empty || !parent.IsInMoveRange( to ) )
				to = null;
		}
		return to;
	}

	public override Direction GetDirection ()
	{
		return (Direction)Random.Range (0, 4);
	}

}
