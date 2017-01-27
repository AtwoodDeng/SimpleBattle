using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global {

	public static Color TeamToColor( TeamColor team ){
		switch( team )
		{
			case TeamColor.Red:
				return Color.Lerp( Color.red , Color.white , 0.25f );
			case TeamColor.Blue:
				return Color.Lerp( Color.blue , Color.white , 0.25f );
			default:
				break;
		};
		return Color.white;
	}

}
