using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000056 RID: 86
public class DropRelease : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	// Token: 0x06000197 RID: 407 RVA: 0x000115D2 File Offset: 0x0000F7D2
	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log(base.gameObject.name);
	}
}
