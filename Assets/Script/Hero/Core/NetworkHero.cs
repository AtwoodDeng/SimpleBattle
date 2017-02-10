using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHero : InteractableHero {

	public enum HeroState
	{
		None,
		Strategy,
		StrategyMove,
		StrategyAttack,
		Battle,
		Dead,
	}
	protected AStateMachine<HeroState,LogicEvents> m_stateMachine;

	BoxCollider m_collider;

	protected override void MAwake ()
	{
		base.MAwake ();
	}

	void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<HeroState, LogicEvents>( HeroState.None );


		m_stateMachine.BlindStateEventHandler (HeroState.Strategy, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			Block block = (Block)arg.GetMessage(M_Event.BLOCK);
			Debug.Log("Get event" + arg.type);
			if ( arg.type == LogicEvents.ConfirmHero )
			{
				Debug.Log("Get Confirm Hero" + block.SimpleBlock + " " + TemBlock.SimpleBlock);
				if ( TemBlock !=null && TemBlock.Equals( block) ) {
					m_stateMachine.State = HeroState.StrategyMove;
				}else
				{
					m_stateMachine.State = HeroState.Strategy;
				}
			}else if ( arg.type == LogicEvents.SelectBlock )
			{
				m_stateMachine.State = HeroState.Strategy;
			}
		});

		m_stateMachine.BlindStateEventHandler (HeroState.StrategyMove, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			Block block = (Block)arg.GetMessage(M_Event.BLOCK);
			if ( arg.type == LogicEvents.ConfirmHero )
			{
				if ( TemBlock !=null && TemBlock.Equals( block) ) {
					m_stateMachine.State = HeroState.StrategyAttack;
				}else
				{
					m_stateMachine.State = HeroState.Strategy;
				}
			}else if ( arg.type == LogicEvents.SelectBlock )
			{
				m_stateMachine.State = HeroState.Strategy;
			}
		});

		m_stateMachine.BlindStateEventHandler (HeroState.StrategyAttack, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			Block block = (Block)arg.GetMessage(M_Event.BLOCK);
			if ( arg.type == LogicEvents.ConfirmHero )
			{
				if ( TemBlock !=null && TemBlock.Equals( block) ) {
					m_stateMachine.State = HeroState.StrategyMove;
				}else
				{
					m_stateMachine.State = HeroState.Strategy;
				}
			}else if ( arg.type == LogicEvents.SelectBlock )
			{
				m_stateMachine.State = HeroState.Strategy;
			}
		});

		m_stateMachine.AddEnter(HeroState.Strategy , delegate {
			if ( m_collider == null )
				m_collider = GetComponent<BoxCollider>();
			if ( m_collider != null )
				m_collider.size = new Vector3( 2.56f , 2.56f , 1f );
			if ( m_collider != null )
				m_collider.enabled = false;
		});

		m_stateMachine.AddEnter( HeroState.StrategyMove , delegate() {
			BattleField.ShowBlock( m_move.GetMoveRange() , BattleBlock.BlockVisualType.MoveRangeEnermy );	
		});

		m_stateMachine.AddEnter( HeroState.StrategyAttack , delegate() {
			BattleField.ShowBlock( m_attack.GetAttackRange() , BattleBlock.BlockVisualType.AttackRangeEnermy );	
		});

		m_stateMachine.BlindFromEveryState(LogicEvents.StrategyPhase , HeroState.Strategy );

		m_stateMachine.State = HeroState.Strategy;

	}

	public override void Init ()
	{
		base.Init ();
		LogicManager.Instance.RegisterHero( this );
		InitStateMachine ();


		SpriteRenderer head = GetComponent<SpriteRenderer>();
		Sprite res = Resources.Load<Sprite>("Img/Icon/" +  GetHeroInfo().type.ToString() + "Icon");
		head.sprite = res;

	}


	public override float BattleMove ()
	{
		return base.BattleMove ();
	}
	public void Move( HeroMoveInfo info )
	{
		if ( info.type == GetHeroInfo().type || info.ori.Equals( TemSimpleBlock ) ) {
			if ( m_strategy is CustomStrategy ) {
				((CustomStrategy)m_strategy).target = info.target ;
				((CustomStrategy)m_strategy).direction = info.toDirection;
				((CustomStrategy)m_strategy).isActive = info.isActive;
			}
		}
	}

//	public override void Select ()
//	{
//		base.Select ();
//		if (m_stateMachine.State == HeroState.StrategyNone
//			|| m_stateMachine.State == HeroState.StrategyAttack ) {
//			m_stateMachine.State = HeroState.StrategyMove;
//		} else if (m_stateMachine.State == HeroState.StrategyMove) {
//			m_stateMachine.State = HeroState.StrategyAttack;
//		}
//	}

	void OnEvent( LogicArg arg )
	{
		m_stateMachine.OnEvent (arg.type , arg );
	}	

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterAll (OnEvent);
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnRegisterAll (OnEvent);
	}

	void OnGUI()
	{
		GUILayout.Label ("");
		GUILayout.Label ("State " + m_stateMachine.State);
	}

}
