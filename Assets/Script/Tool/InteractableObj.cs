using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObj : MBehavior {

	public virtual void Select(){}
	public virtual void Confirm(){}
//	public virtual void OnFingerMove( Vector2 delta ){}

	void OnFingerDown( FingerDownEvent e )
	{
		Select ();
	}

	void OnFingerUp( FingerUpEvent e )
	{
		Confirm ();
	}
//	void OnFingerMove( FingerMotionEvent e )
//	{
//		OnFingerMove( e.Finger.DeltaPosition );
//	}
}
