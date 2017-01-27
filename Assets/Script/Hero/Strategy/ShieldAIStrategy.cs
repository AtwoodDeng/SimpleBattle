using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAIStrategy : Strategy {

	public override SimBlock GetTarget ()
	{
		float[,] scores = new float[BattleField.Instance.gridWidth, BattleField.Instance.gridHeight];
		Block[] enermyBlocks = BattleField.GetEnermyBlock(parent.GetHeroInfo().TeamColor);

		for (int i = 0; i < scores.GetLength (0); ++i) {
			for (int j = 0; j < scores.GetLength (1); ++j) {
				Block b = BattleField.GetBlock (i, j);
				if (parent.IsInMoveRange (b) && b.state == Block.BlockState.Empty) {
					scores [i, j] = 0;
					foreach (Block e in enermyBlocks) {
						scores [i, j] += 1f / b.GetDistance( e ) / e.linkedBlock.GetHeroInfo ().Health;
					}
				} else
					scores [i, j] = -1f;
			}
		}


		float max = 0;
		Block target = parent.TemBlock;
		for (int i = 0; i < scores.GetLength (0); ++i) {
			for (int j = 0; j < scores.GetLength (1); ++j) {
				if (scores [i, j] > max) {
					max = scores [i, j];
					target = BattleField.GetBlock (i, j);
				}
			}
		}

		return target.SimpleBlock;

	}
}
