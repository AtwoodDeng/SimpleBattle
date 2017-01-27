using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {

	public int remainTurn;

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
	}
}

public enum BuffUpdateType
{
	EndBattle,
}
