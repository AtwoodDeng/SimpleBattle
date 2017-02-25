using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StrategyData
{
	public SimBlock simBlock;
	public Direction direction;
	public float winningTime;
	public float testTime;
	public float attackRange;
	public float winningRate{
		get { return 1f * winningTime / ( testTime + 0.001f) * ( 1f + 0.2f * attackRange) ; }
	}
	public StrategyData(SimBlock sb , Direction d )
	{
		simBlock = new SimBlock( sb );
		direction = d;
		winningTime = 0;
		testTime = 0.001f;
	}
	public void AddResult( float result )
	{
		testTime += 1f ;
		winningTime += result;
	}
	public override string ToString ()
	{
		return string.Format ("[StrategyData: toblock={1} dir = {2} winRate ={0} testTime={3}]", 
			winningRate , simBlock , direction,testTime);
	}

	public void SetAttackRange( int range )
	{
		attackRange = (float)range;
	}
}

public class AIStrategy : Strategy {

	int topRound = -1;
	SimBlock bestTarget;
	Direction bestDirection;
	/// <summary>
	/// The Max Round that AI will Calculate
	/// </summary>
	public int AIDepth = 4;

	/// <summary>
	/// The total simulation times
	/// </summary>
//	public int AISimulateTime = 1;

	public int SimulationPerFrame = 4;

	List<StrategyData> strategyList = new List<StrategyData>();

	/// <summary>
	/// Timer of the begin time of calculation
	/// set to -1 if it is not calculating
	/// </summary>
	private float m_calculateTimer;
	public float CalculateTime {
		get{
			if ( CalculateCor != null )
				return Time.time - m_calculateTimer;
			return -1f;
		}
	}

	public override bool IsReady ()
	{
		if ( LogicManager.MState == LogicManager.State.WaitStrategy )
			return CalculateTime > LogicManager.AIStrategyTime;
		return false;
	}

	public override SimBlock GetTarget ()
	{
//		EndCalculate();
		return bestTarget;
	}

	public override Direction GetDirection ()
	{
//		EndCalculate();
		return bestDirection;
	}
	public override void OnBeforeBattle ()
	{
		base.OnBeforeBattle ();
		Calculate ();
	}
	public override void OnBeginBattle ()
	{
		base.OnBeginBattle ();
		EndCalculate ();
	}

	Coroutine CalculateCor;
	public void Calculate( bool IsReplaceOld = false)
	{
		Debug.Log (name + "start Calculate");
		if (CalculateCor != null && IsReplaceOld )
			EndCalculate ();
		if (CalculateCor == null) {
			if (topRound < LogicManager.Instance.Round) {
				topRound = LogicManager.Instance.Round;
				CalculateCor = StartCoroutine (CalculateAICor ());
				m_calculateTimer = Time.time;
			}
		}
	}

	public void EndCalculate()
	{
		Debug.Log (name + "end Calculate");
		if ( CalculateCor != null )
		{
			StopCoroutine( CalculateCor );
			CalculateCor = null;

			int resultIndex = UpdateTarget ();
//			{
//				float totalTestTime = 0;
//				foreach (StrategyData data in strategyList)
//					totalTestTime += data.testTime;
//				Debug.Log ("=====[" + name + "] Total simulate time " + totalTestTime);
//				Debug.Log ("   result :" + strategyList [resultIndex]);
//				foreach (StrategyData data in strategyList)
//					Debug.Log (data);
//			}
		}
		BattleField.EndVirtual();
	}

	/// <summary>
	/// Update the target block and direction
	/// </summary>
	/// <returns>The index of target strategy data.</returns>
	int UpdateTarget()
	{
		int index = Random.Range( 0 , strategyList.Count ) ;
		for( int i = 0 ; i < strategyList.Count ; ++ i ) {
			if ( strategyList[i].winningRate > strategyList[index].winningRate )
				index = i;
		}

		bestTarget = strategyList[index].simBlock;
		bestDirection = strategyList[index].direction;

		return index;
	}

