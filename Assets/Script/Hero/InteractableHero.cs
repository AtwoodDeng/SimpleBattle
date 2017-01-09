using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHero : Hero {

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


	protected Vector2 delta;
	protected BattleBlock TriggerBlock{
		get{
			if ( triggerBlockList.Count > 0 )
				return triggerBlockList [0];
			return null;
		}
		set {
			if (TriggerBlock != null)
				TriggerBlock.ExitHero (this);
			if (value != null) {
				triggerBlockList.Insert (0, value);
				TriggerBlock.EnterHero (this);
			} else {
				triggerBlockList.RemoveAt (0);
				if (TriggerBlock != null)
					TriggerBlock.EnterHero (this);
			}
		}
	}
	protected List<BattleBlock> triggerBlockList = new List<BattleBlock>();

	/// <summary>
	/// On Finger Up
	/// </summary>
	public virtual void Confirm() {
	}

	/// <summary>
	/// On Finger Down
	/// </summary>
	public virtual void Select (){
	}

	/// <summary>
	/// EnterBlock
	/// </summary>
	public virtual void EnterBlock ( BattleBlock block ) {
		if (block == null)
			return;

		TriggerBlock = block;
	}

	public virtual void ExitBlock( BattleBlock block ) {
		if (TriggerBlock == block)
			TriggerBlock = null;
		else
			triggerBlockList.Remove (block);
	}

	void OnFingerDown( FingerDownEvent e )
	{
		Select ();
	}

	void OnFingerUp( FingerUpEvent e )
	{
		Confirm ();
	}

	void OnFingerMove( FingerMotionEvent e )
	{
		delta = e.Finger.DeltaPosition;
	}

	void OnTriggerEnter( Collider col )
	{
		if (col.gameObject.layer == LayerMask.NameToLayer ("BattleBlock")) {
			
			BattleBlock block = col.gameObject.GetComponent<BattleBlock> ();

			EnterBlock (block);
//			if (block != null) {
//				block.EnterHero (this);
//			}

		}
	}

	void OnTriggerExit( Collider col )
	{
		if (col.gameObject.layer == LayerMask.NameToLayer ("BattleBlock")) {
			ExitBlock (TriggerBlock);
//			if (TriggerBlock != null && TriggerBlock.gameObject == col.gameObject) {
//			}
		}
	}

}