using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class BattleField : MonoBehaviour {

	static public BattleField Instance{ get { return m_Instance; } }
	static public BattleField m_Instance;
	public BattleField(){ m_Instance = this; }


	[SerializeField] public int gridWidth = 1;
	[SerializeField] public int gridHeight = 1;
	public static int Width{
		get { return Instance.gridWidth; }
	}
	public static int Height{
		get { return Instance.gridHeight; }
	}
	[SerializeField] GameObject blockPrefab;

	BattleBlock[,] blockGrid;

	public delegate void BlockAction( int i , int j , BattleBlock block );

	public event BlockAction SelectEvent;

	public List<SimBlock> placableBlock = new List<SimBlock>();

//	public bool GenerateBattleField = false;
//	public bool UpdatePlacable = false;

	// Use this for initialization
	void Awake () {
//		if ( blockGrid == null )
			InitBattleBlock ();
	}

	void InitBattleBlock()
	{
		if (blockGrid != null && blockGrid.Length > 0) {
			for (int i = 0; i < blockGrid.GetLength(0); ++i)
				for (int j = 0; j < blockGrid.GetLength(1); ++j) {
					DestroyImmediate (blockGrid [i, j].gameObject);
				}
		}
		
		blockGrid = new BattleBlock[gridWidth,gridHeight];

		for (int i = 0; i < blockGrid.GetLength(0); ++i)
			for (int j = 0; j < blockGrid.GetLength(1); ++j) {
				GameObject block = Instantiate (blockPrefab);
				block.SetActive (true);

				blockGrid [i, j] = block.GetComponent<BattleBlock>();
				blockGrid [i, j].Init (i, j, this);
			}

		foreach (SimBlock b in placableBlock) {
			blockGrid [b.m_i, b.m_j].BlockInfo.isPlacable = true;
		}
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
				blockGrid [i, j].IsSelected = false;
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
		}
	}

	static public Block GetBlock( int i , int j )
	{
		if (Instance != null) {
			if (i >= 0 && i < Instance.gridWidth && j >= 0 && j < Instance.gridHeight)
				return Instance.blockGrid [i, j].BlockInfo;
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
			return Instance.blockGrid[Random.Range(0,Instance.gridWidth),Random.Range(0,Instance.gridHeight)].BlockInfo;
		return null;
	}

	// TODO : remove this function
	static public Block[] GetEnermyBlock( TeamColor myTeamColor )
	{
		List<Block> list = new List<Block> ();

		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if (Instance.blockGrid [i, j].BlockInfo.state == Block.BlockState.Hero) {
					if (Instance.blockGrid [i, j].GetHeroInfo ().TeamColor != myTeamColor) {
						list.Add (Instance.blockGrid [i, j].BlockInfo);
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
				if (Instance.blockGrid [i, j].BlockInfo.state == Block.BlockState.Hero) {
					if (Instance.blockGrid [i, j].GetHeroInfo ().TeamColor != myTeamColor) {
						list.Add (Instance.blockGrid [i, j].BlockInfo.SimpleBlock);
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
		if ( isReset )
			ResetVisualColor( true );
		
		foreach( SimBlock sb in blockList )
		{
			if ( IsBlockInRange( sb ))
				Instance.blockGrid[sb.m_i,sb.m_j].visualType = type;
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
				if ( Instance.blockGrid[i,j].BlockInfo.state == Block.BlockState.Empty || withHero)
					Instance.blockGrid [i, j].visualType = BattleBlock.BlockVisualType.Normal;
			}
	}

	static public void ResetLock()
	{
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				Instance.blockGrid[i,j].BlockInfo.isLock = (Instance.blockGrid[i,j].BlockInfo.state == Block.BlockState.Hero);
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
}
