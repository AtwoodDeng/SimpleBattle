using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerAttack : Attack {

	public override SimBlock[] GetAttackRange (SimBlock temBlock, Direction direction, int attackRange)
	{
		List<SimBlock> blocks = new List<SimBlock>();
		int ci = temBlock.m_i;
		int cj = temBlock.m_j;
		for( int i = - attackRange ; i <= attackRange ; ++ i ) {
			for ( int j = - attackRange ; j <= attackRange ; ++ j ) {
				blocks.Add( new SimBlock( ci + i  , cj + j ) );
			}
		}
		for( int i = 1 ; i <= attackRange ; ++ i ) {
			blocks.Add( new SimBlock( ci + attackRange + i , cj + attackRange + i ));
			blocks.Add( new SimBlock( ci + attackRange + i , cj - attackRange - i ));
			blocks.Add( new SimBlock( ci - attackRange - i , cj + attackRange + i ));
			blocks.Add( new SimBlock( ci - attackRange - i , cj - attackRange - i ));
		}

		return BattleField.RotateBlocks( blocks.ToArray() , temBlock , direction );
	}
}
