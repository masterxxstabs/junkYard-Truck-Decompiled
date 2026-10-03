using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class BoltOverride : MonoBehaviour
{
	// Token: 0x0600005B RID: 91 RVA: 0x00004C0C File Offset: 0x00002E0C
	private void Start()
	{
		durability component = base.GetComponent<durability>();
		component.boltStr = 0f;
		component.numBolts = 0;
		component.canDetach = true;
		Transform child = base.transform.GetChild(0);
		bool flag = false;
		BoxCollider[] components = base.GetComponents<BoxCollider>();
		using (IEnumerator enumerator = child.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (((Transform)enumerator.Current).gameObject.activeSelf)
				{
					if (!components[0].isTrigger)
					{
						components[0].enabled = true;
					}
					if (!components[1].isTrigger)
					{
						components[1].enabled = true;
					}
					flag = true;
					break;
				}
			}
		}
		if (!flag && base.transform.parent.GetComponent<Renderer>().enabled)
		{
			if (components[0].isTrigger)
			{
				components[0].enabled = true;
			}
			if (components[1].isTrigger)
			{
				components[1].enabled = true;
			}
		}
	}
}
