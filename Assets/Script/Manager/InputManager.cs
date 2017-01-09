using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	static public Vector2 delta
	{
		get {
			return m_delta;
		}
	}
	static public Vector2 FocusWorldPos
	{
		get {
			return Camera.main.ScreenToWorldPoint (m_pos);
		}
	}
	static public Vector3 FocusScreenPos
	{
		get {
			return m_pos;
		}
	}

	static Vector2 m_delta;
	static Vector2 m_pos;

	void OnFingerMove( FingerMotionEvent e )
	{
		m_delta = e.Finger.DeltaPosition;
		m_pos = e.Finger.Position;
	}

	void OnFingerDown( FingerDownEvent e )
	{
		
	}

	void OnFingerUp( FingerUpEvent e )
	{
		M_Event.FireLogicEvent (LogicEvents.FingerUp, new LogicArg (this));
	}
}
