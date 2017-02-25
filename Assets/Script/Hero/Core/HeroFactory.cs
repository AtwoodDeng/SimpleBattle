using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFactory {

	public static Hero SetupVirtualHero( HeroType type )
	{
		GameObject heroObj = CreateHeroByType( type );
		Hero hero = heroObj.AddComponent<VirtualHero>();
		heroObj.AddComponent<AIVirtualStrategy> ();
		hero.Init();
//		hero.GetHeroInfo ().DeepCopy (info);

		return hero;
	}

	public static SimpleHero SetupPlacableHero( HeroType type  )
	{
		GameObject heroObj = CreateHeroByType( type );
		SimpleHero hero = heroObj.AddComponent<SimpleHero>();
		heroObj.AddComponent<CustomStrategy> ();
		heroObj.AddComponent<SimpleAnim> ();

		hero.Init();
		hero.GetHeroInfo().Direction = Direction.Right;
		hero.GetHeroInfo().TeamColor = TeamColor.Blue;

		return hero;
	}

	public static NetworkHero SetUpEnemyHero(RawHeroInfo rInfo )
	{
		GameObject heroObj = CreateHeroByType( rInfo.type );
		heroObj.name += "Net";
		NetworkHero hero = heroObj.AddComponent<NetworkHero>();
		heroObj.AddComponent<CustomStrategy> ();
		heroObj.AddComponent<SimpleAnim> ();

		hero.Init( rInfo );
		hero.GetHeroInfo().Direction = rInfo.direction;
		hero.GetHeroInfo().TeamColor = TeamColor.Red;

		return hero;
	}

	static GameObject CreateHeroByType( HeroType type )
	{
		string path = "Hero/" + type.ToString() + "Hero";
		GameObject heroPrefab = Resources.Load<GameObject>( path );
		GameObject newHero = GameObject.Instantiate( heroPrefab ) as GameObject;

		return newHero;
	}
}

public enum HeroType
{
	Arrow = 1,
	Mega = 2,
	Soldier = 3,
//	Soul = 4,
	Sword = 5,
	Fire = 6 ,
	Thief = 7,

	NijiTest = 101,
	SwordTest = 102,
	MegaTest = 103,
}
