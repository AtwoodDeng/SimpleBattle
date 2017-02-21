using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BuffFactory  {
	public static Buff CreateBuffFrom( Buff buff , Dictionary<Hero,Hero> heroMap )
	{
		Assert.IsNotNull (buff);
		switch (buff.GetBuffType ()) {
		case BuffType.SoldierBuff:
			{
				SoliderAttackBuff res = new SoliderAttackBuff ();
				res.DeepCopy (buff, heroMap);
				return res;
			}
		case BuffType.FireBuff:
			{
				FireBuff res = new FireBuff (0,null);
				res.DeepCopy (buff, heroMap);
				return res;
			}
		default:
			break;
		};

		Buff fres = new Buff();
		fres.DeepCopy (buff,heroMap);
		return fres;
	}
}
