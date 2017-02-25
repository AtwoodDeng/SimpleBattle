using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInfo : HeroComponent {

	#region BasicData
	/// <summary>
	/// The heal of the hero
	/// </summary>
	public float m_health;
	public float m_maxHealth;
	public float Health{
		get{
			return Mathf.Clamp( m_health , 0 , m_maxHealth);
		}
		set {
			if ( HealthChangeFunc !=null )
				HealthChangeFunc (m_health, value);
			m_health = value;
		}
	}
	public float HealthRate{
		get {
			return m_health / (m_maxHealth + 0.001f);
		}
	}
	/// <summary>
	/// The move Range of the hero
	/// </summary>
	/// <value>The move range.</value>
	public int m_moveRange;
	public int MoveRange{
		get {
			int res = m_moveRange;
			foreach( Buff buff in buffList )
			{
				res += buff.GetMoveRange();
			}
			return res;
		}
	}

	/// <summary>
	/// The attack range of the hero
	/// </summary>
	/// <value>The attack range.</value>
	public int m_attackRange;
	public int AttackRange{
		get {
			int res = m_attackRange;
			foreach( Buff buff in buffList )
			{
				res += buff.GetAttackRange();
			}
			return res;
		}
	}

	/// <summary>
	/// The agile of the hero
	/// </summary>
	/// <value>The agile.</value>
	public float m_agile;
	public float Agile{
		get {
			float res = m_agile;
			foreach( Buff buff in buffList )
			{
				res += buff.GetAgile();
			}
			return res;
		}
	}
	/// <summary>
	/// The attack of the hero
	/// </summary>
	/// <value>The attack.</value>
	public float m_attack;
	public float Attack{
		get { 
			float res = m_attack;
			foreach( Buff buff in buffList )
			{
				res += buff.GetAttack();
			}
			return res;
		}
	}

	public TeamColor TeamColor{
		get { return m_teamColor ; }
		set { 
			if ( TeamColorFunc != null )
				TeamColorFunc( m_teamColor , value );
			m_teamColor = value;
		}
	}
	public TeamColor m_teamColor;
	/// <summary>
	/// The type of attack.
	/// </summary>
	public DamageType m_attackType;
	public DamageType AttackType{
		get { 
			return m_attackType;
		}
	}
	/// <summary>
	/// The direction of the hero
	/// </summary>
	public Direction m_direction;
	public Direction Direction{
		get {
			return m_direction;
		}
		set {
			m_direction = value;
		}
	}

	public HeroType type;
	public bool isActive;

	public List<HistoryStep> history;
	public int ID;
	public int LocalID{
		get { return ID % 1000 ; }
	}
	#endregion


	public delegate void HealthChangeHandler(float fromHealth, float toHealth);
	public delegate void DeathHandler();
	public delegate void TeamColorHandler( TeamColor fromColor , TeamColor toColor );

	public event HealthChangeHandler HealthChangeFunc;
	public event DeathHandler DeathFunc;
	public event TeamColorHandler TeamColorFunc;
//	public bool IsDead {
//		get { return Health <= 0; }
//	}
	public bool IsAlive {
		get { return Health > 0;	}
	}

	static int LocalHeroIDCounter = 0;

	public override void Init (Hero hero)
	{
		base.Init (hero);
		ID = NetworkManager.ClientID * 1000 + LocalHeroIDCounter++;
		m_agile += Random.Range( -0.1f , 0.1f );
		m_maxHealth = m_health;
	}

	public void Init( RawHeroInfo info )
	{
		ID = info.ID;
		m_agile = info.aglie;
	}

	public HeroInfo()
	{
		history = new List<HistoryStep>();
		buffList = new List<Buff>();
	}

	public RawHeroInfo GetRawHeroInfo()
	{
		RawHeroInfo res = new RawHeroInfo();
		res.block = parent.TemSimpleBlock;
		res.ID = ID;
		res.type = type;
		res.direction = Direction;
		res.aglie = Agile;

		return res;
	}

	/// <summary>
	/// Deep copy from another hero info.
	/// </summary>
	/// <param name="other">Other.</param>
	public void DeepCopy( HeroInfo other , Dictionary<Hero,Hero> heroMap )
	{
		m_health = other.m_health;
		m_moveRange = other.m_moveRange;
		m_attackRange = other.m_attackRange;
		m_attack = other.m_attack;
		m_agile = other.m_agile;
		m_direction = other.m_direction;
		m_attackType = other.m_attackType;
		m_teamColor = other.m_teamColor;
		if ( other.history != null )
		foreach (HistoryStep step in other.history) {
			history.Add (new HistoryStep (step , heroMap));
		}
		if ( other.buffList != null )
		foreach (Buff buff in other.buffList) {
			buffList.Add( BuffFactory.CreateBuffFrom( buff , heroMap ));
		}

	}


	public void Record( Damage[] dmgs , HistoryStep.RecordType type )
	{
		HistoryStep step = new HistoryStep();
		step.type = type;
		step.block = new SimBlock(parent.TemSimpleBlock);
		step.health = m_health;
		if ( dmgs == null )
			step.damages = new Damage[0];
		else{
			step.damages = new Damage[dmgs.Length];
			for( int i = 0 ; i < dmgs.Length; ++ i ) step.damages[i] = dmgs[i];
		}
		history.Add( step );
	}
		
	#region Buff
	private List<Buff> buffList;

	public Buff GetBuff<T>()
	{
		foreach( Buff b in buffList )
		{
			if ( b is T )
			{
				return b;
			}
		}
		return null;
	}

	public void AddBuff( Buff buf )
	{
		buf.parent = parent;
		if ( buf.addType == BuffAddType.Mutiple )
		{
			buffList.Add( buf );
		}else if ( buf.addType == BuffAddType.Replace )
		{
			for( int i = 0 ; i < buffList.Count ; ++ i )
				if( buffList[i].GetBuffType() == buf.GetBuffType()) {
					buffList.RemoveAt( i );
					break;
				}
			buffList.Add(buf);
		}else if ( buf.addType == BuffAddType.Single )
		{
			bool exist = false;
			for( int i = 0 ; i < buffList.Count ; ++ i )
				if( buffList[i].GetBuffType() == buf.GetBuffType()) {
					exist = true;
					break;
				}
			if ( !exist )
				buffList.Add( buf );
		}else if ( buf.addType == BuffAddType.Add )
		{
			bool exist = false;
			for( int i = 0 ; i < buffList.Count ; ++ i )
				if( buffList[i].GetBuffType() == buf.GetBuffType()) {
					exist = true;
					buffList[i].remainTurn += buf.remainTurn -1;
					break;
				}
			if ( !exist )
				buffList.Add( buf );
		}
	}

	public void RecieveDamage( ref Damage dmg )
	{
		foreach( Buff b in buffList )
		{
			b.OnRecieveDamage( ref dmg );
		}
	}

	public void UpdateBuff( BuffUpdateType updateType )
	{
		foreach( Buff b in buffList )
		{
			b.Update( updateType );
		}
		if ( updateType == BuffUpdateType.EndBattle )
		{
			for( int i = buffList.Count - 1 ; i >= 0 ; i-- )
			{
				if ( !buffList[i].IsActive )
				{
					buffList.RemoveAt(i);
				}
			}
		}
	}

	#endregion

	public void RecieveDamage( Damage dmg ) 
	{
		if (IsAlive) {
			Health -= Mathf.Clamp (dmg.damage , 0, 99999f);
			if ( dmg.buffs != null )
			{
				foreach( Buff b in dmg.buffs )
				{
					AddBuff( b );
				}
			}
			if (Health <= 0) {
				DeathFunc ();
			}
		}
	}

	public void SetDirectionFromAngle( float _a )
	{
		Direction = Angle2Direction (_a);
	}

	#region Tool

	public static Direction Angle2Direction( float _a )
	{
		float angle = _a;

		if (angle >= 45f && angle < 135f) {
			return Direction.Up;
		} else if (angle >= -45f && angle < 45f) {
			return Direction.Right;
		} else if (angle >= -135f && angle < -45f) {
			return Direction.Down;
		} 

		return Direction.Left;
	}

	#endregion 
}

