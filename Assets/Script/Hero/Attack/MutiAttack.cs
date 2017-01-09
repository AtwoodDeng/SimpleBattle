using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutiAttack : Attack {

	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float attackInterval = 0.3f;

	public override float DoAttack ()
	{
		// find the nearest enermy

		Block[] enermyBlocks = BattleField.GetEnermyBlock(parent.GetHeroInfo().teamColor);

		int attackCount = 0;
		float duration = 0;
		Block myBlock = parent.TemBlock;
		foreach (Block targetBlock in enermyBlocks) {
			if ( parent.IsInAttackRange( targetBlock) ) {
				attackCount++;
				if (targetBlock != null) {
					// attack the enermy
					GameObject bullet = Instantiate (bulletPrefab);

					Bullet bulletCom = bullet.GetComponent<Bullet> ();
					float time = bulletCom.Shoot (myBlock.linkedBlock.GetCenterPosition (), targetBlock.linkedBlock.GetCenterPosition (), parent.GetHeroInfo ().teamColor, delegate {
						SendDamage( GetDamage(parent.GetHeroInfo()) , targetBlock);
						bulletCom.OnHit ();
					}, attackCount * attackInterval );

					if (time + attackCount * attackInterval > duration) {
						duration = time + attackCount * attackInterval;
					}
				}
			}
		}
		if (attackCount <= 0)
			parent.UnableToAttack();
		return duration;

	}
}
