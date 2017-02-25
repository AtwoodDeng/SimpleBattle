using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : MonoBehaviour {

	void Start() {
		if (Application.platform == RuntimePlatform.Android ||
			Application.isEditor )
			Advertisement.Initialize ("1322066"  );
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			Advertisement.Initialize ("1322065"  );

//		Debug.Log ("Ads Inited " + Advertisement.isInitialized);
//		Debug.Log ("Ads Support " + Advertisement.isSupported);
//		Debug.Log ("Ads Test Mode " + Advertisement.testMode);
	}
}
