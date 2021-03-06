﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class BattleBlock : MonoBehaviour {

	public enum BlockVisualType
	{
		Normal,
		// showing the chosen hero in the strategy phase
		StrategyChosenHero,
		// showing move range on this block
		StrategyMoveRange,
		// showing attack range on this block
		StrategyAttackRange,
		// focuse on a hero, then the hero's block will change to this 
		StrategyFocus,
		// confirm a hero's move, then the hero's block will change to this 
		StrategyConfirm,
		// in battle, when attack, shows the attack hero
		BattleThisHero,
		// in battle, when attack, shows the attack range
		BattleAttackRange,
		// in battle, when attack, shows the attack range
		BattleAttackTarget,
		// Move Target
		BattleMoveTarget,
		// Move Target block
		BattleMoveTargetBlocked,
		// EnermyMoveRange,
		MoveRangeEnermy,
		// attack range of enermy
		AttackRangeEnermy,

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
				case BlockVisualType.StrategyAttackRange:
					BackgroundSetColor(colorSetting.AttackColor);
					break;
				case BlockVisualType.StrategyMoveRange:
					BackgroundSetColor(colorSetting.MoveRangeColor);
					break;
				case BlockVisualType.BattleMoveTarget:
					BackgroundSetColor(colorSetting.BattleMoveTarget);
					break;
				case BlockVisualType.BattleMoveTargetBlocked:
					BackgroundSetColor(colorSetting.BattleMoveTargetBlocked);
					break;
				case BlockVisualType.BattleAttackRange:
					BackgroundSetColor(colorSetting.BattleAttackRange);
					break;
				case BlockVisualType.MoveRangeEnermy:
					BackgroundSetColor(colorSetting.BattleMoveRangeEnermy);
					break;
				case BlockVisualType.AttackRangeEnermy:
					BackgroundSetColor(colorSetting.BattleAttackRangeEnermy);
					break;
				default:
					break;
				}
			}
			else if (  m_block.state == Block.BlockState.Hero )
			{
				switch( value )
				{

				case BlockVisualType.Normal:
					RefreshColorByBlock();
					break;
				case BlockVisualType.StrategyChosenHero:
					BackgroundSetColor( colorSetting.ChosenHeroColor );
					break;
				case BlockVisualType.StrategyAttackRange:
					BackgroundSetColor(colorSetting.AttackColor);
					break;
				case BlockVisualType.StrategyMoveRange:
					BackgroundSetColor(colorSetting.MoveRangeColor);
					break;
				case BlockVisualType.StrategyFocus:
					BackgroundSetColor(colorSetting.StrategyFocusColor);
					break;
				case BlockVisualType.StrategyConfirm:
					BackgroundSetColor(colorSetting.StrategyConfirmColor);
					break;
				case BlockVisualType.BattleMoveTarget:
					BackgroundSetColor(colorSetting.BattleMoveTarget);
					break;
				case BlockVisualType.BattleMoveTargetBlocked:
					
					BackgroundSetColor(colorSetting.BattleMoveTargetBlocked);
					break;
				case BlockVisualType.BattleThisHero:
					BackgroundSetColor(colorSetting.BattleThisHero);
					break;
				case BlockVisualType.BattleAttackRange:
					BackgroundSetColor(colorSetting.BattleAttackRange);
					break;
				case BlockVisualType.BattleAttackTarget:
					BackgroundSetColor(colorSetting.BattleAttackRangeHero);
					break;
				case BlockVisualType.MoveRangeEnermy:
					BackgroundSetColor(colorSetting.BattleMoveRangeEnermy);
					break;
				case BlockVisualType.AttackRangeEnermy:
					BackgroundSetColor(colorSetting.BattleAttackRangeEnermy);
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
		public Color ChosenHeroColor;
		public Color MoveRangeColor;
		public Color AttackColor;
		public Color StrategyFocusColor;
		public Color StrategyConfirmColor;
		public Color BattleMoveTarget;
		public Color BattleMoveTargetBlocked;
		public Color BattleThisHero;
		public Color BattleAttackRange;
		public Color BattleAttackRangeHero;
		public Color BattleMoveRangeEnermy;
		public Color BattleAttackRangeEnermy;

	};
	[SerializeField] BlockColorSetting colorSetting;

	public bool IsPlayAnimation{
		get {
			return !IsVirtual;
		}
	}

	public Block.BlockState State{
		get { return state; }
	}
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
//		Debug.Log ("RefreshColorByBlock");
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

	public static Vector3 virtualOffset = new Vector3( 0 , 20f , 0);
	private bool m_isVirtual;
	public bool IsVirtual
	{
		get { return m_isVirtual; }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public HeroInfo GetHeroInfo()
	{
		if (m_hero == null)
			return null;
		return m_hero.GetHeroInfo ();
	}

	public void Init( int i , int j , BattleField p , bool isVirtual = false)
	{

		m_block = new Block (i, j, this);
		m_hero = null;
		parent = p;

		transform.parent = p.transform;
		Vector3 offset = Vector3.zero;
		name = "block " + i + " " + j;
		m_isVirtual = isVirtual;
		if (isVirtual) {
			offset = virtualOffset;
			name = "virtual block " + i + " " + j;
		}
		
		transform.localPosition = new Vector3 ((i - (p.gridWidth - 1) * 0.5f ) * Width,
			(j - (p.gridHeight - 1) * 0.5f  ) * Height, 0) + offset;

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
//			background.DOColor (new Color (0.5f, 0.5f, 1f), 0.2f);
			BackgroundSetColor( new Color( 0.5f , 0.5f , 1f ));
		}
	}

	public void ExitHero( Hero h )
	{
		if (state == Block.BlockState.Empty ) {

//			background.DOColor ( Color.white , 0.2f);
			BackgroundSetColor( Color.white );
		}
	}

	public void RegisterHero( Hero h )
	{
		m_hero = h;
		if (h == null) {
			state = Block.BlockState.Empty;

		} else {
			state = Block.BlockState.Hero;
		}
	}

	public void BackgroundShine( Color col , float alpha = 1f ){
		if (IsPlayAnimation) {
			background.DOKill ();
			col.a = alpha;
			background.DOColor (col, 0.5f).SetLoops (9999, LoopType.Yoyo);
		}
	}

	private void BackgroundSetColor( Color col , float alpha = 1f ){
		if (IsPlayAnimation) {
			background.DOKill ();
			col.a = alpha;
			background.DOColor (col, 0.2f);
		}
	}

	private void BackgroundReset(){
		if (IsPlayAnimation) {
			background.DOKill ();
			background.DOColor (Color.white, 0);
		}
	}

	public void ResetVisual()
	{
		visualType = BlockVisualType.Normal;
		RefreshColorByBlock();
	}

	public void ResetAll()
	{
		ResetVisual ();
		m_block = new Block (m_block.m_i, m_block.m_j, this);
		m_hero = null;
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

	public override string ToString ()
	{
		return string.Format ("[SimBlock]{0},{1}" , m_i , m_j);
	}
}