using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossAttack : Attack {

	public override SimBlock[] GetAttackRange (SimBlock temBlock, Direction direction, int attackRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		int ci = temBlock.m_i;
		int cj = temBlock.m_j;
		for( int i = 1 ; i <= attackRange ; ++ i )
		{
			res.Add( new SimBlock( ci + i 	, cj 		) );
			res.Add( new SimBlock( ci 		, cj + i 	) );
			res.Add( new SimBlock( ci - i 	, cj 		) );
			res.Add( new SimBlock( ci 		, cj - i 	) );
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
