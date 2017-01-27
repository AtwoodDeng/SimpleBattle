using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleAttack : Attack {

	[SerializeField] GameObject bulletPrefab;

	public override float DoAttack ()
	{
		// find the nearest enermy

		Block[] enermyBlocks = BattleField.GetEnermyBlock(parent.GetHeroInfo().TeamColor);

		int distance = 99;
		Block myBlock = parent.TemBlock;
		Block targetBlock = null;
		foreach (Block b in enermyBlocks) {
			int d = myBlock.GetDistance (b);
			if ( d < distance && parent.IsInAttackRange( b ) )
			{
				distance = d;
				targetBlock = b;
			}
		}

		if (targetBlock != null) {
			// attack the enermy
			GameObject bullet = Instantiate (bulletPrefab);

			Bullet bulletCom = bullet.GetComponent<Bullet> ();
			float duration = bulletCom.Shoot (myBlock.linkedBlock.GetCenterPosition (), targetBlock.linkedBlock.GetCenterPosition (), parent.GetHeroInfo ().TeamColor, delegate {
				SendDamage( GetDamage(parent.GetHeroInfo() , targetBlock.SimpleBlock) , targetBlock);
				bulletCom.OnHit ();
			});
			return duration;
		} else {
//			parent.UnableToAttack ();
			return 0;
		}

	}
}
