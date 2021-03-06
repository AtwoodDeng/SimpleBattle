﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttack : Attack {
	[SerializeField] int distance = 3;

	public override SimBlock[] GetAttackRange (SimBlock temBlock, Direction direction, int attackRange)
	{
		List<SimBlock> res = new List<SimBlock>();
		int ci = temBlock.m_i;
		int cj = temBlock.m_j;
		for ( int i = - attackRange ; i <= attackRange ; ++ i ) {
			for ( int j = - attackRange ; j <= attackRange ; ++ j ) {
				if ( ( Mathf.Abs( i ) + Mathf.Abs( j ) ) < attackRange ) {
					res.Add( new SimBlock( ci + i + distance , cj + j ));
				}
			}
		}
		return BattleField.RotateBlocks( res.ToArray() , temBlock , direction );
	}
}
