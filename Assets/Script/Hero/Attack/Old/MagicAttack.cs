using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttack : Attack {
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float interval = 0.2f;

	public override float DoAttack ()
	{
		int i = parent.TemBlock.m_i;
		int j = parent.TemBlock.m_j;

		count = 0;
		float duration = 0;
		switch (parent.GetHeroInfo ().Direction) {
		case Direction.Up:
			for (int ii = i - 1; ii <= i + 1; ++ii) {
				for (int jj = j + 1; jj < BattleField.Height; ++jj) {
					AttackBlock (BattleField.GetBlock (ii, jj));
					duration += interval;
				}
			}
			break;
		case Direction.Down:
			for (int ii = i - 1; ii <= i + 1; ++ii) {
				for (int jj = j - 1; jj >= 0; --jj) {
					AttackBlock (BattleField.GetBlock (ii, jj));
					duration += interval;
				}
			}
			break;
		case Direction.Left:
			for (int ii = i - 1; ii >= 0; --ii) {
				for (int jj = j - 1; jj <= j + 1 ; ++jj) {
					AttackBlock (BattleField.GetBlock (ii, jj));
					duration += interval;
				}
			}
			break;

		case Direction.Right:
			for (int ii = i + 1; ii < BattleField.Width; ++ii) {
				for (int jj = j - 1; jj <= j + 1 ; ++jj) {
					AttackBlock (BattleField.GetBlock (ii, jj));
					duration += interval;
				}
			}
			break;
		default:
			break;
		};
		return duration;
	}

	int count = 0;
	void AttackBlock( Block block )
	{
		if (block == null)
			return;
		count++;
		StartCoroutine( Attack (block , count * interval));
	}

	IEnumerator Attack( Block block , float delay)
	{
		yield return new WaitForSeconds( delay );

		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		MagicBullet bulletCom = bullet.GetComponent<MagicBullet> ();
		bulletCom.Shoot (block.GetCenterPosition() );
		if (block.state == Block.BlockState.Hero) {
			SendDamage (GetDamage (parent.GetHeroInfo () , block.SimpleBlock) , block);
		}

	}
}
