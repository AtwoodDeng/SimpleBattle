using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {
	public Hero parent;
	public int remainTurn;
	public BuffAddType addType;

	public virtual BuffType GetBuffType()
	{
		return BuffType.None;
	}

	public virtual void Update( BuffUpdateType type )
	{
		switch( type )
		{
		case BuffUpdateType.EndBattle:
			remainTurn--;
			break;
		default:
			break;
		}
	}

	public virtual void OnRecieveDamage( ref Damage dmg )
	{
		
	}

	public virtual void EndBattle()
	{
		remainTurn--;
	}

	public bool IsActive{ get { return remainTurn > 0 ; } }
	public virtual float GetAttack(){ return 0; }
	public virtual int GetAttackRange() { return 0 ; }
	public virtual int GetMoveRange() { return 0; }
	public virtual int GetAgile() { return 0; }

	public Buff()
	{
		remainTurn = 1;
		addType = BuffAddType.Mutiple;
	}
}

public enum BuffUpdateType
{
	EndBattle,

}

public enum BuffType
{
	None,
	SoldierBuff,
	FireBuff,
}

public enum BuffAddType
{
	// mutiple buff exist
	Mutiple,
	// replace the original buff
	Replace,
	// if the buff exist, then do nothing
	Single,
	// add the exist turn to the buff
	Add ,

}
