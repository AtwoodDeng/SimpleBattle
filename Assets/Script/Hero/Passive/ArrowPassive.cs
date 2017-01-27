using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPassive : Passive {

	[SerializeField] float attackIncrease = 10f;

	public override bool DoChangeDamageOnAttack (ref Damage[] dmgs)
	{
		SimBlock[] enermys = BattleField.GetEnermyBlockSim( parent.GetHeroInfo().TeamColor );
		SimBlock temBlock = parent.TemBlock.SimpleBlock;

		bool isBlocked = false;
		foreach( SimBlock b in enermys ) {
			if ( b.GetDistance( temBlock ) <= 1 )
				isBlocked = true;
		}

		if ( isBlocked )
		{
			for ( int i = 0 ; i < dmgs.Length ; ++ i ) {
				dmgs[i].damage = 0;
			}
		}else{
			for ( int i = 0 ; i < dmgs.Length ; ++ i ) {
				dmgs[i].damage += temBlock.GetDistance( dmgs[i].target ) * attackIncrease;
			}
		}

		return !isBlocked;
	}

}
