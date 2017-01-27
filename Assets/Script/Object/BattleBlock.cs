using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class BattleBlock : MonoBehaviour {

	public enum BlockVisualType
	{
		Normal,
		// showing move range on this block
		MoveRange,
		// showing attack range on this block
		AttackRange,
		// focuse on a hero, then the hero's block will change to this 
		StrategyFocus,
		// confirm a hero's move, then the hero's block will change to this 
		StrategyConfirm,
		// in battle, when attack, shows the attack hero
		BattleAttackHero,
		// in battle, when attack, shows the attack range
		BattleAttackRange,

	}
	BlockVisualType m_visualType;
	public BlockVisualType visualType
	{
		get { return m_visualType; }
		set {
			if (  m_block.state == Block.BlockState.Empty )
			{
				switch( value )
				{
				case BlockVisualType.Normal:
						RefreshColorByBlock();
					break;
				case BlockVisualType.AttackRange:
					BackgroundSetColor(colorSetting.AttackColor);
					break;
				case BlockVisualType.MoveRange:
					BackgroundSetColor(colorSetting.MoveRangeColor);
					break;
				case BlockVisualType.BattleAttackRange:
					BackgroundSetColor(colorSetting.BattleAttackRange);
					break;
				default:
					break;
				}
			}
			if (  m_block.state == Block.BlockState.Hero )
			{
				switch( value )
				{
				case BlockVisualType.Normal:
					RefreshColorByBlock();
					break;
				case BlockVisualType.AttackRange:
					BackgroundSetColor(colorSetting.AttackColor);
					break;
				case BlockVisualType.MoveRange:
					BackgroundSetColor(colorSetting.MoveRangeColor);
					break;
				case BlockVisualType.StrategyFocus:
					BackgroundSetColor(colorSetting.StrategyFocusColor);
					break;
				case BlockVisualType.StrategyConfirm:
					BackgroundSetColor(colorSetting.StrategyConfirmColor);
					break;
				case BlockVisualType.BattleAttackHero:
					BackgroundSetColor(colorSetting.BattleAttackHero);
					break;
				case BlockVisualType.BattleAttackRange:
					BackgroundSetColor(colorSetting.BattleAttackRangeHero);
					break;
				default:
					break;
				};
			}
		}
	}
	[System.Serializable]
	public struct BlockColorSetting
	{
		public Color EmptyColor;
		public Color HeroColor;
		public Color MoveRangeColor;
		public Color AttackColor;
		public Color StrategyFocusColor;
		public Color StrategyConfirmColor;
		public Color BattleAttackHero;
		public Color BattleAttackRange;
		public Color BattleAttackRangeHero;
	};
	[SerializeField] BlockColorSetting colorSetting;


	Block.BlockState state{
		get {
			return m_block.state;
		}
		set {
			m_block.state = value;
			RefreshColorByBlock();
		}
	}

	public void RefreshColorByBlock()
	{
		switch( m_block.state )
		{
		case Block.BlockState.Empty:
			BackgroundSetColor( colorSetting.EmptyColor );
			break;
		case Block.BlockState.Hero:
			BackgroundSetColor( colorSetting.HeroColor );
			break;
		default:
			break;
		};
	}

	public bool IsSelected{
		get {
			return m_block.IsSelected;

		}
		set {
			m_block.IsSelected = value;
		}
	}

	public bool IsPlacable{
		get {
			return m_block.isPlacable;
		}
	}

	[SerializeField] float interval = 0.05f;

	float Width{
		get { return transform.localScale.x + interval; }
	}
	float Height{
		get { return transform.localScale.y + interval; }
	}
	[SerializeField] SpriteRenderer background;


	BattleField parent;
	public Block BlockInfo{
		get { return m_block; }
	}
	[SerializeField] Block m_block;
	Hero m_hero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public HeroInfo GetHeroInfo()
	{
		return m_hero.GetHeroInfo ();
	}

	public void Init( int i , int j , BattleField p )
	{

		m_block = new Block (i, j, this);
		parent = p;

		transform.parent = p.transform;
		transform.localPosition = new Vector3 ((i - (p.gridWidth - 1) * 0.5f ) * Width,
			(j - (p.gridHeight - 1) * 0.5f  ) * Height, 0);

	}

	public Vector3 GetCenterPosition()
	{
		return transform.parent.position + new Vector3 ((m_block.m_i - (parent.gridWidth - 1) * 0.5f ) * Width,
			(m_block.m_j - (parent.gridHeight - 1) * 0.5f  ) * Height, 0);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube (transform.position, new Vector3( Width , Height , 0.1f ) );
	}

	void OnFingerHover( FingerHoverEvent e)
	{
	}

	void OnFingerDown()
	{
		parent.OnBlockSelect (m_block);
	}

	void OnFingerUp()
	{
		parent.OnBlockConfirm (m_block);
	}



	public void EnterHero( Hero h )
	{
		if (state == Block.BlockState.Empty && IsPlacable) {
			background.DOColor (new Color (0.5f, 0.5f, 1f), 0.2f);
		}
	}

	public void ExitHero( Hero h )
	{
		if (state == Block.BlockState.Empty ) {

			background.DOColor ( Color.white , 0.2f);
		}
	}

	public void RegisterHero( Hero h )
	{
		m_hero = h;
		if (h == null) {
			state = Block.BlockState.Empty;
//			background.DOColor ( Color.white , 0.2f);

		} else {
			state = Block.BlockState.Hero;
//			background.DOColor ( Color.gray , 0.2f);
		}
	}

	public void BackgroundShine( Color col ){
		background.DOKill ();
		background.DOColor (col, 0.5f).SetLoops (9999, LoopType.Yoyo);

	}

	private void BackgroundSetColor( Color col ){
		background.DOKill ();
		background.DOColor (col,0.2f);
	}

	private void BackgroundReset(){
		background.DOKill ();
		background.DOColor (Color.white,0);
	}

	public void ResetVisual()
	{
		visualType = BlockVisualType.Normal;
		RefreshColorByBlock();
	}


}

