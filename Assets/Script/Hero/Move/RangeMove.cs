using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMove : Move {


	public override SimBlock[] GetMoveRange (SimBlock temBlock, Direction direction, int moveRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		for( int i = - moveRange ; i <= moveRange ; ++ i )
		{
			for ( int j = - moveRange ; j <= moveRange ; ++ j )
			{
				SimBlock block = new SimBlock( temBlock.m_i + i , temBlock.m_j + j );
				if ( block.GetDistance( temBlock ) <= moveRange )
				{
					res.Add( block );
				}
			}
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
