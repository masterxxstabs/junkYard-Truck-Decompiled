using System;
using UnityEngine;

namespace AshVP
{
	// Token: 0x0200033B RID: 827
	public class GetReference : MonoBehaviour
	{
		// Token: 0x06001535 RID: 5429 RVA: 0x000DFD0C File Offset: 0x000DDF0C
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x000DFD1C File Offset: 0x000DDF1C
		private void OnDrawGizmosSelected()
		{
			if (this.gearShifts == null)
			{
				return;
			}
			for (int i = 0; i < this.gearShifts.Length; i++)
			{
				this.gearShifts[i].GearShiftNumber = i + 1;
			}
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x000DFD55 File Offset: 0x000DDF55
		private void Update()
		{
			this.CurrentSpeedKmph = this.CurrentSpeedMps * 3.6f;
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x000DFD6C File Offset: 0x000DDF6C
		private void FixedUpdate()
		{
			this.CurrentSpeedMps = base.transform.InverseTransformDirection(this.rb.velocity).magnitude;
			for (int i = 0; i < this.gearShifts.Length; i++)
			{
				if (this.CurrentSpeedMps < 0f)
				{
					this.currentGear = 0;
				}
				else if (i == 0 && 0f < this.CurrentSpeedMps && this.CurrentSpeedMps < this.gearShifts[i].ShiftAt * 100f)
				{
					this.currentGear = this.gearShifts[i].GearShiftNumber;
				}
				else
				{
					if (this.CurrentSpeedMps < this.gearShifts[i].ShiftAt * 100f)
					{
						return;
					}
					if (i == this.gearShifts.Length - 1 && this.gearShifts[i - 1].ShiftAt * 100f < this.CurrentSpeedMps && this.CurrentSpeedMps < this.gearShifts[i].ShiftAt * 100f)
					{
						this.currentGear = this.gearShifts[i].GearShiftNumber;
					}
					else
					{
						this.currentGear = this.gearShifts[i].GearShiftNumber + 1;
					}
				}
			}
		}

		// Token: 0x040025F3 RID: 9715
		private Rigidbody rb;

		// Token: 0x040025F4 RID: 9716
		public float CurrentSpeedMps;

		// Token: 0x040025F5 RID: 9717
		public float CurrentSpeedKmph;

		// Token: 0x040025F6 RID: 9718
		public int currentGear;

		// Token: 0x040025F7 RID: 9719
		[SerializeField]
		private GetReference.GearShifts[] gearShifts;

		// Token: 0x020004F5 RID: 1269
		[Serializable]
		private class GearShifts
		{
			// Token: 0x04002CE6 RID: 11494
			public int GearShiftNumber;

			// Token: 0x04002CE7 RID: 11495
			public float ShiftAt;
		}
	}
}
