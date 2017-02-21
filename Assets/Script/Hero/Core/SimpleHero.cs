using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class SimpleHero : InteractableHero {

	public enum HeroState
	{
		None,
		ReadyToPlace,
		MoveWithMouse,
		Prepare,
		Strategy,
		StrategyChoose,
		StrategyDirection,
		StrategyConfirm,
		BattleMove,
		BattleAttack,
		Dead,
	}
		
	[SerializeField] SpriteRenderer targetLine;
	[SerializeField] SpriteRenderer targetArrow;

	protected BoxCollider m_collider;
	protected AStateMachine<HeroState,LogicEvents> m_stateMachine;
	Vector3 oritinalPos;

	protected override void MAwake ()
	{
		base.MAwake ();
		InitStateMachine ();

		// TODO: FOR TEST
		oritinalPos = transform.position;
		m_collider = GetComponent<BoxCollider> ();
	}

	public override void Init ()
	{
		base.Init ();

		SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
		foreach( SpriteRenderer sprite in sprites ) {
			if ( sprite.name.Equals("targetLine" ) )
				targetLine = sprite;
			if ( sprite.name.Equals("targetArrow" ))
				targetArrow = sprite;
		}

		SpriteRenderer head = GetComponent<SpriteRenderer>();
		Sprite res = Resources.Load<Sprite>("Img/Icon/" +  GetHeroInfo().type.ToString() + "Icon");
		head.sprite = res;

		InitStateMachine();

		oritinalPos = transform.position;
		m_collider = GetComponent<BoxCollider> ();
	}
	public override void SetBlock (SimBlock block)
	{
		base.SetBlock (block);

		if ( TemBlock != null )
			transform.position = TemBlock.GetCenterPosition();
	}


	bool IsStateMachineInited = false;
	void InitStateMachine()
	{
		if ( IsStateMachineInited ) return;
		IsStateMachineInited = true;

		m_stateMachine = new AStateMachine<HeroState, LogicEvents> (HeroState.None);

		m_stateMachine.BlindFromEveryState (LogicEvents.PlaceHeroPhase, HeroState.ReadyToPlace);

		m_stateMachine.AddEnter (HeroState.ReadyToPlace, delegate {
			transform.DOMove( oritinalPos , 1f );
		});

		m_stateMachine.AddEnter(HeroState.MoveWithMouse , delegate {
			m_collider.size = new Vector3( 0.1f , 0.1f , 1f );
		});

		m_stateMachine.AddUpdate (HeroState.MoveWithMouse, delegate {
			transform.position = InputManager.FocusWorldPos;
		});

		m_stateMachine.AddExit (HeroState.MoveWithMouse, delegate {
			m_collider.size = new Vector3(2.56f,2.56f,1f);
		});

		m_stateMachine.AddEnter (HeroState.Prepare, delegate {
			if ( TriggerBlock != null ) {
				TriggerBlock.RegisterHero (this);

				TemBlock = TriggerBlock.BlockInfo;
				LogicManager.Instance.RegisterHero( this );

				transform.DOMove( TriggerBlock.GetCenterPosition() , 0.2f );

				// Disable Collider
				if ( m_collider != null )
					m_collider.enabled = false;
			}

		});

		m_stateMachine.BlindFromEveryState (LogicEvents.StrategyPhase, HeroState.Strategy);

		m_stateMachine.BlindStateEventHandler (HeroState.Strategy, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			if ( arg.type == LogicEvents.ConfirmHero )
			{
				Block block = (Block)arg.GetMessage(M_Event.BLOCK);
				if ( TemBlock != null &&  TemBlock.Equals(block))
				{
					m_stateMachine.State = HeroState.StrategyChoose;
				}
			}
		});

		m_stateMachine.BlindStateEventHandler (HeroState.StrategyChoose, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			if ( arg.type == LogicEvents.ConfirmHero )
			{
//				Block block = (Block)arg.GetMessage(M_Event.BLOCK);
//				if ( TemBlock != null && !TemBlock.Equals(block))
//				{
					m_stateMachine.State = HeroState.Strategy;
//				}
			}else if ( arg.type == LogicEvents.SelectBlock )
			{
				Block block = (Block)arg.GetMessage(M_Event.BLOCK);

				if ( m_move.IsInMoveRange( block.SimpleBlock ) )
				{
					((CustomStrategy)m_strategy).target = block.SimpleBlock;
					DrawToTarget( block );
					m_stateMachine.State = HeroState.StrategyDirection;
				}
			}
		});


		m_stateMachine.BlindStateEventHandler (HeroState.StrategyDirection, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			if ( arg.type == LogicEvents.FingerUp )
			{
				m_stateMachine.State = HeroState.StrategyConfirm;
			}
		});

		m_stateMachine.BlindStateEventHandler (HeroState.StrategyConfirm, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			if ( arg.type == LogicEvents.ConfirmHero )
			{
				Block block = (Block)arg.GetMessage(M_Event.BLOCK);
				if ( TemBlock.Equals(block))
				{
					m_stateMachine.State = HeroState.StrategyChoose;
				}
			}else if ( arg.type == LogicEvents.ConfirmMove ) {
				if ( LogicManager.Instance.mode == LogicManager.Mode.SingleBattle ) {
					if ( arg.sender != this )
						m_stateMachine.State = HeroState.Strategy;
				}
			}
		});

		m_stateMachine.AddEnter (HeroState.Strategy, delegate {
			if ( TemBlock == null )
				m_stateMachine.State = HeroState.None;
			if ( m_strategy is CustomStrategy )
				((CustomStrategy)m_strategy).target = TemSimpleBlock;

			targetLine.enabled = false;
			targetArrow.enabled = false;

			if ( m_strategy is CustomStrategy )
				((CustomStrategy)m_strategy).isActive = false;	

		});

		m_stateMachine.AddEnter (HeroState.StrategyChoose, delegate {
			TemBlock.linkedBlock.visualType = BattleBlock.BlockVisualType.StrategyFocus;
			BattleField.ShowBlock( m_move.GetMoveRange() , BattleBlock.BlockVisualType.StrategyMoveRange );
			BattleField.ShowBlock( new SimBlock[]{ TemBlock.SimpleBlock} , BattleBlock.BlockVisualType.StrategyChosenHero , false );
		});

		m_stateMachine.AddEnter (HeroState.StrategyDirection, delegate() {
			targetArrow.enabled = true;
			Block block = BattleField.GetBlock( ((CustomStrategy)m_strategy).target );
			BattleField.ShowBlock( new SimBlock[]{ TemBlock.SimpleBlock} , BattleBlock.BlockVisualType.StrategyChosenHero , false );
			targetArrow.transform.position = block.linkedBlock.GetCenterPosition();
		});

		m_stateMachine.AddUpdate (HeroState.StrategyDirection, delegate {
			// set the target arrow to right angle
			Vector3 focusPos = InputManager.FocusWorldPos;
			Block block = BattleField.GetBlock( ((CustomStrategy)m_strategy).target );
			Vector3 toward = focusPos - block.linkedBlock.GetCenterPosition();
			float angle = Mathf.Atan2( toward.y , toward.x ) * Mathf.Rad2Deg;
			angle = Mathf.Round( ( angle ) / 90f) * 90f; 

			targetArrow.transform.rotation = Quaternion.Euler( 0 , 0 , angle );

			// set the angle of strategy
			((CustomStrategy)m_strategy).angle = angle;

			// get the direction of the strategy
			Direction direction = ((CustomStrategy)m_strategy).GetDirection();
			BattleField.ShowBlock( m_attack.GetAttackRange( ((CustomStrategy)m_strategy).target , direction , GetHeroInfo().AttackRange ) , BattleBlock.BlockVisualType.StrategyAttackRange);

		});

		m_stateMachine.AddExit (HeroState.StrategyDirection, delegate {
			((CustomStrategy)m_strategy).isActive = true;	
			LogicArg arg = new LogicArg(this);
			Debug.Log("Fire Confirm Move ");
			M_Event.FireLogicEvent(LogicEvents.ConfirmMove , arg );
		});

		m_stateMachine.AddEnter (HeroState.StrategyConfirm, delegate {
			BattleField.ResetVisualColor( true );
			TemBlock.linkedBlock.visualType = BattleBlock.BlockVisualType.StrategyConfirm;
		});

		m_stateMachine.AddEnter (HeroState.BattleMove, delegate {
			targetLine.enabled = false;
			targetArrow.enabled = false;
		});

	}

	void DrawToTarget( Block block )
	{
		Vector3 fromPos = TemBlock.linkedBlock.GetCenterPosition ();
		Vector3 toPos = block.linkedBlock.GetCenterPosition ();
		Vector3 pos = ( fromPos + toPos ) * 0.5f;

		Vector3 delta = toPos - fromPos;

		float angle = Mathf.Atan2 (delta.y, delta.x) * Mathf.Rad2Deg;
		targetLine.transform.localScale = new Vector3 (delta.magnitude * 1.5f , 0.1f, 0);
		targetLine.transform.position = pos;
		targetLine.transform.rotation = Quaternion.Euler (0, 0, angle);

		targetLine.enabled = true;
	}

	void OnEvent( LogicArg arg )
	{
		m_stateMachine.OnEvent (arg.type , arg );
	}

	public void SelectAndDraw()
	{
		m_stateMachine.State = HeroState.MoveWithMouse;
	}

	public override void Select ()
	{
		base.Select ();
		if (m_stateMachine.State == HeroState.ReadyToPlace) {
			m_stateMachine.State = HeroState.MoveWithMouse;
		} else if (m_stateMachine.State == HeroState.Strategy) {
			m_stateMachine.State = HeroState.StrategyChoose;
		}
	}

	public override void Confirm ()
	{
		base.Confirm ();
		if (m_stateMachine.State == HeroState.MoveWithMouse) {
			if (TriggerBlock != null && TriggerBlock.BlockInfo.state == Block.BlockState.Empty && TriggerBlock.IsPlacable) {
				m_stateMachine.State = HeroState.Prepare;
			} else {
				m_stateMachine.State = HeroState.ReadyToPlace;
			}
		}
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

	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();
	}

	public override float BattleMove ()
	{
		m_stateMachine.State = HeroState.BattleMove;
		return base.BattleMove ();
	}

	public override float BattleAttack ()
	{
		m_stateMachine.State = HeroState.BattleAttack;
		return base.BattleAttack ();
	}

}
