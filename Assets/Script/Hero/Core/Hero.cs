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
	public void SetBlock( SimBlock block )
	{
		TemBlock = BattleField.GetBlock( block );

		Block b = BattleField.GetBlock( block );
		if ( b != null )
			transform.position = b.GetCenterPosition();
	}
		

	public virtual void BeginBattle()
	{
		if ( m_passive != null )
			m_passive.BeginBattle();
	}

	/// <summary>
	/// Lock on one or more target
	/// </summary>
	public void BattleTarget() {
		GetHeroInfo().Record( null , HistoryStep.RecordType.StartBattle );
		
		targetBlock = BattleField.GetBlock( m_strategy.GetTarget ());
		targetDirection = m_strategy.GetDirection ();
		if ( targetBlock.isLock )
		{
			targetBlock = TemBlock;
			UnableToMove();
		}
		TemBlock.isLock = false;
		targetBlock.isLock = true;

		// if the first time enter the battle Target 
	}

	/// <summary>
	/// Move to the target block
	/// </summary>
	/// <returns>The move.</returns>
	public virtual float BattleMove (){
		TemBlock = targetBlock;
		GetHeroInfo ().direction = targetDirection;

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
			BattleField.ShowBlock( new SimBlock[]{ TemBlock.SimpleBlock } , BattleBlock.BlockVisualType.BattleAttackHero );
			BattleField.ShowBlock( m_attack.GetAttackRange() , BattleBlock.BlockVisualType.BattleAttackRange );
			// the damage will be sent during the animation
			return m_heroAnim.Attack( dmgs );
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
		m_passive.RecieveDamage( ref dmg );
		GetHeroInfo().RecieveDamage( dmg );
	}

//	public virtual void UnableToAttack() {
//	}

	public virtual void UnableToMove() {
		
	}

	public bool IsInAttackRange( Block b )
	{
		int attackRange = GetHeroInfo ().AttackRange;
		if (b != null && TemBlock != null) {
			if (TemBlock.GetDistance (b) <= attackRange)
				return true;
		}

		return false;
	}

	public bool IsInMoveRange( Block b )
	{
		int moveRange = GetHeroInfo ().MoveRange;
		if (b != null && TemBlock != null) {
			if (TemBlock.GetDistance (b) <= moveRange)
				return true;
		}

		return false;
	}

	public HeroMoveInfo GetMoveInfo()
	{
		HeroMoveInfo res = new HeroMoveInfo();
		res.ID = GetHeroInfo().ID;
		res.ori = TemBlock.SimpleBlock;
		res.target = m_strategy.GetTarget();
		res.toDirection = m_strategy.GetDirection();
		res.type = GetHeroInfo().type;
		return res;
	}
}
