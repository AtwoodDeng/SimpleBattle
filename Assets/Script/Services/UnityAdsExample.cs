using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsExample : MonoBehaviour {
	public void ShowRewardedAd()
	{
		Debug.Log ("Show Ads");
		if (Advertisement.IsReady())
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show(options);
		}
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
}
