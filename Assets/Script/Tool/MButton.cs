using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MButton : InteractableObj {

	[SerializeField] UnityEvent OnSelect;
	[SerializeField] UnityEvent OnConfirm;

	public override void Select ()
	{
		OnSelect.Invoke();
	}

	public override void Confirm ()
	{
		OnConfirm.Invoke();
	}
}
