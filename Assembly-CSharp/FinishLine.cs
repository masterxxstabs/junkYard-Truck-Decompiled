using System;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class FinishLine : MonoBehaviour
{
	// Token: 0x06000440 RID: 1088 RVA: 0x0002CCE4 File Offset: 0x0002AEE4
	private void OnTriggerEnter(Collider other)
	{
		if (this.raceTruck.activeSelf)
		{
			this.minCheckpoints = 65;
		}
		else
		{
			this.minCheckpoints = 28;
		}
		if (!this.racenpc.finished)
		{
			if ((other.gameObject.name == "dirt pickup truck" || other.gameObject.name == "f1003" || other.gameObject.name == "DirtBike") && this.racenpc.elapsed)
			{
				if (this.interactor.checkpoints >= this.minCheckpoints && this.interactor.raceActive)
				{
					this.interactor.checkpoints = 0;
					this.interactor.raceActive = false;
					this.racenpc.wonrace = true;
					this.racenpc.finished = true;
					if (other.gameObject.name == "dirt pickup truck")
					{
						this.racenpc.whichCar = 0;
					}
					else if (other.gameObject.name == "f1003")
					{
						this.racenpc.whichCar = 1;
					}
					else if (other.gameObject.name == "DirtBike")
					{
						this.racenpc.whichCar = 2;
					}
					this.racenpc.ActivateTrophy();
				}
				else if (this.interactor.checkpoints > 0 && !this.interactor.raceActive)
				{
					this.interactor.checkpoints = 0;
					this.interactor.raceActive = false;
					this.racenpc.wonrace = false;
					this.racenpc.finished = true;
				}
			}
			if (other.gameObject.name == "RaceTruckAI1" && this.racenpc.elapsed)
			{
				this.racenpc.finished = true;
				this.interactor.checkpoints = 0;
				this.interactor.raceActive = false;
				this.racenpc.wonrace = false;
			}
			if (other.gameObject.name == "RaceCarAI1" && this.racenpc.elapsed)
			{
				this.racenpc.finished = true;
				this.interactor.checkpoints = 0;
				this.interactor.raceActive = false;
				this.racenpc.wonrace = false;
			}
		}
	}

	// Token: 0x0400089A RID: 2202
	public Interactor interactor;

	// Token: 0x0400089B RID: 2203
	private int minCheckpoints = 65;

	// Token: 0x0400089C RID: 2204
	public RaceNpc racenpc;

	// Token: 0x0400089D RID: 2205
	public GameObject raceTruck;

	// Token: 0x0400089E RID: 2206
	public GameObject raceCar;
}
