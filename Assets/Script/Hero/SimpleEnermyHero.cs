using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleEnermyHero : Hero {
	[SerializeField] int row;
	[SerializeField] int column;

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
	[SerializeField]TextMesh healthText;
	[SerializeField]TextMesh dmgText;

	void Start()
	{
		Block toward = BattleField.GetBlock (row, column);
		if (toward.state == Block.BlockState.Empty) {
			toward.linkedBlock.RegisterHero (this);
			TemBlock = toward;
			transform.position = toward.linkedBlock.GetCenterPosition ();
		}


		healthShow = (int)GetHeroInfo ().health;
		GetHeroInfo().HealthChangeFunc += delegate(float fromHealth, float toHealth) {
			DOTween.To( () => healthShow , (x) => healthShow  = x , (int)toHealth , 1f );
			transform.DOShakePosition( 0.5f , 0.2f );


			// show damage
			float dmg = fromHealth - toHealth;
			dmgText.text = dmg.ToString();
			dmgText.transform.position = healthText.transform.position;
			dmgText.transform.DOMoveY( 0.5f , 2f ).SetRelative( true );
			dmgText.color = Color.red;
			DOTween.To( () => dmgText.color , (x) => dmgText.color = x , new Color( 1f , 0.5f , 0 , 0 ) , 1f ).SetDelay(2f);
		};


		GetHeroInfo().DeathFunc += delegate() {
			healthText.gameObject.SetActive(false);
			dmgText.gameObject.SetActive(false);
			TemBlock = null;
			GetComponent<SpriteRenderer>().DOColor( Color.red , 1f ).OnComplete( delegate() {
				gameObject.SetActive(false);	
			});
		};
	}


}
