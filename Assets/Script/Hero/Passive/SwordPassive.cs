using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPassive : Passive {
	[SerializeField] float attackReduce = 10;

	public override bool DoChangeDamageOnAttack (ref Damage[] dmgs)
	{
		if ( dmgs == null || dmgs.Length <= 0 )
			return false;
		
		Damage targetD = dmgs[0];
		foreach( Damage d in dmgs )
		{
			if ( parent.TemSimpleBlock.GetDistance( d.target ) <= parent.TemSimpleBlock.GetDistance( targetD.target ) )
				targetD = d;
		}

		targetD.damage -= attackReduce * parent.TemSimpleBlock.GetDistance( targetD.target );

		dmgs = new Damage[]{ targetD };
		return true;
	}
}
