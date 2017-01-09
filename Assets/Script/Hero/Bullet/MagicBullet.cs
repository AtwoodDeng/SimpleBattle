using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : Bullet {

	public void Shoot( Vector3 pos )
	{
		
		transform.position = pos;
	}
}
