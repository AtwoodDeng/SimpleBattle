using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHero : Hero {
	protected override void MAwake ()
	{
		base.MAwake ();
		InitStateMachine ();
	}

	void InitStateMachine()
	{
		
	}

	public override void Init ()
	{
		base.Init ();
		LogicManager.Instance.RegisterHero( this );
	}


	public void Move( HeroMoveInfo info )
	{
		if ( info.type == GetHeroInfo().type || info.ori.Equals( TemBlock.SimpleBlock ) ) {
			if ( m_strategy is CustomStrategy ) {
				((CustomStrategy)m_strategy).target = info.target ;
				((CustomStrategy)m_strategy).direction = info.toDirection;
			}
		}
	}

}
