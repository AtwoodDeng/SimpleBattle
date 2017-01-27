using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCreator : MBehavior {

	[SerializeField] SpriteRenderer HeroIcon;

	HeroType TemHeroType
	{
		get {
			return m_type;
		}
		set {

			if ( HeroIcon != null )
				HeroIcon.sprite = GetSpriteByType(value);
			m_type = value;

		}
	}
	HeroType m_type;

	protected override void MStart ()
	{
		base.MStart ();

		TemHeroType = HeroType.Arrow;
	}

	Sprite GetSpriteByType( HeroType type )
	{
//		Debug.Log("Path " + "Img/Icon/" + type.ToString() + "Icon" );
		Sprite res = Resources.Load<Sprite>("Img/Icon/" + type.ToString() + "Icon");
//		Debug.Log("REs "  + res );
		return res;
	}

	public void OnLeft()
	{
		int Length = System.Enum.GetNames(typeof(HeroType)).Length;
		TemHeroType = (HeroType) ( (int)(TemHeroType - 1 + Length ) % Length);
	}

	public void OnRight()
	{
		int Length = System.Enum.GetNames(typeof(HeroType)).Length;
		TemHeroType = (HeroType) ( (int)(TemHeroType + 1 + Length) % Length );
	}

	public void OnCreate()
	{
		SimpleHero hero = HeroFactory.SetupPlacableHero( TemHeroType );
		hero.transform.position = HeroIcon.transform.position;

		hero.SelectAndDraw();
	}

	public void OnFinishCreate()
	{
		
	}
}
