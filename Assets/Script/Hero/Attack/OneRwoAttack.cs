using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneRwoAttack : Attack {


	public override SimBlock[] GetAttackRange (SimBlock temBlock, Direction direction, int attackRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		for( int i = 1 ; i <= attackRange ; ++ i )
		{
			res.Add( new SimBlock( temBlock.m_i + i , temBlock.m_j ));
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
