using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : HeroComponent {

	/// <summary>
	/// Old Interface for moving to target block
	/// </summary>
	/// <returns>The to.</returns>
	/// <param name="target">Target.</param>
//	public virtual float MoveTo( SimBlock target)
//	{
//		if ( parent.isVirtual )
//			return 0;
//		float duration = LogicManager.moveDuration;
//		Block targetBlock = BattleField.GetBlock( target);
//		parent.transform.DOMove (targetBlock.linkedBlock.GetCenterPosition (), duration);
//		return duration;
//	}

	public SimBlock[] GetMoveRange( )
	{
		return GetMoveRange(parent.TemBlock.SimpleBlock,
			parent.GetHeroInfo().direction,
			parent.GetHeroInfo().MoveRange);
	}

	public virtual SimBlock[] GetMoveRange( SimBlock temBlock , Direction direction, int moveRange)
	{
		return new SimBlock[0];
	}
}
