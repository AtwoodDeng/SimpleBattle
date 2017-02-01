using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMove : Move {


	public override SimBlock[] GetMoveRange (SimBlock temBlock, Direction direction , int moveRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		res.Add( temBlock );
		int ci = temBlock.m_i;
		int cj = temBlock.m_j;
		int i = moveRange;
		res.Add( new SimBlock( ci + i 	, cj 		) );
		res.Add( new SimBlock( ci 		, cj + i 	) );
		res.Add( new SimBlock( ci - i 	, cj 		) );
		res.Add( new SimBlock( ci 		, cj - i 	) );

		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
