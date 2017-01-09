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

	static public Block GetRandomBlock()
	{
		if ( Instance != null )
			return Instance.blockGrid[Random.Range(0,Instance.gridWidth),Random.Range(0,Instance.gridHeight)].BlockInfo;
		return null;
	}

	static public Block[] GetEnermyBlock( TeamColor myTeamColor )
	{
		List<Block> list = new List<Block> ();

		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if (Instance.blockGrid [i, j].BlockInfo.state == Block.BlockState.Hero) {
					if (Instance.blockGrid [i, j].GetHeroInfo ().teamColor != myTeamColor) {
						list.Add (Instance.blockGrid [i, j].BlockInfo);
					}
				}
			}

		return list.ToArray();
	}

	static public void ShowWalkable( Block centerBlock , int walkRange )
	{
		ResetEmpty ( null );
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if (Instance.blockGrid [i, j].BlockInfo.state == Block.BlockState.Empty) {
					if ( centerBlock.GetDistance( Instance.blockGrid[i,j].BlockInfo) <= walkRange )
					{
						Instance.blockGrid [i, j].BackgroundSetColor (new Color (1f, 1f, 0.6f));
					}
				}
			}
	}

	static public void ResetEmpty( Block centerBlock  )
	{
		for (int i = 0; i < Instance.gridWidth; ++i)
			for (int j = 0; j < Instance.gridHeight; ++j) {
				if (Instance.blockGrid [i, j].BlockInfo.state == Block.BlockState.Empty) {
					Instance.blockGrid [i, j].BackgroundSetColor (Color.white);
				}
			}
	}
}
