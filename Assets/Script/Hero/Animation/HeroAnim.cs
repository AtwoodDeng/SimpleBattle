using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim : HeroComponent {

	/// <summary>
	/// Play the move animation
	/// </summary>
	/// <returns> the animation time</returns>
	/// <param name="block">Block.</param>
	public virtual float MoveTo( SimBlock block )
	{
		return 0;
	}
	/// <summary>
	/// Play attack animation 
	/// </summary>
	/// <returns>The animation time</returns>
	/// <param name="blocks"> the target blocks</param>
	/// 
	public virtual float Attack( Damage[] damges )
	{
		return 0;
	}

	public virtual void EndAttack()
	{
		
	}

	public virtual void AttackPassive()
	{
	}

	public virtual void BeginBattlePassive()
	{
		Debug.Log("Play BeginBattle Passive Animation");	
	}

	public virtual void EndBattlePassive()
	{
		
	}

	public virtual void Passive()
	{
		
	}

	public virtual void RecieveDamage()
	{
		
	}
}
