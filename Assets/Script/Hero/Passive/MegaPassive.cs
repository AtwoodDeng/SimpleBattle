using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaPassive : Passive {
	[SerializeField] float attackMutiple = 2f;

	public override bool DoChangeDamageOnAttack (ref Damage[] dmgs)
	{
		HistoryStep lastHistory = GetLastStep(HistoryStep.RecordType.StartBattle);

		if ( lastHistory.block.Equals(parent.TemSimpleBlock) )
		{
			for( int i = 0 ; i < dmgs.Length ; ++ i )
			{
				dmgs[i].damage *= attackMutiple;

			}
			return true;
		}
		return false;
	}
}
