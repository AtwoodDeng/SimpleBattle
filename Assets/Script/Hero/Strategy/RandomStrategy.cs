using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStrategy : Strategy {

	public bool ifFollowNext = false;
	public SimBlock nextBlock;
	public Direction nextDirection;

	public override SimBlock GetTarget ()
	{
		if (ifFollowNext) {
			ifFollowNext = false;
			return nextBlock;
		}
		SimBlock[] moveRange = parent.Move.GetMoveRange();
		if (moveRange == null || moveRange.Length <= 0)
			return parent.TemSimpleBlock;

		return moveRange[Random.Range(0,moveRange.Length)];
	}

	public override Direction GetDirection ()
	{
		if (ifFollowNext) {
			ifFollowNext = false;
			return nextDirection;
		}
		return (Direction)Random.Range (0, 4);
	}
}
