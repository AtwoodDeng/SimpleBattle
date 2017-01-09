using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

	[SerializeField] protected Strategy strategy;
	[SerializeField] protected Move move;
	[SerializeField] protected Attack attack;

	public Block TemBlock{
		get {
			return m_temBlock;
		}

		set {
			if (m_temBlock != null)
				m_temBlock.linkedBlock.RegisterHero (null);
			m_temBlock = value;
			if (m_temBlock != null)
				m_temBlock.linkedBlock.RegisterHero (this);
		}
	}
	protected Block m_temBlock;
	Block targetBlock;
	Direction targetDirection;

	[SerializeField] HeroInfo info;

	virtual public HeroInfo GetHeroInfo()
	{
		return info;
	}

	virtual public int GetAgile()
	{
		return GetHeroInfo ().agile;
	}

	void Awake()
	{
		if (strategy == null)
			strategy = GetComponent<Strategy> ();
		if (move == null)
			move = GetComponent<Move> ();
		if (attack == null)
			attack = GetComponent<Attack> ();
	}

	public virtual void BattleTarget() {
		targetBlock = strategy.GetTarget ();
		targetDirection = strategy.GetDirection ();
		targetBlock.state = Block.BlockState.Locked;
	}
	public virtual float BattleMove (){
		TemBlock = targetBlock;
		GetHeroInfo ().direction = targetDirection;
		return move.MoveTo (targetBlock);
	}
	public virtual float BattleAttack () {
		return attack.DoAttack ();
	}

	public virtual void UnableToAttack() {
	}

	public virtual void UnableToMove() {
	}


	public bool IsInAttackRange( Block b )
	{
		int attackRange = GetHeroInfo ().attackRange;
		if (b != null && TemBlock != null) {
			if (TemBlock.GetDistance (b) <= attackRange)
				return true;
		}

		return false;
	}

	public bool IsInMoveRange( Block b )
	{
		int moveRange = GetHeroInfo ().moveRange;
		if (b != null && TemBlock != null) {
			if (TemBlock.GetDistance (b) <= moveRange)
				return true;
		}

		return false;
	}
}

[System.Serializable]
public class HeroInfo
{
	public float health{
		get{
			return m_health;
		}
		set {
			HealthChangeFunc (m_health, value);
			m_health = value;
		}
	}
	public float m_health;
	public int moveRange;
	public int attackRange;
	public int agile;
	public float attack;
	public float armor;
	public TeamColor teamColor;
	public delegate void HealthChangeHandler(float fromHealth, float toHealth);
	public event HealthChangeHandler HealthChangeFunc;
	public delegate void DeathHandler();
	public event DeathHandler DeathFunc;
	public Direction direction;
	public bool IsDead{
		get { return health <= 0; }
	}

	public void RecieveDamage( Damage dmg ) 
	{
		if (!IsDead) {
			health -= Mathf.Clamp (dmg.damage - armor, 0, 99999f);
			if (health <= 0) {
				DeathFunc ();
			}
		}
	}

	public void SetDirectionFromAngle( float _a )
	{
		direction = Angle2Direction (_a);
	}

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
}

public class Damage
{
	public float damage;
	HeroInfo caster;
	public Damage( float _d , HeroInfo _c )
	{
		damage = _d;
		caster = _c;
	}
}

public enum TeamColor
{
	Red,
	Blue,
}

public enum Direction
{
	Up,
	Down,
	Left,
	Right,
}