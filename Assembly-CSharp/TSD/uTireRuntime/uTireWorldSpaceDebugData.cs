using System;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x0200034E RID: 846
	internal struct uTireWorldSpaceDebugData
	{
		// Token: 0x060015BA RID: 5562 RVA: 0x000E1AA6 File Offset: 0x000DFCA6
		public uTireWorldSpaceDebugData(Vector4 _startPos, Vector4 _endPos, Color _lineColor)
		{
			this.startPos = _startPos;
			this.endPos = _endPos;
			this.lineColor = _lineColor;
		}

		// Token: 0x04002657 RID: 9815
		public Vector4 startPos;

		// Token: 0x04002658 RID: 9816
		public Vector4 endPos;

		// Token: 0x04002659 RID: 9817
		public Color lineColor;
	}
}
