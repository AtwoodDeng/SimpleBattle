using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltedCrossMove : Move {

	public override SimBlock[] GetMoveRange (SimBlock temBlock, Direction direction, int moveRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		res.Add( temBlock );
		for( int i = 1 ; i <= moveRange ; ++ i )
		{
			res.Add ( new SimBlock( temBlock.m_i + i , temBlock.m_j + i ));
			res.Add ( new SimBlock( temBlock.m_i - i , temBlock.m_j + i ));
			res.Add ( new SimBlock( temBlock.m_i - i , temBlock.m_j - i ));
			res.Add ( new SimBlock( temBlock.m_i + i , temBlock.m_j - i ));
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
