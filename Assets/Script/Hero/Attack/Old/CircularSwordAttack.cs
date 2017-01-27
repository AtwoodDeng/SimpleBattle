using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircularSwordAttack : Attack {
	[SerializeField] GameObject swordPrefab;
	GameObject sword;
	[SerializeField] float duration = 1f;

	public override float DoAttack ()
	{
		Block[] enermyBlocks = BattleField.GetEnermyBlock(parent.GetHeroInfo().TeamColor);

		if (sword == null) {
			sword = Instantiate (swordPrefab) as GameObject;
			sword.transform.parent = transform;
			sword.transform.localPosition = Vector3.zero;
		}
		sword.SetActive (true);
		Sequence seq = DOTween.Sequence ();
		seq.Append (sword.transform.DOScale (0,0));
		seq.Append (sword.transform.DOScale (0.65f / parent.transform.lossyScale.x,0.1f * duration));
		seq.Append (sword.transform.DORotate (new Vector3 (0, 0, 360f), duration).SetEase (Ease.InOutQuart).SetRelative (true));
		seq.Append (sword.transform.DOScale (0, 0.1f * duration));
		seq.AppendCallback( delegate {
			foreach( Block b in enermyBlocks )
			{
				if ( parent.IsInAttackRange( b ))
				{
					SendDamage( GetDamage(parent.GetHeroInfo() , b.SimpleBlock ) , b );
				}
			}
			sword.SetActive(false);
		});

		return duration;

	}


}
