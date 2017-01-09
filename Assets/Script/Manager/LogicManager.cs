using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour {


	static public LogicManager Instance{ get { return m_Instance; } }
	static public LogicManager m_Instance;
	public LogicManager(){ m_Instance = this; }


	public static float moveDuration = 0.25f;
	public static float moveInterval = 0.1f;
	public static float attackDuration = 1f;
	public static float attackInterval = 0.1f;

	public enum State
	{
		None,
		PlaceHero,
		Strategy,
		Battle,
		Count,
	}

	AStateMachine<State,LogicEvents> m_stateMachine;

	List<Hero> heroList = new List<Hero>();

	void Awake() {
		InitStateMachine ();

	}

	void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<State, LogicEvents> (State.None);
		m_stateMachine.AddEnter (State.PlaceHero, delegate {
			M_Event.FireLogicEvent(LogicEvents.PlaceHeroPhase,new LogicArg(this));	
		});

		m_stateMachine.AddEnter (State.Strategy, delegate {
			M_Event.FireLogicEvent(LogicEvents.StrategyPhase , new LogicArg(this));
		});

		m_stateMachine.AddEnter (State.Battle, OnBattle);

		m_stateMachine.AddEnter (State.Count, delegate {
			m_stateMachine.State = State.Strategy;	
		});

	}

	void Start()
	{
		heroList.AddRange (FindObjectsOfType<Hero> ());

		m_stateMachine.State = State.PlaceHero;
	}

	void OnBattle(){
		StartCoroutine (DoBattle());
	}

	IEnumerator DoBattle()
	{
		Hero[] list = GetSortedHeroList ();
		Debug.Log ("List Length" + list.Length);
		for (int i = 0; i < list.Length; ++i) {
			list [i].BattleTarget ();
		}

		for (int i = 0; i < list.Length; ++i) {
			float duration = list [i].BattleMove ();
			yield return new WaitForSeconds (duration + moveInterval);
		}

		yield return new WaitForSeconds( 0.5f );


		for (int i = 0; i < list.Length; ++i) {
			if (!list [i].GetHeroInfo ().IsDead) {
				Debug.Log ("Begin Attack " + list [i].name);
				float duration = list [i].BattleAttack ();
				yield return new WaitForSeconds (duration + attackInterval);
			}
		}

		yield return new WaitForSeconds( 0.5f );

		m_stateMachine.State = State.Count;
	}

	Hero[] GetSortedHeroList()
	{
		// TODO sort the hero according to the agile
		List<Hero> temList = new List<Hero>( heroList.ToArray() );
		for (int i = temList.Count -1; i >= 0 ; --i)
			if (temList [i].GetHeroInfo().IsDead)
				temList.RemoveAt (i);
		
		temList.Sort ((x, y) => {
			return - x.GetAgile().CompareTo(y.GetAgile());
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
			m_stateMachine.State = State.Strategy;
		} else if (m_stateMachine.State == State.Strategy) {
			m_stateMachine.State = State.Battle;
		}
	}

	public void AddOnPhaseChange( AStateMachine<State,LogicEvents>.StateChangeHandler handler  )
	{
		m_stateMachine.AddOnChange (handler);
	}

}
