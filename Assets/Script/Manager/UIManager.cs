using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[SerializeField] Text phaseText;

	void Start()
	{
		LogicManager.Instance.AddOnPhaseChange (delegate(LogicManager.State fromState, LogicManager.State toState) {
			phaseText.text = toState.ToString();
		});
	}

	public void OnNextPhase()
	{
		LogicManager.Instance.NextPhase ();
	}
}