	IEnumerator CalculateAICor()
	{
		strategyList.Clear();

		SimBlock[] moveRange = parent.Move.GetMoveRange ();

		for (int i = 0; i < moveRange.Length; ++i) {
			for (int k = 0; k < 4; ++k) {
				if( ! moveRange[i].Equals( parent.TemSimpleBlock )) // do not move to tem block
					strategyList.Add( new StrategyData( moveRange[i] , (Direction)k ));
			}
		}

		yield return new WaitForEndOfFrame();

		for (int k = 0; k < 4; ++k) {
			BattleField.StartVirtual ();
			for (int i = strategyList.Count / 4 * k; i < strategyList.Count / 4 * (k+1) ; ++i) {
				int range = parent.GetAttackRangeInTarget(
					strategyList[ i ].simBlock,
					strategyList[i].direction
				).Length;
				strategyList [i ].SetAttackRange ( range );
			}
			BattleField.EndVirtual ();
			yield return new WaitForEndOfFrame ();
		}

		while( true )
		{
			BattleField.StartVirtual ();

			for( int i = 0 ; i < SimulationPerFrame ; ++ i )
			{
				StrategyData data = strategyList[Random.Range(0,strategyList.Count)];
				data.AddResult( GetWinTime( data.simBlock , data.direction , 1 ));
			}

//			UpdateTarget ();

			BattleField.EndVirtual ();

			yield return new WaitForEndOfFrame();
		}

		Debug.Log ("========= End AI ==========");

	}

//	void CalculateAI()
//	{
//		strategyList.Clear();
////		Debug.Log ("========= Start AI ==========");
//
//		SimBlock[] moveRange = parent.Move.GetMoveRange ();
//		int[] winTimes = new int[moveRange.Length*4];
//		for (int i = 0; i < winTimes.Length; ++i)
//			winTimes [i] = 0;
//
//
//		BattleField.StartVirtual ();
//		for (int i = 0; i < moveRange.Length; ++i)
//			for (int k = 0; k < 4; ++k) {
//				SimBlock moveTo = moveRange [i];
//				Direction direction = (Direction)k;
//				winTimes [i * 4 + k] = GetWinTime (moveTo, direction , AISimulateTime );
//			}
//
//		BattleField.EndVirtual ();
//
//		Debug.Log ("========= End AI ==========");
//
//		int maxIndex = 0;
//		for (int i = 1; i < winTimes.Length; ++i) {
//			if (winTimes [i] > winTimes [maxIndex])
//				maxIndex = i;
//			Debug.Log( "Win Time " + i + " " + winTimes[i] );
//		}
//		Debug.Log (" Max Winning Time = " + winTimes [maxIndex]);
//
//		bestTarget = moveRange [maxIndex / 4];
//		bestDirection = (Direction)(maxIndex % 4);
//
//	}

	public float GetWinTime( SimBlock moveTo , Direction direction , int interation )
	{
		float totalWin = 0;
//		Debug.Log ("【" + name + "】 Get Win Time by Move to " + moveTo.ToString () + " Direction " + direction);
		for (int k = 0; k < interation ; ++k) {
			int round = 0;
//			Debug.Log ("The k = " + k + " time ");
			BattleField.CopyFromRealToVirtual ();
			Hero virtualHero = BattleField.virtualHeroMap [parent];
			if (virtualHero != null) {
				if (virtualHero.Strategy is AIVirtualStrategy) {
					((AIVirtualStrategy)virtualHero.Strategy).ifFollowNext = true;
					((AIVirtualStrategy)virtualHero.Strategy).nextBlock = moveTo;
					((AIVirtualStrategy)virtualHero.Strategy).nextDirection = direction;
				}
			}
//			Debug.Log ("======= Begin Battle ======");
			while (round < AIDepth) {
//				Debug.Log ("----Round " + round + "----");
				round++;
//				Debug.Log ("The " + round + " round ");
				bool isForceResult = false;
				if (round == AIDepth)
					isForceResult = true;
				float result = LogicManager.Instance.VirtualBattle (BattleField.virtualHeroList.ToArray() ,
					parent.GetHeroInfo().TeamColor, isForceResult);
				if (result >= 0) {
					totalWin += result;
					break;
				}
//				TeamColor winningTeam = LogicManager.Instance.VirtualBattle (BattleField.virtualHeroList.ToArray() , isForceResult);
//				if (winningTeam != TeamColor.None) { // there is a result
//					totalWin += (winningTeam == parent.GetHeroInfo().TeamColor)?1:0;
//					Debug.Log (" winning team is " + winningTeam );
//					break;
//				}
			}
//			Debug.Log ("======= End Battle ======");
		}
//		Debug.Log ("Total wins " + totalWin);
		return totalWin;


	}



}