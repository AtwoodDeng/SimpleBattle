using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : HeroComponent {

	/// <summary>
	/// No Longer used
	/// </summary>
	/// <returns>The animation time to attack.</returns>
	public virtual float DoAttack()
	{
		return 1f;
	}

	public SimBlock[] GetAttackRange( )
	{
		return GetAttackRange(parent.TemSimpleBlock,
			parent.GetHeroInfo().direction,
			parent.GetHeroInfo().AttackRange);
	}

	/// <summary>
	/// Get all the blocks within the attack range
	/// </summary>
	/// <returns>The attack range.</returns>
	/// <param name="temBlock">Tem block.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="attackRange">Attack range.</param>
	public virtual SimBlock[] GetAttackRange( SimBlock temBlock , Direction direction , int attackRange)
	{
		return new SimBlock[0];
	}

	/// <summary>
	/// Get the targe Enermy block list 
	/// </summary>
	/// <returns>The target enermy block.</returns>
	public virtual SimBlock[] GetTargetBlock()
	{
		SimBlock[] enermyBlocks = BattleField.GetEnermyBlockSim(parent.GetHeroInfo().TeamColor);
		SimBlock[] attackRange = GetAttackRange();

		List<SimBlock> res = new List<SimBlock>();
		foreach( SimBlock b in enermyBlocks )
		{
			foreach( SimBlock sb in attackRange )
			{
				if ( (b.m_i == sb.m_i ) && ( b.m_j == sb.m_j ))
					res.Add( sb );
			}
		}
		return res.ToArray();
	}

	/// <summary>
	/// Get all damage made by this turn
	/// </summary>
	/// <returns>The all damage.</returns>
	public Damage[] GetAllDamage()
	{
		List<Damage> res = new List<Damage>();
		SimBlock[] targetBlock = GetTargetBlock();

		foreach( SimBlock sb in targetBlock )
		{
			res.Add( GetDamage( parent.GetHeroInfo() , sb ));
		}
		return res.ToArray();
	}

	public virtual Damage GetDamage( HeroInfo myInfo , SimBlock targetBlock )
	{
		return new Damage (myInfo.Attack, myInfo.attackType , myInfo , targetBlock);
	}

	/// <summary>
	/// Not used any more
	/// </summary>
	/// <param name="dmg">Dmg.</param>
	/// <param name="block">Block.</param>
	public static void SendDamage( Damage dmg, Block block )
	{
		if (block.linkedBlock != null && block.linkedBlock.GetHeroInfo () != null && block.state == Block.BlockState.Hero) {
			block.linkedBlock.GetHeroInfo ().RecieveDamage (dmg);
		}
	}


	public bool IsInAttackRange( SimBlock block )
	{
		foreach( SimBlock b in GetAttackRange())
		{
			if ( b.Equals( block ))
				return true;
		}
		return false;
	}
}
