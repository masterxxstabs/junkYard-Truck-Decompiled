using System;
using UnityEngine;

// Token: 0x020000C2 RID: 194
[RequireComponent(typeof(Car))]
public class CarControl : MonoBehaviour
{
	// Token: 0x0600048A RID: 1162 RVA: 0x0002FCB8 File Offset: 0x0002DEB8
	private void Awake()
	{
		this.car = base.GetComponent<Car>();
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0002FCC6 File Offset: 0x0002DEC6
	protected void ControlCar(float acel, float turn)
	{
		this.car.Control = new Vector2(turn, acel);
	}

	// Token: 0x04000957 RID: 2391
	protected Car car;
}
