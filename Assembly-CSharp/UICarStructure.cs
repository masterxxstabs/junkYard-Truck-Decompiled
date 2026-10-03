using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C8 RID: 200
public class UICarStructure : MonoBehaviour
{
	// Token: 0x060004A0 RID: 1184 RVA: 0x0002FF8E File Offset: 0x0002E18E
	private void Awake()
	{
		this.Image = base.GetComponent<Image>();
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0002FF9C File Offset: 0x0002E19C
	private void Update()
	{
		this.Image.fillAmount = 1f - this.Car.CarDamage;
	}

	// Token: 0x04000960 RID: 2400
	public Car Car;

	// Token: 0x04000961 RID: 2401
	private Image Image;
}
