using System;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class drawline : MonoBehaviour
{
	// Token: 0x060008FA RID: 2298 RVA: 0x00075826 File Offset: 0x00073A26
	private void Start()
	{
		this.lineRenderer = base.gameObject.GetComponent<LineRenderer>();
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00075839 File Offset: 0x00073A39
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.origin.position);
		this.lineRenderer.SetPosition(1, this.anchor.position);
	}

	// Token: 0x04001566 RID: 5478
	private LineRenderer lineRenderer;

	// Token: 0x04001567 RID: 5479
	public Transform origin;

	// Token: 0x04001568 RID: 5480
	public Transform anchor;
}
