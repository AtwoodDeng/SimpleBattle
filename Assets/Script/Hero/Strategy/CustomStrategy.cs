﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomStrategy : Strategy {

	public SimBlock target;
	public float angle{
		get {
			return m_angle;
		}
		set {
			m_angle = value;
			direction = Angle2Direction( m_angle );
		}
	}
	float m_angle;

	public Direction direction;
	public override SimBlock GetTarget ()
	{
		if (BattleField.GetBlock( target ).state == Block.BlockState.Empty) {
			return target;
		}

		// TODO : remove the else situation
		return parent.TemBlock.SimpleBlock;
	}

	public override Direction GetDirection ()
	{
		return direction;
	}

	public Direction Angle2Direction( float angle )
	{
		return HeroInfo.Angle2Direction (angle);
	}

}