[System.Serializable]
public class RawHeroInfo
{
	public int ID;
	public HeroType type;
	public SimBlock block{
		get {
			return new SimBlock( m_i , m_j ); 
		}
		set {
			m_i = value.m_i;
			m_j = value.m_j;
		}
	}
	public int m_i;
	public int m_j;
	public float aglie;
	public Direction direction;

	public RawHeroInfo Copy()
	{
		RawHeroInfo res = new RawHeroInfo();
		res.type = type;
		res.ID = ID;
		res.block = block;
		res.aglie = aglie;
		res.direction = direction;
		return res;
	}

	public override string ToString ()
	{
		return string.Format ("ID: {2}, type: {3} , block:({0},{1}) , direction {4}, aglie {5}",
			m_i , m_j , ID , type.ToString() , direction , aglie );
	}

}

[System.Serializable]
public class HeroMoveInfo
{
	public int ID;
	public bool isActive;
	public SimBlock target{
		get {
			return new SimBlock( t_i , t_j );
		}
		set {
			t_i = value.m_i;
			t_j = value.m_j;
		}
	}
	public int t_i;
	public int t_j;
	public Direction toDirection;

	public HeroType type;
	public SimBlock ori{
		get {
			return new SimBlock( o_i , o_j );
		}
		set {
			o_i = value.m_i;
			o_j = value.m_j;
		}
	}
	public int o_i;
	public int o_j;



