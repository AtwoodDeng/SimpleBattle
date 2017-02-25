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
	public bool isAI;
	public bool isAutoPlay;
	public static bool IsAutoPlay
	{
		get {
			if (Instance != null)
				return Instance.isAutoPlay;
			return false;
		}
	}

	public static float moveDuration = 0.8f;
	public static float targetInterval = 0.5f;
	public static float moveInterval = 0.1f;
	public static float attackDuration = 1f;
	public static float attackInterval = 0.1f;
	// interval between different phase and two hero's move
	public static float basicInterval = 0.5f;
	public static float AIStrategyTime = 5f;

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
	static public State MState{
		get { 
			if (Instance == null)
				return State.None;
			return Instance.m_stateMachine.State;
		}
	}
	private float m_stateStartTimer;
	/// <summary>
	/// The seconds from the temperary has started
	/// </summary>
	/// <value>The state time.</value>
	static public float StateTime{
		get{
			if (Instance == null)
				return 0;
			return Time.time - Instance.m_stateStartTimer;
		}
	}

	AStateMachine<State,LogicEvents> m_stateMachine;

	List<Hero> heroList = new List<Hero>();

	protected override void MAwake ()
	{
		base.MAwake ();
		InitStateMachine ();
	}

	protected override void MStart ()
	{
		base.MStart ();
		//		heroList.AddRange (FindObjectsOfType<Hero> ());

		m_stateMachine.State = State.PlaceHero;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			NextPhase ();
		}
		m_stateMachine.Update ();
	}

	#region StateMachine
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
			RecordTime();

		});

		m_stateMachine.AddUpdate (State.Strategy, delegate() {
			if ( IsAutoPlay )
				m_stateMachine.State = State.WaitStrategy;	
		});

		m_stateMachine.AddEnter( State.WaitStrategy , delegate {
			if ( isOnline )
			{	
				List<HeroMoveInfo> heros = new List<HeroMoveInfo>();
				foreach( Hero h in heroList )
				{
					if ( h.GetHeroInfo().TeamColor == TeamColor.Blue && h.GetHeroInfo().IsAlive)
						heros.Add( h.GetMoveInfo() );
				}
				Debug.Log("Send Move Hero");
				NetworkManager.Instance.SendMoveHero( heros.ToArray() );
			}else if ( isAI ) {
			
			}else
			{
				m_stateMachine.State = State.Battle;
			}
		});

		m_stateMachine.AddUpdate (State.WaitStrategy, delegate {
			if ( isAI ) {
				if ( IsAllReady(TeamColor.Blue) && IsAllReady( TeamColor.Red ))
					m_stateMachine.State = State.Battle;
			}
		});

		m_stateMachine.AddEnter (State.Battle, OnBattle);

		m_stateMachine.AddEnter (State.Count, delegate {

		m_stateMachine.State = State.Strategy;	
		});

	}

	void RecordTime()
	{
		m_stateStartTimer = Time.time;
	}

	#endregion



	#region Battle
	/// <summary>
	/// Called When the game is initialized
	/// </summary>
	public void InitGame()
	{
		m_round = 0;
	}
	/// <summary>
	/// Call all hero .BeforeBattle
	/// and increae the number of round
	/// </summary>
	void OnBeforeBattle( Hero[] heros )
	{
		m_round++;

		Hero[] list = GetSortedHeroList ( heros );

		for (int i = 0; i < list.Length; ++i) {
			list [i].BeforeBattle ();
		}
	}
	/// <summary>
	/// Start the battle coroutine
	/// </summary>
	void OnBattle(){
		StartCoroutine (DoBattle());
		RecordTime ();
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
			yield return new WaitForSeconds( targetInterval );

			// Move
			for (int i = 0; i < list.Length; ++i) {
				float duration = list [i].BattleMove ();
				yield return new WaitForSeconds (duration + moveInterval);
				list[i].EndMove();
			}

			yield return new WaitForSeconds( basicInterval );

			// Battle
			for (int i = 0; i < list.Length; ++i) {
				if (list [i].GetHeroInfo ().IsAlive) {
	//				Debug.Log ("Begin Attack " + list [i].name);
					float duration = list [i].BattleAttack ();
					yield return new WaitForSeconds (duration + attackInterval);
					list[i].EndAttack();
				}
			}

			// End
			for (int i = 0; i < list.Length; ++i) {
				if (list [i].GetHeroInfo ().IsAlive) {
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
				if (list [i].GetHeroInfo ().IsAlive) {
					bool canAttack = list [i].BattleTarget ();
					yield return new WaitForSeconds( targetInterval );
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
				if (list [i].GetHeroInfo ().IsAlive) {
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
			yield return new WaitForSeconds( targetInterval );

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
					if (list [i].GetHeroInfo ().IsAlive) {
						//				Debug.Log ("Begin Attack " + list [i].name);
						float duration = list [i].BattleAttack ();
						yield return new WaitForSeconds (duration + attackInterval);
						list[i].EndAttack();
					}
				}
			}

			// End
			for (int i = 0; i < list.Length; ++i) {
				if (list [i].GetHeroInfo ().IsAlive) {
					list[i].EndBattle();
				}
			}

			yield return new WaitForSeconds( basicInterval );

			m_stateMachine.State = State.Count;
		}
	}

	/// <summary>
	/// Start a virtual battle
	/// </summary>
	/// <returns>The power of the result, return -0.001 if even.</returns>
	/// <param name="virtualheroList">Virtualhero list.</param>
	/// <param name="which">which team to detect.</param>
	/// <param name="forceResult">If set to <c>true</c> force result.</param>
	public float VirtualBattle( Hero[] virtualheroList , TeamColor which, bool forceResult = false )
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
				if (list [i].GetHeroInfo ().IsAlive) {
					bool canAttack = list [i].BattleTarget ();
					list [i].BattleMove ();
					list[i].EndMove();
					if (canAttack) {
						list [i].BattleAttack ();
						list [i].EndAttack ();
					} else {
//						Debug.Log ("**=" + list [i].name + " is blocked");
					}
				}
			}
