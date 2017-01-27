using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrossMove : Move {

//	public override float MoveTo (SimBlock target)
//	{
//		Block targetBlock = BattleField.GetBlock( target);
//		float duration = LogicManager.moveDuration;
//		parent.transform.DOMove (targetBlock.linkedBlock.GetCenterPosition (), duration);
//		return duration;
//	}

	public override SimBlock[] GetMoveRange (SimBlock temBlock, Direction direction , int moveRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		res.Add( temBlock );
		int ci = temBlock.m_i;
		int cj = temBlock.m_j;
		for( int i = 1 ; i <= moveRange; ++ i )
		{
			res.Add( new SimBlock( ci + i 	, cj 		) );
			res.Add( new SimBlock( ci 		, cj + i 	) );
			res.Add( new SimBlock( ci - i 	, cj 		) );
			res.Add( new SimBlock( ci 		, cj - i 	) );
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