	public HeroMoveInfo Copy()
	{
		HeroMoveInfo res = new HeroMoveInfo();
		res.ID = ID;
		res.target = target;
		res.toDirection = toDirection;
		res.isActive = isActive;

		res.ori = ori;
		res.type = type;
		return res;
	}
}

public class HistoryStep
{
	public enum RecordType
	{
		StartBattle,
		AfterAttack,
		EndBattle,
	}
	public RecordType type;
	public SimBlock block;
	public Damage[] damages;
	public float health;
	public HistoryStep()
	{
		
	}

	public HistoryStep( HistoryStep other , Dictionary<Hero,Hero> heroMap )
	{
		type = other.type;
		block = new SimBlock (other.block);
		health = other.health;
		if (other.damages != null) {
			damages = new Damage[other.damages.Length];
			for (int i = 0; i < damages.Length; i++) {
				damages [i] = new Damage (other.damages [i], heroMap);
			}
		}
	}
}

public class Damage
{
	public float damage;
	public DamageType type;
	public bool IsHeal
	{
		get { return damage < 0 ; }
	}
	public Hero caster;
	public SimBlock target;
	public Buff[] buffs;

	public Damage( float _d , DamageType _type , Hero _h , SimBlock _t )
	{
		damage = _d;
		caster = _h;
		target = _t;
		type = _type;
	}
	public Damage( Damage other , Dictionary<Hero,Hero> heroMap )
	{
		damage = other.damage;
		type = other.type;
		caster = heroMap [other.caster];
		target = new SimBlock (other.target);
		if (other.buffs != null) {
			buffs = new Buff[other.buffs.Length];
			for (int i = 0; i < other.buffs.Length; ++i) {
				buffs [i] = BuffFactory.CreateBuffFrom (other.buffs [i], heroMap);
			}
		}
	}
}

public enum DamageType
{
	Physics,
	Magic,
	FireBuff,
}

public enum TeamColor
{
	Red=0,
	Blue=1,
	None=10,
}

public enum Direction
{
	Up,
	Down,
	Left,
	Right,
}
