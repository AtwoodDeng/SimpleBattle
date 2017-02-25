using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualHero : Hero {
	List< SpriteRenderer> m_sprites;
	public override void Init ()
	{
		base.Init ();
		transform.position = new Vector3 (999f, 999f, 999f);
		m_sprites = new List<SpriteRenderer> ();
		m_sprites.Add (gameObject.GetComponent<SpriteRenderer> ());
		m_sprites.AddRange (gameObject.GetComponentsInChildren<SpriteRenderer> ());
		foreach (SpriteRenderer sr in m_sprites)
			sr.enabled = false;
	}
//	public override void SetBlock (SimBlock block)
//	{
//		base.SetBlock (block);
//
//		if ( TemBlock != null )
//			transform.position = TemBlock.GetCenterPosition();
//	}
}
