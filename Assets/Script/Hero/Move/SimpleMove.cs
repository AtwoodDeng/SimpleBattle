using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleMove : Move {

	public override float MoveTo (Block target)
	{
		float duration = LogicManager.moveDuration;
		parent.transform.DOMove (target.linkedBlock.GetCenterPosition (), duration);
		return duration;
	}

//	public override Block SetTarget (Block block)
//	{
//		if (block.state == Block.BlockState.Empty) {
//			target = block;
//		} else {
//			target = parent.TemBlock;
//		}
//		target.state = Block.BlockState.Locked;
//
//		return target;
//	}
}
