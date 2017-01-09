using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class SimpleHero : InteractableHero {

	int m_healthShow;
	int healthShow{
		get {
			return m_healthShow;
		}
		set {
			m_healthShow = value;
			healthText.text = m_healthShow.ToString ();
		}
	}
	[SerializeField]TextMesh healthText;
	[SerializeField]TextMesh dmgText;


	[SerializeField] SpriteRenderer targetLine;
	[SerializeField] SpriteRenderer targetArrow;

	protected BoxCollider m_collider;
	protected AStateMachine<HeroState,LogicEvents> m_stateMachine;
	Vector3 oritinalPos;

	void Awake()
	{
		InitStateMachine ();

		InitHeroInfo ();

		// TODO: FOR TEST
		oritinalPos = transform.position;
		m_collider = GetComponent<BoxCollider> ();
	}
		

	void InitHeroInfo()
	{
		healthShow = (int)GetHeroInfo ().health;
		GetHeroInfo().HealthChangeFunc += delegate(float fromHealth ,float toHealth) {
			DOTween.To( () => healthShow , (x) => healthShow  = x , Mathf.Max( 0 , (int)toHealth) , 1f );
			transform.DOShakePosition( 0.5f , 0.2f );

			// show damage
			float dmg = fromHealth - toHealth;
			dmgText.text = dmg.ToString();
			dmgText.transform.position = healthText.transform.position;
			dmgText.transform.DOMoveY( 0.5f , 2f ).SetRelative( true );
			dmgText.color = Color.red;
			DOTween.To( () => dmgText.color , (x) => dmgText.color = x , new Color( 1f , 0.5f , 0 , 0 ) , 1f ).SetDelay(2f);
		};

		GetHeroInfo().DeathFunc += delegate() {
			healthText.gameObject.SetActive(false);
			dmgText.gameObject.SetActive(false);
			TemBlock = null;
			GetComponent<SpriteRenderer>().DOColor( Color.red , 1f ).OnComplete( delegate() {
				gameObject.SetActive(false);	
			});
		};
	}

	void InitStateMachine()
	{
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

			if ( TriggerBlock != null )
			{
				TemBlock = TriggerBlock.BlockInfo;
				transform.DOMove( TriggerBlock.GetCenterPosition() , 0.2f );

				// Disable Collider
				Collider collider = GetComponent<Collider>();
				if ( collider != null )
					collider.enabled = false;
			}

		});

		m_stateMachine.BlindFromEveryState (LogicEvents.StrategyPhase, HeroState.Strategy);

		m_stateMachine.BlindStateEventHandler (HeroState.Strategy, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			if ( arg.type == LogicEvents.ConfirmHero )
			{
				Block block = (Block)arg.GetMessage(M_Event.BLOCK);
				if ( TemBlock.Equals(block))
				{
					m_stateMachine.State = HeroState.StrategyChoose;
				}
			}
		});

		m_stateMachine.BlindStateEventHandler (HeroState.StrategyChoose, delegate(object obj) {
			LogicArg arg = (LogicArg) obj;
			if ( arg.type == LogicEvents.ConfirmHero )
			{
				Block block = (Block)arg.GetMessage(M_Event.BLOCK);
				if ( !TemBlock.Equals(block))
				{
					m_stateMachine.State = HeroState.Strategy;
				}
			}else if ( arg.type == LogicEvents.SelectBlock )
			{
				Block block = (Block)arg.GetMessage(M_Event.BLOCK);

				if ( TemBlock.GetDistance( block ) <= GetHeroInfo().moveRange )
				{
					((CustomStrategy)strategy).target = block;
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
			}
		});

		m_stateMachine.AddEnter (HeroState.Strategy, delegate {
//			targetBlock = TemBlock;

			((CustomStrategy)strategy).target = TemBlock;
			TemBlock.linkedBlock.BackgroundSetColor(Color.gray);	 
		});

		m_stateMachine.AddEnter (HeroState.StrategyChoose, delegate {
			TemBlock.linkedBlock.BackgroundSetColor(Color.yellow);	
			BattleField.ShowWalkable( TemBlock , GetHeroInfo().moveRange );
		});

		m_stateMachine.AddEnter (HeroState.StrategyDirection, delegate() {
			targetArrow.enabled = true;
			targetArrow.transform.position = ((CustomStrategy)strategy).target.linkedBlock.GetCenterPosition();
		});

		m_stateMachine.AddUpdate (HeroState.StrategyDirection, delegate {
			Vector3 focusPos = InputManager.FocusWorldPos;
			Vector3 toward = focusPos - ((CustomStrategy)strategy).target.linkedBlock.GetCenterPosition();
			float angle = Mathf.Atan2( toward.y , toward.x ) * Mathf.Rad2Deg;
			angle = Mathf.Round( ( angle ) / 90f) * 90f; 

			targetArrow.transform.rotation = Quaternion.Euler( 0 , 0 , angle );

			((CustomStrategy)strategy).angle = angle;
		});

		m_stateMachine.AddEnter (HeroState.StrategyConfirm, delegate {
			BattleField.ResetEmpty( TemBlock );
			TemBlock.linkedBlock.BackgroundSetColor(new Color(0.6f , 0.6f , 1f ) );	
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
				TriggerBlock.RegisterHero (this);
			} else {
				m_stateMachine.State = HeroState.ReadyToPlace;
			}
		}
	}

	void OnEnable()
	{
		M_Event.RegisterAll (OnEvent);
	}

	void OnDisable()
	{
		M_Event.UnRegisterAll (OnEvent);
	}

	void Update()
	{
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


	void OnGUI()
	{
		GUILayout.Label ("");
		GUILayout.Label ("State " + m_stateMachine.State);
	}
}
