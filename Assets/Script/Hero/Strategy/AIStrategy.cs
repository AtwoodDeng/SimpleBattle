using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStrategy : Strategy {

	int topRound = -1;
	SimBlock bestTarget;
	Direction bestDirection;
	/// <summary>
	/// The Max Round that AI will Calculate
	/// </summary>
	public int AIDepth = 1000;

	/// <summary>
	/// The total simulation times
	/// </summary>
	public int AISimulateTime = 1;

	public override SimBlock GetTarget ()
	{
		Calculate ();
		return bestTarget;
	}

	public override Direction GetDirection ()
	{
		Calculate ();
		return bestDirection;
	}

	public void Calculate()
	{
		if (topRound < LogicManager.Instance.Round) {
			topRound = LogicManager.Instance.Round;
//			StartCoroutine( CalculateAI ());
			CalculateAI();
		}
	}

	void CalculateAI()
	{
//		Debug.Log ("========= Start AI ==========");

		SimBlock[] moveRange = parent.Move.GetMoveRange ();
		int[] winTimes = new int[moveRange.Length*4];
		for (int i = 0; i < winTimes.Length; ++i)
			winTimes [i] = 0;


		BattleField.StartVirtual ();
		for (int i = 0; i < moveRange.Length; ++i)
			for (int k = 0; k < 4; ++k) {
				SimBlock moveTo = moveRange [i];
				Direction direction = (Direction)k;
				winTimes [i * 4 + k] = GetWinTime (moveTo, direction);
			}

		BattleField.EndVirtual ();

		Debug.Log ("========= End AI ==========");

		int maxIndex = 0;
		for (int i = 1; i < winTimes.Length; ++i)
			if (winTimes [i] > winTimes [maxIndex])
				maxIndex = i;
		Debug.Log (" Max Winning Time = " + winTimes [maxIndex]);

		bestTarget = moveRange [maxIndex / 4];
		bestDirection = (Direction)(maxIndex % 4);

	}

	public int GetWinTime( SimBlock moveTo , Direction direction )
	{
		int totalWin = 0;
//		Debug.Log ("Move to " + moveTo.ToString () + " Direction " + direction);
		for (int k = 0; k < AISimulateTime; ++k) {
			int round = 0;
//			Debug.Log ("The k = " + k + " time ");
			BattleField.CopyFromRealToVirtual ();
			Hero virtualHero = BattleField.virtualHeroMap [parent];
			if (virtualHero != null) {
				if (virtualHero.Strategy is RandomStrategy) {
					((RandomStrategy)virtualHero.Strategy).ifFollowNext = true;
					((RandomStrategy)virtualHero.Strategy).nextBlock = moveTo;
					((RandomStrategy)virtualHero.Strategy).nextDirection = direction;
				}
			}
			while (round < AIDepth) {
				round++;
//				Debug.Log ("The " + round + " round ");
				TeamColor winningTeam = LogicManager.Instance.VirtualBattle (BattleField.virtualHeroList.ToArray() );
				if (winningTeam != TeamColor.None) { // there is a result
					totalWin += (winningTeam == parent.GetHeroInfo().TeamColor)?1:0;
//					Debug.Log (" winning team is " + winningTeam );
					break;
				}
			}
		}
//		Debug.Log ("Total wins " + totalWin);
		return totalWin;


	}



}
