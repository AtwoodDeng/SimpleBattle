using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[SerializeField] Text phaseText;
	[SerializeField] Button NextPhaseButton;

	void Start()
	{
		LogicManager.Instance.AddOnPhaseChange (delegate(LogicManager.State fromState, LogicManager.State toState) {
			phaseText.text = toState.ToString();

		});
	}

	void Update()
	{
		if (LogicManager.MState == LogicManager.State.Strategy) {
			if (LogicManager.IsAutoPlay)
				NextPhaseButton.interactable = false;
			else {
				if (LogicManager.Instance.IsAllReady (TeamColor.Blue))
					NextPhaseButton.interactable = true;
				else
					NextPhaseButton.interactable = false;
			}
		}
		if ( phaseText.text.StartsWith("WaitStrategy") )
		{
			float strategyTime = LogicManager.AIStrategyTime - LogicManager.StateTime;
			strategyTime = Mathf.Clamp( strategyTime , 0 , 999f );
			phaseText.text = "WaitStrategy(" +  ((int)strategyTime).ToString() +  ")";
			if (LogicManager.Instance.IsAllReady (TeamColor.Red))
				phaseText.text += " red all ready ";
		}
		if ( phaseText.text.StartsWith("Strategy") )
		{
			float strategyTime = LogicManager.AIStrategyTime - LogicManager.StateTime;
			strategyTime = Mathf.Clamp( strategyTime , 0 , 999f );
			phaseText.text = "Strategy(" +  ((int)strategyTime).ToString() +  ")";
		}
	}


	public void OnNextPhase()
	{
		LogicManager.Instance.NextPhase ();
	}

	public void OnAutoBattle( bool ifAuto )
	{
		LogicArg arg = new LogicArg (this);
		arg.AddMessage ("ifAuto", ifAuto);
		M_Event.FireLogicEvent (LogicEvents.UIAutoBattle, arg);
	}
}
