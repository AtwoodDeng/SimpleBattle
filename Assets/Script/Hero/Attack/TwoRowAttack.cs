using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoRowAttack : Attack {


	public override SimBlock[] GetAttackRange (SimBlock temBlock, Direction direction, int attackRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		for( int i = 1 ; i <= attackRange ; ++ i )
		{
			for ( int j = -1 ; j <= 1 ; ++ j )
			{
				if ( j != 0 )
					res.Add( new SimBlock( temBlock.m_i + i , temBlock.m_j + j ));
			}
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
