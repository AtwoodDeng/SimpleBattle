using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleEnermyHero : Hero {
	[SerializeField] int row;
	[SerializeField] int column;

//	int m_healthShow;
//	int healthShow{
//		get {
//			return m_healthShow;
//		}
//		set {
//			m_healthShow = value;
//			healthText.text = m_healthShow.ToString ();
//		}
//	}
//	[SerializeField]TextMesh healthText;
//	[SerializeField]TextMesh dmgText;

	protected override void MStart ()
	{
		base.MStart ();

		Block toward = BattleField.GetBlock (row, column);
		if (toward.state == Block.BlockState.Empty) {
			toward.linkedBlock.RegisterHero (this);
			TemBlock = toward;
			transform.position = toward.linkedBlock.GetCenterPosition ();
		}

		LogicManager.Instance.RegisterHero(this);


	}


}
