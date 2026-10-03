using System;
using UnityEngine;

namespace WaveMaker
{
	// Token: 0x020001A4 RID: 420
	[CreateAssetMenu(fileName = "WaveMakerDescriptor", menuName = "WaveMaker Descriptor", order = 10)]
	public class WaveMakerDescriptor : ScriptableObject
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000A30 RID: 2608 RVA: 0x0008AB56 File Offset: 0x00088D56
		public int MaxResolution
		{
			get
			{
				return this._maxResolution;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000A31 RID: 2609 RVA: 0x0008AB5E File Offset: 0x00088D5E
		public bool IsInitialized
		{
			get
			{
				return this._isInitialized;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000A32 RID: 2610 RVA: 0x0008AB66 File Offset: 0x00088D66
		public ref bool[] FixedGridRef
		{
			get
			{
				return ref this.fixedGrid;
			}
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x0008AB6E File Offset: 0x00088D6E
		private void Awake()
		{
			this._isInitialized = false;
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x0008AB78 File Offset: 0x00088D78
		private void OnEnable()
		{
			if (this.fixedGrid == null)
			{
				this.fixedGrid = new bool[this.ResolutionX * this.ResolutionZ];
				this.UpdateFixedGrid(false);
			}
			this.oldResolutionX = this.ResolutionX;
			this.oldResolutionZ = this.ResolutionZ;
			this._isInitialized = true;
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x0008ABCC File Offset: 0x00088DCC
		public void SetFixed(int x, int z, bool isFixed)
		{
			if (x >= 0 && x < this.ResolutionX && z >= 0 && z < this.ResolutionZ)
			{
				this.fixedGrid[this.ResolutionX * z + x] = isFixed;
				return;
			}
			if (x == 0 || z == 0 || x == this.ResolutionX - 1 || z == this.ResolutionZ - 1)
			{
				return;
			}
			Debug.LogWarning(string.Concat(new object[]
			{
				"WaveMaker - Cannot set the fixed status to the given sample. It is out of bounds. ",
				x,
				" - ",
				z
			}));
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x0008AC58 File Offset: 0x00088E58
		public bool IsFixed(int x, int z)
		{
			if (x < 0 || x >= this.ResolutionX || z < 0 || z >= this.ResolutionZ)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"WaveMaker - Cannot get the fixed status to the given sample. It is out of bounds. ",
					x,
					" - ",
					z
				}));
				return true;
			}
			return this.fixedGrid[this.ResolutionX * z + x];
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x0008ACC4 File Offset: 0x00088EC4
		public void SetResolution(int newResolutionX, int newResolutionZ)
		{
			if (newResolutionX < 2 || newResolutionZ < 2 || newResolutionX > this._maxResolution || newResolutionZ > this._maxResolution)
			{
				newResolutionX = Mathf.Clamp(newResolutionX, 2, this._maxResolution);
				newResolutionZ = Mathf.Clamp(newResolutionZ, 2, this._maxResolution);
				Debug.LogError("WaveMaker - Descriptor resolution cannot be out of the range (2-" + this._maxResolution + "). Clamping.");
			}
			this.oldResolutionX = this.ResolutionX;
			this.oldResolutionZ = this.ResolutionZ;
			this.ResolutionX = newResolutionX;
			this.ResolutionZ = newResolutionZ;
			this.UpdateFixedGrid(true);
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x0008AD58 File Offset: 0x00088F58
		public void FixBorders()
		{
			for (int i = 0; i < this.ResolutionX; i++)
			{
				for (int j = 0; j < this.ResolutionZ; j++)
				{
					if (i == 0 || j == 0 || i == this.ResolutionX - 1 || j == this.ResolutionZ - 1)
					{
						this.fixedGrid[this.ResolutionX * j + i] = true;
					}
				}
			}
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0008ADB8 File Offset: 0x00088FB8
		public void UpdateFixedGrid(bool copyPreviousStatus = true)
		{
			if (this.fixedGrid == null)
			{
				copyPreviousStatus = false;
			}
			bool[] array = new bool[this.ResolutionX * this.ResolutionZ];
			for (int i = 0; i < this.ResolutionX; i++)
			{
				for (int j = 0; j < this.ResolutionZ; j++)
				{
					int num = j * this.ResolutionX + i;
					array[num] = false;
					if (copyPreviousStatus && i < this.oldResolutionX && j < this.oldResolutionZ)
					{
						array[num] = this.fixedGrid[j * this.oldResolutionX + i];
					}
				}
			}
			this.fixedGrid = array;
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x0008AE44 File Offset: 0x00089044
		public void SetAllFixStatus(bool newValue = false)
		{
			for (int i = 0; i < this.fixedGrid.Length; i++)
			{
				this.fixedGrid[i] = newValue;
			}
		}

		// Token: 0x04001C2F RID: 7215
		[Header("Plane Resolution")]
		public int ResolutionX = 50;

		// Token: 0x04001C30 RID: 7216
		public int ResolutionZ = 50;

		// Token: 0x04001C31 RID: 7217
		[HideInInspector]
		public Color defaultColor = Color.white;

		// Token: 0x04001C32 RID: 7218
		[HideInInspector]
		public Color fixedColor = Color.black;

		// Token: 0x04001C33 RID: 7219
		[SerializeField]
		private bool[] fixedGrid;

		// Token: 0x04001C34 RID: 7220
		private int _maxResolution = 256;

		// Token: 0x04001C35 RID: 7221
		private bool _isInitialized;

		// Token: 0x04001C36 RID: 7222
		private int oldResolutionX;

		// Token: 0x04001C37 RID: 7223
		private int oldResolutionZ;
	}
}
