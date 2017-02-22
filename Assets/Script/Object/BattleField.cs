using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class BattleField : MonoBehaviour {

	static public BattleField Instance {
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<BattleField> ();
			return m_Instance; }
		set {
			if (m_Instance == null)
				m_Instance = value;
		}
	}
	static public BattleField m_Instance;


	[SerializeField] public int gridWidth = 1;
	[SerializeField] public int gridHeight = 1;
	public static int Width{
		get { return Instance.gridWidth; }
	}
	public static int Height{
		get { return Instance.gridHeight; }
	}
	[SerializeField] GameObject blockPrefab;

	BattleBlock[,] BlockGrid{
		get {
			if (m_battleFieldState == BattleFieldState.Normal)
				return m_normalBlockGrid;
			else
				return m_virtualBlockGrid;
		}
		set {
			if (m_battleFieldState == BattleFieldState.Normal)
				m_normalBlockGrid = value;
			else
				m_virtualBlockGrid = value;
			
		}
	}
	BattleBlock[,] m_normalBlockGrid;
	BattleBlock[,] m_virtualBlockGrid;

	public delegate void BlockAction( int i , int j , BattleBlock block );

	public event BlockAction SelectEvent;

	public List<SimBlock> placableBlock = new List<SimBlock>();

	public enum BattleFieldState
	{
		Normal,
		Virtual,
	}
	BattleFieldState m_battleFieldState;


