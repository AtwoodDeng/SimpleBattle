using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : Attack {

	public override SimBlock[] GetAttackRange (SimBlock temBlock, Direction direction, int attackRange)
	{

		List<SimBlock> res = new List<SimBlock>();
		int ci = temBlock.m_i;
		int cj = temBlock.m_j;
		for ( int i = 1 ; i <= attackRange ; ++ i )
		{
			for( int j = - attackRange + 1 ; j < attackRange ; ++ j )
			{
				if ( Mathf.Abs( j ) < i )
					res.Add( new SimBlock( ci + i , cj + j ) );
			}
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
