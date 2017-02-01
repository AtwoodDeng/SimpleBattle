using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFactory {

	public static SimpleHero SetupPlacableHero( HeroType type  )
	{
		GameObject heroObj = CreateHeroByType( type );
		SimpleHero hero = heroObj.AddComponent<SimpleHero>();

		hero.Init();
		hero.GetHeroInfo().direction = Direction.Right;
		hero.GetHeroInfo().TeamColor = TeamColor.Blue;

		return hero;
	}

	public static NetworkHero SetUpEnemyHero(RawHeroInfo rInfo )
	{
		GameObject heroObj = CreateHeroByType( rInfo.type );
		heroObj.name += "Net";
		NetworkHero hero = heroObj.AddComponent<NetworkHero>();

		hero.Init( rInfo );
		hero.GetHeroInfo().direction = rInfo.direction;
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
}
