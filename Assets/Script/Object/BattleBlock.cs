using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class BattleBlock : MonoBehaviour {

	Block.BlockState state{
		get {
			return m_block.state;
		}
		set {
			m_block.state = value;
		}
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

	[SerializeField] float width;
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
		transform.localPosition = new Vector3 ((i - (p.gridWidth - 1) * 0.5f ) * width,
			(j - (p.gridHeight - 1) * 0.5f  ) * width, 0);

	}

	public Vector3 GetCenterPosition()
	{
		return transform.parent.position + new Vector3 ((m_block.m_i - (parent.gridWidth - 1) * 0.5f ) * width,
			(m_block.m_j - (parent.gridHeight - 1) * 0.5f  ) * width, 0);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube (transform.position, Vector3.one * width);
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
			background.DOColor ( Color.white , 0.2f);
		} else {
			state = Block.BlockState.Hero;
			background.DOColor ( Color.gray , 0.2f);
		}
	}

	public void BackgroundShin( Color col ){
		background.DOKill ();
		background.DOColor (col, 0.5f).SetLoops (9999, LoopType.Yoyo);

	}

	public void BackgroundSetColor( Color col ){
		background.DOKill ();
		background.DOColor (col,0.2f);
	}

	public void BackgroundReset(){
		background.DOKill ();
		background.DOColor (Color.white,0);
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
		Locked,
		Object,
	}

	public bool isPlacable;
	public BlockState state;
	public bool IsSelected;
	public BattleBlock linkedBlock;

	public int m_i,m_j;

	public Block( int i ,int j , BattleBlock lb )
	{
		m_i = i;
		m_j = j;
		linkedBlock = lb;
		state = BlockState.Empty;
		isPlacable = false;
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

}