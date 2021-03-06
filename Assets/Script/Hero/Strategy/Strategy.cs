﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strategy : HeroComponent {
	public virtual bool IsReady()
	{
		return true;
	}

	public virtual void OnBeforeBattle()
	{
	}

	public virtual void OnBeginBattle()
	{
	}

	public virtual SimBlock GetTarget()
	{
		return parent.TemSimpleBlock;
	}

	public virtual Direction GetDirection()
	{
		return Direction.Left;
	}

	public virtual bool GetActive()
	{
		return true;
	}
}
