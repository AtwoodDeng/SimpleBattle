using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	public Hero parent;

	void Awake()
	{
		if (parent == null)
			parent = GetComponent<Hero>();
		if (parent == null && transform.parent != null)
			parent = transform.parent.GetComponent<Hero> ();
	}

	public virtual float DoAttack()
	{
		return 1f;
	}

	public virtual void ShowAttackRange( Block center , Direction direction )
	{
		
	}

	public virtual Damage GetDamage( HeroInfo myInfo )
	{
		return new Damage (myInfo.attack, myInfo);
	}

	public static void SendDamage( Damage dmg, Block block )
	{
		if (block.linkedBlock != null && block.linkedBlock.GetHeroInfo () != null && block.state == Block.BlockState.Hero) {
			block.linkedBlock.GetHeroInfo ().RecieveDamage (dmg);
		}
	}
}