//			foreach (Hero h in list) {
//				Debug.Log(string.Format("  ={0}: pos {1} dir {2} heal {3} " ,
//					h.name , h.TemSimpleBlock , h.GetHeroInfo().Direction , h.GetHeroInfo().Health));
//			}
			// End
			for (int i = 0; i < list.Length; ++i) {
				if (list [i].GetHeroInfo ().IsAlive) {
					list[i].EndBattle();
				}
			}
		}

		if (forceResult) {
			float[] totalHealthRate = new float[2];
			for (int i = 0; i < totalHealthRate.Length; ++i)
				totalHealthRate [i] = 0;
			foreach (Hero h in virtualheroList) {
				totalHealthRate [(int)h.GetHeroInfo ().TeamColor] += h.GetHeroInfo().HealthRate;
			}
			float healthBlue = totalHealthRate [(int)TeamColor.Blue] + 0.0001f;
			float healthRed = totalHealthRate [(int)TeamColor.Red] + 0.0001f;
			if (which == TeamColor.Blue) {
				return Mathf.Pow( healthBlue / (healthBlue + healthRed) , 2f );
			}
			return Mathf.Pow( healthRed / (healthRed + healthBlue) , 2f );
		} else {
			bool[] isAllDead = new bool[2];
			for (int i = 0; i < isAllDead.Length; ++i)
				isAllDead [i] = true;
			foreach (Hero h in virtualheroList)
				if (h.GetHeroInfo ().IsAlive)
					isAllDead [(int)h.GetHeroInfo ().TeamColor] = false;
			if (isAllDead [(int)TeamColor.Blue] && !isAllDead [(int)TeamColor.Red])
				return ( which == TeamColor.Red)? 1.0f : 0 ;
			else if (isAllDead [(int)TeamColor.Red] && !isAllDead [(int)TeamColor.Blue])
				return ( which == TeamColor.Blue)? 1.0f : 0;
		}
		return -0.0001f;
	}
	#endregion

	#region HeroList


	public void RegisterHero( Hero h )
	{
		if ( !heroList.Contains(h) )
			heroList.Add(h);
	}

	public void UnregisterHero( Hero h )
	{
		heroList.Remove(h);
	}

	Hero[] GetSortedHeroList( Hero[] heros )
	{
		// TODO sort the hero according to the agile
		List<Hero> temList = new List<Hero>( heros );
		for (int i = temList.Count -1; i >= 0 ; --i)
			if (!temList [i].GetHeroInfo().IsAlive)
				temList.RemoveAt (i);
		
		temList.Sort ((x, y) => {
			return - x.GetHeroInfo().Agile.CompareTo(y.GetHeroInfo().Agile);
		});

		return temList.ToArray ();
	}

	/// <summary>
	/// Determines whether this team is all ready for battle
	/// (the strategy has done its calculation )
	/// </summary>
	/// <returns><c>true</c> if this instance is all ready the specified team; otherwise, <c>false</c>.</returns>
	/// <param name="team">Which team.</param>
	public bool IsAllReady( TeamColor team )
	{
		bool res = true;
		foreach (Hero h in heroList) {
			if (h.GetHeroInfo ().TeamColor == team && !h.IsReady () && h.GetHeroInfo().IsAlive)
				res = false;
		}
		return res;
	}

	#endregion


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
		M_Event.RegisterEvent(LogicEvents.UIAutoBattle , OnAutoPlay );
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.UnregisterEvent(LogicEvents.NetPlaceHero , OnPlaceHeroNetwork );
		M_Event.UnregisterEvent(LogicEvents.NetMoveHero , OnMoveHeroNetwork );
		M_Event.UnregisterEvent(LogicEvents.UIAutoBattle , OnAutoPlay );
	}


	void OnAutoPlay( LogicArg arg )
	{
		isAutoPlay = (bool) arg.GetMessage ("ifAuto");
		M_Event.FireLogicEvent (LogicEvents.AutoBattle, new LogicArg (this));
	}

	/// Network
	void OnPlaceHeroNetwork( LogicArg arg )
	{
		if ( MState == State.WaitPlaceHero )
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
		if ( MState == LogicManager.State.WaitStrategy )
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