//	public bool GenerateBattleField = false;
//	public bool UpdatePlacable = false;

	// Use this for initialization
	void Awake () {
		Instance = this;
		m_battleFieldState = BattleFieldState.Normal;
//		if ( blockGrid == null )

		InitBattleBlock ();
		InitVirtualBlock ();
	}

	void InitBattleBlock()
	{
		if (BlockGrid != null && BlockGrid.Length > 0) {
			for (int i = 0; i < BlockGrid.GetLength(0); ++i)
				for (int j = 0; j < BlockGrid.GetLength(1); ++j) {
					DestroyImmediate (BlockGrid [i, j].gameObject);
				}
		}
		
		BlockGrid = new BattleBlock[gridWidth,gridHeight];

		for (int i = 0; i < BlockGrid.GetLength(0); ++i)
			for (int j = 0; j < BlockGrid.GetLength(1); ++j) {
				GameObject block = Instantiate (blockPrefab);
				block.SetActive (true);

				BlockGrid [i, j] = block.GetComponent<BattleBlock>();
				BlockGrid [i, j].Init (i, j, this, m_battleFieldState == BattleFieldState.Virtual);
			}

		foreach (SimBlock b in placableBlock) {
			BlockGrid [b.m_i, b.m_j].BlockInfo.isPlacable = true;
		}
	}

	void InitVirtualBlock()
	{
		m_battleFieldState = BattleFieldState.Virtual;
		InitBattleBlock ();
		m_battleFieldState = BattleFieldState.Normal;
	}


	
	// Update is called once per frame
	void Update () {
//		if (GenerateBattleField) {
//			InitBattleBlock ();
//			GenerateBattleField = false;
//		}
	}

	/// <summary>
	/// Finger down, send select block
	/// </summary>
	/// <param name="block">Block.</param>
	public void OnBlockSelect( Block block )
	{

		for (int i = 0; i < gridWidth; ++i)
			for (int j = 0; j < gridHeight; ++j) {
				BlockGrid [i, j].IsSelected = false;
			}

		block.IsSelected = true;

		LogicArg arg = new LogicArg (this);
		arg.AddMessage (M_Event.BLOCK, block);

		if (block.state == Block.BlockState.Empty) {
			M_Event.FireLogicEvent (LogicEvents.SelectBlock, arg);
		} else if (block.state == Block.BlockState.Hero) {
//			M_Event.FireLogicEvent (LogicEvents.SelectHero, arg);
		}
	}

	/// <summary>
	/// Finger up, send confirmHero
	/// </summary>
	/// <param name="block">Block.</param>
	public void OnBlockConfirm( Block block )
	{
		LogicArg arg = new LogicArg (this);
		arg.AddMessage (M_Event.BLOCK, block);

		if (block.state == Block.BlockState.Empty) {
//			M_Event.FireLogicEvent (LogicEvents.SelectBlock, arg);
		} else if (block.state == Block.BlockState.Hero) {
			M_Event.FireLogicEvent (LogicEvents.ConfirmHero, arg);
			Debug.Log("Confirm Hero Fire " + block.SimpleBlock);
		}
	}

	static public Block GetBlock( int i , int j )
	{
		if (Instance != null) {
			if (i >= 0 && i < Instance.gridWidth && j >= 0 && j < Instance.gridHeight)
				return Instance.BlockGrid [i, j].BlockInfo;
			else
				return null;
		}
		return null;
	}

	static public Block GetBlock( SimBlock sb )
	{
		return GetBlock( sb.m_i , sb.m_j );
	}

	static public Block GetRandomBlock()
	{
		if ( Instance != null )
			return Instance.BlockGrid[Random.Range(0,Instance.gridWidth),Random.Range(0,Instance.gridHeight)].BlockInfo;
		return null;
	}

	// TODO : remove this function
	static public Block[] GetEnermyBlock( TeamColor myTeamColor )
	{
		List<Block> list = new List<Block> ();

		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if (Instance.BlockGrid [i, j].BlockInfo.state == Block.BlockState.Hero) {
					if (Instance.BlockGrid [i, j].GetHeroInfo ().TeamColor != myTeamColor) {
						list.Add (Instance.BlockGrid [i, j].BlockInfo);
					}
				}
			}

		return list.ToArray();
	}


	static public SimBlock[] GetEnermyBlockSim( TeamColor myTeamColor )
	{
		List<SimBlock> list = new List<SimBlock> ();

		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if (Instance.BlockGrid [i, j].BlockInfo.state == Block.BlockState.Hero) {
					if (Instance.BlockGrid [i, j].GetHeroInfo ().TeamColor != myTeamColor) {
						list.Add (Instance.BlockGrid [i, j].BlockInfo.SimpleBlock);
					}
				}
			}

		return list.ToArray();
	}

	static bool IsBlockInRange( SimBlock b )
	{
		return (b.m_i >= 0 ) && (b.m_j >= 0 ) && (b.m_i < Instance.gridWidth ) && (b.m_j < Instance.gridHeight);
	}

	static public void ShowBlock( SimBlock[] blockList , BattleBlock.BlockVisualType type , bool isReset = true )
	{
//		Debug.Log ("Set to " + type);
		if ( isReset )
			ResetVisualColor( true );
		
		foreach( SimBlock sb in blockList )
		{
			if ( IsBlockInRange( sb ))
				Instance.BlockGrid[sb.m_i,sb.m_j].visualType = type;
		}
	}

//	static public void ShowWalkable( Block centerBlock , int walkRange )
//	{
//		ResetEmpty ( null );
//		for (int i = 0; i < Instance.gridWidth; ++i)
//			for (int j = 0; j < Instance.gridHeight; ++j) {
//				if (Instance.blockGrid [i, j].BlockInfo.state == Block.BlockState.Empty) {
//					if ( centerBlock.GetDistance( Instance.blockGrid[i,j].BlockInfo) <= walkRange )
//					{
//						Instance.blockGrid [i, j].BackgroundSetColor (new Color (1f, 1f, 0.6f));
//					}
//				}
//			}
//	}

	static public void ResetVisualColor( bool withHero )
	{
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if ( Instance.BlockGrid[i,j].BlockInfo.state == Block.BlockState.Empty || withHero)
					Instance.BlockGrid [i, j].visualType = BattleBlock.BlockVisualType.Normal;
			}
	}

	static public void ResetLock()
	{
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				Instance.BlockGrid[i,j].BlockInfo.isLock = (Instance.BlockGrid[i,j].BlockInfo.state == Block.BlockState.Hero);
			}
	}

