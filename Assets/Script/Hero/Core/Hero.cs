using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MBehavior {

	[SerializeField] protected Strategy m_strategy;
	[SerializeField] protected Move m_move;
	[SerializeField] protected Attack m_attack;
	[SerializeField] protected Passive m_passive;
	[SerializeField] protected HeroAnim m_heroAnim;
	[SerializeField] protected HeroInfo m_info;

	public Attack Attack { get { return m_attack; }}
	public Move Move { get{ return m_move; } }
	public HeroAnim HeroAnim { get{ return m_heroAnim; } }
	public Strategy Strategy { get{ return m_strategy; } }
	public Passive Passive { get{ return m_passive; } }

	public bool isVirtual = false;

	public Block TemBlock{
		get {
			return m_temBlock;
		}

		set {
			if (m_temBlock != null && m_temBlock.linkedBlock != null )
				m_temBlock.linkedBlock.RegisterHero (null);
			m_temBlock = value;
			if (m_temBlock != null && m_temBlock.linkedBlock != null )
				m_temBlock.linkedBlock.RegisterHero (this);
		}
	}
	[SerializeField] protected Block m_temBlock;
	public SimBlock TemSimpleBlock
	{
		get {
			if ( TemBlock == null )
				return new SimBlock( -1 , -1 );
			return TemBlock.SimpleBlock;
		}
	}
	Block targetBlock;
	Direction targetDirection;


	virtual public HeroInfo GetHeroInfo()
	{
		return m_info;
	}

	protected override void MStart ()
	{
		base.MStart ();

		if ( !m_IsInit )
			Init();
	}

	bool m_IsInit = false;
	public virtual void Init()
	{
		if ( m_info == null)
			m_info = GetComponent<HeroInfo> ();
		if ( m_info != null )
			m_info.Init( this );
		if (m_strategy == null)
			m_strategy = GetComponent<Strategy> ();
		if ( m_strategy != null )
			m_strategy.Init( this );
		if (m_move == null)
			m_move = GetComponent<Move> ();
		if ( m_move != null )
			m_move.Init( this );
		if (m_attack == null)
			m_attack = GetComponent<Attack> ();
		if ( m_attack != null )
			m_attack.Init( this );
		if (m_passive == null)
			m_passive = GetComponent<Passive> ();
		if ( m_passive != null )
			m_passive.Init( this );
		if ( m_heroAnim == null)
			m_heroAnim = GetComponent<HeroAnim> ();
		if ( m_heroAnim != null )
			m_heroAnim.Init( this );

		GetHeroInfo().DeathFunc += delegate {
			TemBlock.isLock = false;
			TemBlock = null;
		};
		m_IsInit = true;
	}

	public void Init( RawHeroInfo info )
	{
		Init();
		if ( m_info != null )
			m_info.Init( info );
		SetBlock( info.block );
	}

	// Set and move to target block
	virtual public void SetBlock( SimBlock block )
	{
		TemBlock = BattleField.GetBlock( block );
	}
		

	public virtual void BeginBattle()
	{
		if ( m_passive != null )
			m_passive.BeginBattle();

		if ( m_strategy != null )
			GetHeroInfo().isActive = m_strategy.GetActive();
	}

	/// <summary>
	/// Set the move target 
	/// </summary>
	/// <returns><c>true</c>, if target was not blocked, the move succeed <c>false</c> otherwise.</returns>
	public bool BattleTarget() {
		GetHeroInfo().Record( null , HistoryStep.RecordType.StartBattle );
		
		targetBlock = BattleField.GetBlock( m_strategy.GetTarget ());
		targetDirection = m_strategy.GetDirection ();


		bool isBlock = targetBlock.isLock;
		if (isBlock) {
//			targetBlock = GetNearestBlock(targetBlock);
//			if ( targetBlock == null )
			BattleField.ShowBlock (new SimBlock[] { targetBlock.SimpleBlock }, BattleBlock.BlockVisualType.BattleMoveTargetBlocked, true);
			targetBlock = TemBlock;
			UnableToMove ();
		} else {
			BattleField.ShowBlock( new SimBlock[] {targetBlock.SimpleBlock} , BattleBlock.BlockVisualType.BattleMoveTarget , true);
		}
		TemBlock.isLock = false;
		targetBlock.isLock = true;

		return !isBlock;
	}

	/// <summary>
	/// Gets the nearest block from target
	/// </summary>
	/// <returns>The nearest block. returns null if all blocks are locked</returns>
	/// <param name="target">Target.</param>
	public Block GetNearestBlock( Block target )
	{
		List<SimBlock> moveRange = new List<SimBlock>(m_move.GetMoveRange());
		moveRange.Sort(delegate(SimBlock x, SimBlock y) {
			int disX = target.SimpleBlock.GetDistance(x);	
			int disY = target.SimpleBlock.GetDistance(y);
			return disX.CompareTo(disY);
		});

//		Debug.Log(" target " + target.SimpleBlock);

		for( int i = 0 ; i < moveRange.Count ; ++ i ) {
			Block res = BattleField.GetBlock( moveRange[i] );
//			Debug.Log(string.Format( " i = {0} block = {1} dist = {2}" , i , res.SimpleBlock , target.GetDistance(res)));

			if ( !res.isLock ) {
				return res;
			}
		}

		return null;
	}

	public virtual void UnableToMove() {
		if ( m_heroAnim != null )
			m_heroAnim.MoveBlocked();
	}

	/// <summary>
	/// Move to the target block
	/// </summary>
	/// <returns>The move.</returns>
	public virtual float BattleMove (){
		TemBlock = targetBlock;
		GetHeroInfo ().Direction = targetDirection;

		if ( m_heroAnim != null )
		{
			return m_heroAnim.MoveTo(targetBlock.SimpleBlock);
		}
		return 0;
	}

	public virtual void EndMove()
	{
		
	}

	/// <summary>
	/// Do the attack
	/// </summary>
	/// <returns>The attack.</returns>
	public virtual float BattleAttack () {
		Damage[] dmgs = m_attack.GetAllDamage();

		if ( m_passive != null )
		{
			m_passive.ChangeDamageOnAttack( ref dmgs );
		}
			
		GetHeroInfo().Record(dmgs , HistoryStep.RecordType.AfterAttack );

		if ( m_heroAnim != null )
		{
			BattleField.ShowBlock( m_attack.GetAttackRange() , BattleBlock.BlockVisualType.BattleAttackRange );
			BattleField.ShowBlock( m_attack.GetTargetBlock() , BattleBlock.BlockVisualType.BattleAttackTarget , false);
			BattleField.ShowBlock( new SimBlock[]{ TemSimpleBlock } , BattleBlock.BlockVisualType.BattleAttackHero , false);
			// the damage will be sent during the animation
			return m_heroAnim.Attack( dmgs , m_attack.GetTargetBlock() , m_attack.GetAttackRange() );
		}else{
			foreach( Damage d in dmgs )
			{
				SendDamage( d );
			}
		}
		return 0;
	}

	public virtual void EndAttack()
	{
		if ( m_heroAnim != null )
		{
			BattleField.ResetVisualColor(true);
		}

	}

	public virtual void EndBattle()
	{
		if ( m_passive != null )
			m_passive.EndBattle();

		GetHeroInfo().UpdateBuff(BuffUpdateType.EndBattle);

		GetHeroInfo().Record( null , HistoryStep.RecordType.EndBattle );

		GetHeroInfo().isActive = false;

	}

	public void SendDamage( Damage dmg )
	{
		Block block = BattleField.GetBlock( dmg.target );
		if (block.linkedBlock != null && block.linkedBlock.GetHeroInfo () != null &&
			block.state == Block.BlockState.Hero) {
				block.linkedBlock.GetHeroInfo().parent.RecieveDamage( dmg );
		}
	}



	public void RecieveDamage( Damage dmg )
	{
		GetHeroInfo().RecieveDamage( ref dmg );
		if ( m_passive != null )
			m_passive.RecieveDamage( ref dmg );
		GetHeroInfo().RecieveDamage( dmg );
	}


//	public bool IsInAttackRange( Block b )
//	{
//		int attackRange = GetHeroInfo ().AttackRange;
//		if (b != null && TemBlock != null) {
//			if (TemBlock.GetDistance (b) <= attackRange)
//				return true;
//		}
//
//		return false;
//	}
//
//	public bool IsInMoveRange( Block b )
//	{
//		int moveRange = GetHeroInfo ().MoveRange;
//		if (b != null && TemBlock != null) {
//			if (TemBlock.GetDistance (b) <= moveRange)
//				return true;
//		}
//
//		return false;
//	}

	public HeroMoveInfo GetMoveInfo()
	{
		HeroMoveInfo res = new HeroMoveInfo();
		res.ID = GetHeroInfo().ID;
		res.ori = TemSimpleBlock;
		res.target = m_strategy.GetTarget();
		res.toDirection = m_strategy.GetDirection();
		res.type = GetHeroInfo().type;
		res.isActive = m_strategy.GetActive();
		return res;
	}
}
