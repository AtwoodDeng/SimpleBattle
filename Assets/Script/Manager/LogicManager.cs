using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MBehavior {


	static public LogicManager Instance{ get { return m_Instance; } }
	static public LogicManager m_Instance;
	public LogicManager(){ m_Instance = this; }
	public bool isOnline;

	public static float moveDuration = 0.25f;
	public static float moveInterval = 0.1f;
	public static float attackDuration = 1f;
	public static float attackInterval = 0.1f;

	public enum Mode
	{
		AllBattle,
		SingleBattle,
	}
	public Mode mode;

	public enum State
	{
		None,
		PlaceHero,
		WaitPlaceHero,
		Strategy,
		WaitStrategy,
		Battle,
		Count,
	}
	public State State_{
		get { 
			return m_stateMachine.State;
		}
	}

	AStateMachine<State,LogicEvents> m_stateMachine;

	List<Hero> heroList = new List<Hero>();

	protected override void MAwake ()
	{
		base.MAwake ();
		InitStateMachine ();
	}

	void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<State, LogicEvents> (State.None);
		m_stateMachine.AddEnter (State.PlaceHero, delegate {
			M_Event.FireLogicEvent(LogicEvents.PlaceHeroPhase,new LogicArg(this));	
		});

		m_stateMachine.AddEnter( State.WaitPlaceHero , delegate {

			if ( isOnline )
			{
				List<RawHeroInfo> heros = new List<RawHeroInfo>();
				foreach( Hero h in heroList )
				{
					if ( h.GetHeroInfo().TeamColor == TeamColor.Blue )
						heros.Add( h.GetHeroInfo().GetRawHeroInfo());
				}

				NetworkManager.Instance.SendPlaceHero( heros.ToArray() );
			}else{
				m_stateMachine.State = State.Strategy;
			}
		});

		m_stateMachine.AddEnter (State.Strategy, delegate {
			M_Event.FireLogicEvent(LogicEvents.StrategyPhase , new LogicArg(this));
			OnBeforeBattle();
		});

		m_stateMachine.AddEnter( State.WaitStrategy , delegate {
			if ( isOnline )
			{	
				List<HeroMoveInfo> heros = new List<HeroMoveInfo>();
				foreach( Hero h in heroList )
				{
					if ( h.GetHeroInfo().TeamColor == TeamColor.Blue && !h.GetHeroInfo().IsDead)
						heros.Add( h.GetMoveInfo() );
				}
				Debug.Log("Send Move Hero");
				NetworkManager.Instance.SendMoveHero( heros.ToArray() );
			}else {
				m_stateMachine.State = State.Battle;
			}
		});

		m_stateMachine.AddEnter (State.Battle, OnBattle);

		m_stateMachine.AddEnter (State.Count, delegate {



			m_stateMachine.State = State.Strategy;	
		});

	}

	public void RegisterHero( Hero h )
	{
		if ( !heroList.Contains(h) )
			heroList.Add(h);
	}

	public void UnregisterHero( Hero h )
	{
		heroList.Remove(h);
	}

	protected override void MStart ()
	{
		base.MStart ();
//		heroList.AddRange (FindObjectsOfType<Hero> ());

		m_stateMachine.State = State.PlaceHero;
	}

	/// <summary>
	/// Call all hero .BeginBattle
	/// </summary>
	void OnBeforeBattle()
	{
		Hero[] list = GetSortedHeroList ();

		for (int i = 0; i < list.Length; ++i) {
			list [i].BeginBattle ();
		}
	}

	void OnBattle(){
		StartCoroutine (DoBattle());
	}

	IEnumerator DoBattle()
	{
		if ( mode == Mode.AllBattle )
		{
			Hero[] list = GetSortedHeroList ();
	//		Debug.Log ("List Length" + list.Length);

			// Begin
			for (int i = 0; i < list.Length; ++i) {
				list [i].BeginBattle ();
			}

			BattleField.ResetLock();
			// Strategy
			for (int i = 0; i < list.Length; ++i) {
				list [i].BattleTarget ();
			}

			// Move
			for (int i = 0; i < list.Length; ++i) {
				float duration = list [i].BattleMove ();
				yield return new WaitForSeconds (duration + moveInterval);
				list[i].EndMove();
			}

			yield return new WaitForSeconds( 0.5f );

			// Battle
			for (int i = 0; i < list.Length; ++i) {
				if (!list [i].GetHeroInfo ().IsDead) {
	//				Debug.Log ("Begin Attack " + list [i].name);
					float duration = list [i].BattleAttack ();
					yield return new WaitForSeconds (duration + attackInterval);
					list[i].EndAttack();
				}
			}

			// End
			for (int i = 0; i < list.Length; ++i) {
				if (!list [i].GetHeroInfo ().IsDead) {
					list[i].EndBattle();
				}
			}

			yield return new WaitForSeconds( 0.5f );

			m_stateMachine.State = State.Count;
		}else if ( mode == Mode.SingleBattle )
		{
			Hero[] list = GetSortedHeroList();

			// Begin
			for (int i = 0; i < list.Length; ++i) {
				list [i].BeginBattle ();
			}

			BattleField.ResetLock();
			// Strategy
			for (int i = 0; i < list.Length; ++i) {
				if ( list[i].GetHeroInfo().isActive )
					list [i].BattleTarget ();
			}

			// Move
			for (int i = 0; i < list.Length; ++i) {
				if ( list[i].GetHeroInfo().isActive ) {
					float duration = list [i].BattleMove ();
					yield return new WaitForSeconds (duration + moveInterval);
					list[i].EndMove();
				}
			}

			yield return new WaitForSeconds( 0.5f );

			// Battle
			for (int i = 0; i < list.Length; ++i) {
				if ( list[i].GetHeroInfo().isActive ) {
					if (!list [i].GetHeroInfo ().IsDead) {
						//				Debug.Log ("Begin Attack " + list [i].name);
						float duration = list [i].BattleAttack ();
						yield return new WaitForSeconds (duration + attackInterval);
						list[i].EndAttack();
					}
				}
			}

			// End
			for (int i = 0; i < list.Length; ++i) {
				if (!list [i].GetHeroInfo ().IsDead) {
					list[i].EndBattle();
				}
			}

			yield return new WaitForSeconds( 0.5f );

			m_stateMachine.State = State.Count;
		}
	}

	Hero[] GetSortedHeroList()
	{
		// TODO sort the hero according to the agile
		List<Hero> temList = new List<Hero>( heroList.ToArray() );
		for (int i = temList.Count -1; i >= 0 ; --i)
			if (temList [i].GetHeroInfo().IsDead)
				temList.RemoveAt (i);
		
		temList.Sort ((x, y) => {
			return - x.GetHeroInfo().Agile.CompareTo(y.GetHeroInfo().Agile);
		});

		return temList.ToArray ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			NextPhase ();
		}
	}

	public void NextPhase()
	{
		if (m_stateMachine.State == State.PlaceHero) {
			m_stateMachine.State = State.WaitPlaceHero;
		} else if (m_stateMachine.State == State.Strategy) {
			m_stateMachine.State = State.WaitStrategy;
		}
	}

	public void AddOnPhaseChange( AStateMachine<State,LogicEvents>.StateChangeHandler handler  )
	{
		m_stateMachine.AddOnChange (handler);
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.RegisterEvent(LogicEvents.NetPlaceHero , OnPlaceHeroNetwork );
		M_Event.RegisterEvent(LogicEvents.NetMoveHero , OnMoveHeroNetwork );
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent(LogicEvents.NetPlaceHero , OnPlaceHeroNetwork );
		M_Event.UnregisterEvent(LogicEvents.NetMoveHero , OnMoveHeroNetwork );
	}

	/// Network
	void OnPlaceHeroNetwork( LogicArg arg )
	{
		if ( m_stateMachine.State == State.WaitPlaceHero )
		{
			PlaceHeroMessage msg = (PlaceHeroMessage)arg.GetMessage("msg");
			foreach( RawHeroInfo hInfo in msg.heroInfo )
			{
				hInfo.block = BattleField.GetReflectBlock( hInfo.block );
				hInfo.direction = BattleField.GetReflectDirection( hInfo.direction );
				HeroFactory.SetUpEnemyHero( hInfo );
			}
			m_stateMachine.State = State.Strategy;
		}
	}


	void OnMoveHeroNetwork( LogicArg arg )
	{
		if ( LogicManager.Instance.State_ == LogicManager.State.WaitStrategy )
		{
			Debug.Log("Get Move MSG");
			MoveHeroMessage msg = (MoveHeroMessage)arg.GetMessage("msg");
			foreach( HeroMoveInfo mInfo in msg.heroMoves ) {
				foreach( Hero h in heroList ) {
					if ( h.GetHeroInfo().ID == mInfo.ID ) {
						if ( h is NetworkHero )
						{
							mInfo.ori = BattleField.GetReflectBlock( mInfo.ori );
							mInfo.target = BattleField.GetReflectBlock( mInfo.target );
							mInfo.toDirection = BattleField.GetReflectDirection( mInfo.toDirection );
							((NetworkHero)h).Move( mInfo );
						}
					}
				}
			}

			m_stateMachine.State = State.Battle;
		}
	}
}
