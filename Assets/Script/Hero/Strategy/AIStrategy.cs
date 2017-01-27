using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStrategy : Strategy {

	public override SimBlock GetTarget ()
	{
		Block to = null;
		SimBlock[] moveRange = parent.Move.GetMoveRange();

		return moveRange[Random.Range(0,moveRange.Length)]; 
	}

	public override Direction GetDirection ()
	{
		return (Direction)Random.Range (0, 4);
	}

}
