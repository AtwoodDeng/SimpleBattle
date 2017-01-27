using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulPassive : Passive {
	[SerializeField] int AttackNumber = 3;

	public override bool DoChangeDamageOnAttack (ref Damage[] dmgs)
	{
		if ( dmgs == null || dmgs.Length <= 0 )
			return false;
		
		List<SimBlock> TargetBlock = new List<SimBlock>();
		foreach( Damage d in dmgs )
		{
			TargetBlock.Add( d.target );
		}

		dmgs = new Damage[AttackNumber];

		for ( int i = 0 ; i < AttackNumber ; ++ i )
		{
			dmgs[i] = parent.Attack.GetDamage( parent.GetHeroInfo() , 
				TargetBlock[ Random.Range( 0 , TargetBlock.Count ) ] );
		}
		return true;
	}
}
