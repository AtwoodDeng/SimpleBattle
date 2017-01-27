using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : HeroComponent {

	public virtual bool DoRecieveDamage( ref Damage dmg )
	{
		return false;
	}

	public void RecieveDamage( ref Damage dmg )
	{
		if ( DoRecieveDamage( ref dmg ) )
		{
			if ( parent.HeroAnim != null )
				parent.HeroAnim.RecieveDamage();
		}
	}

	public virtual bool DoChangeDamageOnAttack( ref Damage[] dmgs )
	{
		return false;
	}

	public void ChangeDamageOnAttack( ref Damage[] dmgs )
	{
		if ( DoChangeDamageOnAttack( ref dmgs ) )
		{
			if ( parent.HeroAnim != null )
				parent.HeroAnim.AttackPassive();
		}
	}

	public HistoryStep GetLastStep( HistoryStep.RecordType type )
	{
		HistoryStep lastHistory = null;

		int hi = parent.GetHeroInfo().history.Count - 1;

		while( hi >= 0 )
		{
			lastHistory = parent.GetHeroInfo().history[hi];
			if ( lastHistory.type == type )
				break;
			hi --;
		}

		return lastHistory;
	}

	public virtual bool DoBeginBattle()
	{
		return false;
	}

	public void BeginBattle()
	{
		if ( DoBeginBattle( ) )
		{
			if ( parent.HeroAnim != null )
				parent.HeroAnim.BeginBattlePassive();
		}
	}

	public virtual bool DoEndBattle()
	{
		return false;
	}

	public void EndBattle()
	{
		if ( DoEndBattle( ) )
		{
			if ( parent.HeroAnim != null )
				parent.HeroAnim.EndBattlePassive();
		}
	}

}