[System.Serializable]
public class Block
{
	public enum BlockState
	{
		None,
		Empty,
		Hero,
		Object,
	}

	public bool isLock;

	public bool isPlacable;
	public BlockState state;
	public bool IsSelected;
	public BattleBlock linkedBlock;
	public SimBlock SimpleBlock{
		get {
			return new SimBlock(m_i,m_j);
		}
	}
	public int m_i,m_j;

	public Block( int i ,int j , BattleBlock lb )
	{
		m_i = i;
		m_j = j;
		linkedBlock = lb;
		state = BlockState.Empty;
		isPlacable = false;
		isLock = false;
	}

	public override bool Equals (object obj)
	{
		if (obj is Block) {
			return ((Block)obj).m_i == m_i && ((Block)obj).m_j == m_j; 
		}
		return base.Equals (obj);
	}

	public int GetDistance( Block toward )
	{
		return Mathf.Abs (m_i - toward.m_i) + Mathf.Abs( m_j - toward.m_j);
	}

	public Vector3 GetCenterPosition()
	{
		if (linkedBlock != null) {
			return linkedBlock.GetCenterPosition ();
		}

		return Vector3.zero;
	}


}

[System.Serializable]
public class SimBlock
{
	public int m_i,m_j;
	public SimBlock()
	{
		m_i = m_j = 0;
	}
	public SimBlock( int _i , int _j ){
		m_i = _i;
		m_j = _j;
	}

	public SimBlock( SimBlock sb ){
		m_i = sb.m_i;
		m_j = sb.m_j;
		
	}

	public bool Equals (SimBlock sb)
	{
		return ( m_i == sb.m_i ) && (m_j == sb.m_j);
	}

	public int GetDistance( SimBlock toward )
	{
		return Mathf.Abs (m_i - toward.m_i) + Mathf.Abs( m_j - toward.m_j);
	}
}