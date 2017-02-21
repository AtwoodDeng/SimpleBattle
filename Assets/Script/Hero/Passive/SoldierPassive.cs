using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierPassive : Passive {

	[SerializeField] float attackIncrease = 15f;

	public override bool DoRecieveDamage (ref Damage dmgs)
	{
		if ( !dmgs.IsHeal ) {
			SoliderAttackBuff buff = null;

			if ( parent.GetHeroInfo().GetBuff<SoliderAttackBuff>() != null )
			{
				buff = (SoliderAttackBuff)parent.GetHeroInfo().GetBuff<SoliderAttackBuff>();
			}

			if ( buff == null ) {
				buff = new SoliderAttackBuff();
				Debug.Log("Add Solider buff");
				parent.GetHeroInfo().AddBuff( buff );
			}
			
			buff.AddUpBuff( attackIncrease );
			return true;
		}
		return false;
	}

//	public override bool DoBeginBattle ()
//	{
//		Debug.Log("Do Begin Battle" + parent.name );
//		parent.GetHeroInfo().HealthChangeFunc += OnSoliderHealthChange;
//		return true;
//	}
//
//	void OnSoliderHealthChange( float _f , float _t )
//	{
//		Debug.Log("On Solider health change");
//		if ( _t < _f ) {
//			SoliderAttackBuff buff = null;
//
//			if ( parent.GetHeroInfo().GetBuff<SoliderAttackBuff>() != null )
//			{
//				buff = (SoliderAttackBuff)parent.GetHeroInfo().GetBuff<SoliderAttackBuff>();
//			}
//
//			if ( buff == null ) {
//				buff = new SoliderAttackBuff();
//				Debug.Log("Add Solider buff");
//				parent.GetHeroInfo().AddBuff( buff );
//			}
//			
//			buff.AddUpBuff();
//			parent.HeroAnim.Passive();
//		}
//	}
//
//	public override bool DoEndBattle ()
//	{
//		Debug.Log("End Battle " + parent.name );
//		parent.GetHeroInfo().HealthChangeFunc -= OnSoliderHealthChange;
//		return true;
//	}
}


public class SoliderAttackBuff : Buff
{
	float AttackIncrease = 0;

	public override void DeepCopy (Buff buff, Dictionary<Hero, Hero> heroMap)
	{
		base.DeepCopy (buff, heroMap);
		if (buff is SoliderAttackBuff) {
			AttackIncrease = ((SoliderAttackBuff)buff).AttackIncrease;
		}
	}

	public override BuffType GetBuffType ()
	{
		return BuffType.SoldierBuff;
	}

	public override float GetAttack ()
	{
		Debug.Log("Get Attack from buff " + AttackIncrease);
		return AttackIncrease;
	}

	public void AddUpBuff( float increase )
	{
		remainTurn = 999;
		AttackIncrease += increase;
	}

	public SoliderAttackBuff(){
		remainTurn = 999;
		AttackIncrease = 0;
	}
}