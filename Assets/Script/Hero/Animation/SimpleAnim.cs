using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleAnim : HeroAnim {
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float interval = 0.2f;

	[SerializeField] TextMesh healthText;
	[SerializeField] TextMesh dmgText;
	[SerializeField] SpriteRenderer sprite;

	int m_healthShow;
	int healthShow{
		get {
			return m_healthShow;
		}
		set {
			m_healthShow = value;
			healthText.text = m_healthShow.ToString ();
		}
	}

	public override void Init (Hero hero)
	{
		base.Init (hero);
		TextMesh[] texts = gameObject.GetComponentsInChildren<TextMesh>();
		foreach( TextMesh text in texts )
		{
			if ( text.name.Equals( "healthText" ))
				healthText = text;
			if ( text.name.Equals( "dmgText" ))
				dmgText = text;
		}

		if ( sprite == null )
		{
			sprite = GetComponent<SpriteRenderer>(); 
		}

		parent.GetHeroInfo().TeamColorFunc += delegate(TeamColor fromColor, TeamColor toColor) {
			if ( sprite != null )
			{
				sprite.color = Global.TeamToColor( toColor );
			}
		};

		InitHeroInfo();
	}
		
	void InitHeroInfo()
	{
		healthShow = (int)parent.GetHeroInfo ().Health;

		// on hero hurt
		parent.GetHeroInfo().HealthChangeFunc += delegate(float fromHealth ,float toHealth) {
			DOTween.To( () => healthShow , (x) => healthShow  = x , Mathf.Max( 0 , (int)toHealth) , 1f );
			transform.DOShakePosition( 0.5f , 0.2f );

			// show damage
			float dmg = fromHealth - toHealth;
			dmgText.text = dmg.ToString();
			dmgText.transform.DOKill();
			dmgText.transform.position = healthText.transform.position;
			dmgText.transform.DOMoveY( 0.5f , 2f ).SetRelative( true );
			dmgText.color = Color.red;
			DOTween.To( () => dmgText.color , (x) => dmgText.color = x , new Color( 1f , 0.5f , 0 , 0 ) , 1f ).SetDelay(2f);
		};

		// on hero dead
		parent.GetHeroInfo().DeathFunc += delegate() {
			healthText.gameObject.SetActive(false);
			dmgText.gameObject.SetActive(false);
			parent.TemBlock = null;
			GetComponent<SpriteRenderer>().DOColor( Color.red , 1f ).OnComplete( delegate() {
				gameObject.SetActive(false);	
			});
		};
	}

	public override float MoveTo (SimBlock block)
	{
		float duration = LogicManager.moveDuration;
		Block targetBlock = BattleField.GetBlock( block );
		parent.transform.DOMove (targetBlock.linkedBlock.GetCenterPosition (), duration);
		return duration;
	}

	public override float Attack (Damage[] damges, SimBlock[] targets, SimBlock[] attackRange)
	{
		float duration = interval * damges.Length + 1f;

		StartCoroutine( PlayAttack( damges )); 

		return duration;
	}

	IEnumerator PlayAttack( Damage[] dmgs )
	{
		foreach( Damage d in dmgs )
		{
			Attack( d );
			yield return new WaitForSeconds( interval );
		}
	}

	void Attack( Damage dmg )
	{
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		MagicBullet bulletCom = bullet.GetComponent<MagicBullet> ();
		Block block = BattleField.GetBlock( dmg.target );
		bulletCom.Shoot (block.GetCenterPosition() );
		parent.SendDamage ( dmg );
	}
}
