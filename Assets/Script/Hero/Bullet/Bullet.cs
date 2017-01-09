using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour {

	TeamColor m_teamColor;
	[SerializeField] SpriteRenderer render;
	[SerializeField] float velocity = 5f;

	public virtual float Shoot( Vector3 senderPos , Vector3 targetPos , TeamColor color , TweenCallback onHit , float delay = 0, Ease easeType = Ease.Linear  )
	{
		m_teamColor = color;
		transform.position = senderPos;
		float duration = (targetPos - senderPos).magnitude / velocity;
		if (render != null)
			render.enabled = true;
		transform.DOMove (targetPos, duration ).SetDelay(delay).SetEase(easeType).OnComplete (onHit);

		return duration + delay;
	}

	public void OnHit()
	{
		render.enabled = false;
	}


}
