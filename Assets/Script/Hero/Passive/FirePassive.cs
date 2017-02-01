using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePassive : Passive {
	[SerializeField] float DamagePerLayer = 10;
	public override bool DoChangeDamageOnAttack (ref Damage[] dmgs)
	{
		foreach( Damage d in dmgs )
		{
			FireBuff fireBuff = new FireBuff( DamagePerLayer , parent );
			d.buffs = new Buff[]{ fireBuff };
		}

		return true;
	}
}

public class FireBuff: Buff
{
	public Hero sender;
	int count = 0;
	public float DamagePerLayer = 0;

	public FireBuff( float _DPL , Hero _S )
	{
		DamagePerLayer = _DPL;
		sender = _S;
		addType = BuffAddType.Add;
		remainTurn = 2;
	}

	public override BuffType GetBuffType ()
	{
		return BuffType.FireBuff;
	}

	public override void OnRecieveDamage (ref Damage dmg)
	{
		if ( !dmg.IsHeal && ( dmg.type == DamageType.Physics || dmg.type == DamageType.Magic ) )
		{
			count ++ ;
			Damage fireDMG = new Damage( count * DamagePerLayer , DamageType.FireBuff , sender.GetHeroInfo() , parent.TemSimpleBlock );
			parent.RecieveDamage( fireDMG );
		}
	}
}