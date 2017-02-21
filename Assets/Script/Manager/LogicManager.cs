using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MBehavior {


	static public LogicManager Instance {
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<LogicManager> ();
			return m_Instance; }
		set {
			if (m_Instance == null)
				m_Instance = value;
		}
	}
	static public LogicManager m_Instance;

	public bool isOnline;

	public static float moveDuration = 0.8f;
	public static float moveInterval = 0.1f;
	public static float attackDuration = 1f;
	public static float attackInterval = 0.1f;
	// interval between different phase and two hero's move
	public static float basicInterval = 0.5f;

	public int Round
	{
		get {
			return m_round;
		}
	}
	private int m_round;

	public enum Mode
	{
		AllBattle,
		SingleBattle,
		InOrderBattle,
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
			InitGame();
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
			OnBeforeBattle(heroList.ToArray());
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

	public void InitGame()
	{
		m_round = 0;
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
	/// and increae the number of round
	/// </summary>
	void OnBeforeBattle( Hero[] heros )
	{
		m_round++;

		Hero[] list = GetSortedHeroList ( heros );

		for (int i = 0; i < list.Length; ++i) {
			list [i].BeginBattle ();
		}
	}

	void OnBattle(){
		StartCoroutine (DoBattle());
	}

	IEnumerator DoBattle()
	{
		///////////////////////////
		/// 
		/// All Heros move first and attack in order
		/// 
		///////////////////////////
		if ( mode == Mode.AllBattle )
		{
			Hero[] list = GetSortedHeroList ( heroList.ToArray() );
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

			yield return new WaitForSeconds( basicInterval );

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

			yield return new WaitForSeconds( basicInterval );

			m_stateMachine.State = State.Count;

			///////////////////////////
			/// 
			/// All Heros move and attack in order
			/// 
			///////////////////////////
		}else if ( mode == Mode.InOrderBattle )	
		{
			Hero[] list = GetSortedHeroList ( heroList.ToArray() );
			//		Debug.Log ("List Length" + list.Length);

			// Begin
			for (int i = 0; i < list.Length; ++i) {
				list [i].BeginBattle ();
			}

			BattleField.ResetLock();
			// Strategy
			for (int i = 0; i < list.Length; ++i) {
			}

			// Move
			for (int i = 0; i < list.Length; ++i) {
			}

			// Battle
			for (int i = 0; i < list.Length; ++i) {
				if (!list [i].GetHeroInfo ().IsDead) {
					bool canAttack = list [i].BattleTarget ();
					float moveDuration = list [i].BattleMove ();
					yield return new WaitForSeconds ( moveDuration );
					yield return new WaitForSeconds ( moveInterval );
					list[i].EndMove();
					if ( canAttack ) {
						float AttackDuration = list [i].BattleAttack ();
						yield return new WaitForSeconds (AttackDuration);
						yield return new WaitForSeconds (attackInterval);
						list[i].EndAttack();
					}
				}

				yield return new WaitForSeconds( basicInterval );
			}

			// End
			for (int i = 0; i < list.Length; ++i) {
				if (!list [i].GetHeroInfo ().IsDead) {
					list[i].EndBattle();
				}
			}

			yield return new WaitForSeconds( basicInterval );

			m_stateMachine.State = State.Count;

			///////////////////////////
			/// 
			/// Only One Hero Move
			/// 
			///////////////////////////
		}else if ( mode == Mode.SingleBattle )
		{
			Hero[] list = GetSortedHeroList( heroList.ToArray() );

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

			yield return new WaitForSeconds( basicInterval );

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

			yield return new WaitForSeconds( basicInterval );

			m_stateMachine.State = State.Count;
		}
	}

	/// <summary>
	/// Start a virtual Battle 
	/// </summary>
	/// <returns>The the winning team, if no team wins, return None.</returns>
	/// <param name="virtualheroList">Virtualhero list.</param>
	public TeamColor VirtualBattle( Hero[] virtualheroList )
	{
		if ( mode == Mode.InOrderBattle )	
		{
			Hero[] list = GetSortedHeroList ( virtualheroList );

			// Begin
			for (int i = 0; i < list.Length; ++i) {
				list [i].BeginBattle ();
			}

			BattleField.ResetLock();
			// Strategy
			for (int i = 0; i < list.Length; ++i) {
			}

			// Move
			for (int i = 0; i < list.Length; ++i) {
			}

			// Battle
			for (int i = 0; i < list.Length; ++i) {
				if (!list [i].GetHeroInfo ().IsDead) {
					bool canAttack = list [i].BattleTarget ();
					list [i].BattleMove ();
					list[i].EndMove();
					if ( canAttack ) {
						list [i].BattleAttack ();
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
		}

		bool[] isAllDead = new bool[2];
		for( int i = 0 ; i < isAllDead.Length ; ++ i )
			isAllDead[i] = true;
		foreach (Hero h in virtualheroList)
			if (!h.GetHeroInfo ().IsDead)
				isAllDead [(int)h.GetHeroInfo ().TeamColor] = false;
		if (isAllDead [(int)TeamColor.Blue] && !isAllDead [(int)TeamColor.Red])
			return TeamColor.Red;
		else if (isAllDead [(int)TeamColor.Red] && !isAllDead [(int)TeamColor.Blue])
			return TeamColor.Blue;

		return TeamColor.None;
	}

	Hero[] GetSortedHeroList( Hero[] heros )
	{
		// TODO sort the hero according to the agile
		List<Hero> temList = new List<Hero>( heros );
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
