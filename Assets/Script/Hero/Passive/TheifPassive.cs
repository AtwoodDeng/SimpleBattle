using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheifPassive : Passive {

	[SerializeField] float CriticalAttackRate = 3f;

	public override bool DoChangeDamageOnAttack (ref Damage[] dmgs)
	{
		if ( dmgs != null && dmgs.Length == 1 )
		{
			dmgs[0].damage *= CriticalAttackRate;
			return true;
		}
		return false;
	}
}