//	static public void ResetEmpty( Block centerBlock  )
//	{
//		for (int i = 0; i < Instance.gridWidth; ++i)
//			for (int j = 0; j < Instance.gridHeight; ++j) {
//				if (Instance.blockGrid [i, j].BlockInfo.state == Block.BlockState.Empty) {
//					Instance.blockGrid [i, j].BackgroundSetColor (Color.white);
//				}
//			}
//	}

	/// <summary>
	/// Rotates the blocks and remove the illegal block
	/// </summary>
	/// <returns>The blocks.</returns>
	/// <param name="blocks">Blocks.</param>
	/// <param name="center">Center.</param>
	/// <param name="direction">Direction.</param>
	static public SimBlock[] RotateBlocks( SimBlock[] blocks , SimBlock center, Direction direction )
	{
		List<SimBlock> res = new List<SimBlock>();
		int ci = center.m_i;
		int cj = center.m_j;
		for( int i = 0 ; i < blocks.Length ; ++ i )
		{
			// the relative coordinate of the block
			int ii = blocks[i].m_i - ci;
			int jj = blocks[i].m_j - cj;
			switch( direction )
			{
			case Direction.Right:
				{
				SimBlock b = new SimBlock( ci + ii , cj + jj );
				if ( IsBlockInRange( b ))
					res.Add( b );
				break;
				}
			case Direction.Left:
				{
				SimBlock b = new SimBlock( ci - ii , cj - jj );
				if ( IsBlockInRange( b ))
					res.Add( b );
				break;
				}
			case Direction.Up:
				{
				SimBlock b = new SimBlock( ci - jj , cj + ii );
				if ( IsBlockInRange( b ))
					res.Add( b );
				break;
				}
			case Direction.Down:
				{
				SimBlock b = new SimBlock( ci + jj , cj - ii );
				if ( IsBlockInRange( b ))
					res.Add( b );
				break;
				}
			default:
				break;

			};
		}
		return res.ToArray();
	}

	public static SimBlock GetReflectBlock( SimBlock block )
	{
		int width = Instance.gridWidth;
		return new SimBlock( width - 1 - block.m_i , block.m_j );
	}

	public static Direction GetReflectDirection( Direction d )
	{
		switch( d ){
		case Direction.Down:
			return Direction.Down;
		case Direction.Up:
			return Direction.Up;
		case Direction.Left:
			return Direction.Right;
		case Direction.Right:
			return Direction.Left;
		default:
			break;
		};
		return Direction.Up;
	}


	public static Dictionary<Hero,Hero> virtualHeroMap = new Dictionary<Hero, Hero>();
	public static List<Hero> virtualHeroList = new List<Hero> ();

	public static void SetUpVirtualHeros()
	{
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if ( Instance.m_normalBlockGrid [i, j].State == Block.BlockState.Hero ) {
					HeroType type = Instance.m_normalBlockGrid [i, j].GetHeroInfo ().type;
					Hero hero = HeroFactory.SetupVirtualHero (type);
					virtualHeroList.Add (hero);
					virtualHeroMap [Instance.m_normalBlockGrid [i, j].GetHeroInfo ().parent] = hero;
					Instance.m_virtualBlockGrid [i, j].RegisterHero (hero);
				}
			}
		
	}

	public static void CopyFromRealToVirtual()
	{
		if (virtualHeroList.Count <= 0)
			SetUpVirtualHeros ();
		
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				Instance.m_virtualBlockGrid [i, j].ResetAll ();
			}
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if ( Instance.m_normalBlockGrid [i, j].State == Block.BlockState.Hero ) {
					Hero normalHero = Instance.m_normalBlockGrid [i, j].GetHeroInfo ().parent;
					Hero virtualHero = virtualHeroMap [normalHero];
					virtualHero.SetBlock (new SimBlock (i, j));
					Instance.m_virtualBlockGrid [i, j].RegisterHero (virtualHero);
				}
			}

		foreach (KeyValuePair<Hero,Hero> kv in virtualHeroMap) {
			kv.Value.GetHeroInfo ().DeepCopy (kv.Key.GetHeroInfo (), virtualHeroMap);
		}
	}

	public static void StartVirtual()
	{
		Instance.m_battleFieldState = BattleFieldState.Virtual;
	}

	public static void EndVirtual()
	{
		Instance.m_battleFieldState = BattleFieldState.Normal;
	}
}
