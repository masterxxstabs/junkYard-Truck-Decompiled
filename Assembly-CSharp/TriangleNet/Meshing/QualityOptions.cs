using System;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x02000224 RID: 548
	public class QualityOptions
	{
		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000DC0 RID: 3520 RVA: 0x000AC10A File Offset: 0x000AA30A
		// (set) Token: 0x06000DC1 RID: 3521 RVA: 0x000AC112 File Offset: 0x000AA312
		public double MaximumAngle { get; set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000DC2 RID: 3522 RVA: 0x000AC11B File Offset: 0x000AA31B
		// (set) Token: 0x06000DC3 RID: 3523 RVA: 0x000AC123 File Offset: 0x000AA323
		public double MinimumAngle { get; set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000DC4 RID: 3524 RVA: 0x000AC12C File Offset: 0x000AA32C
		// (set) Token: 0x06000DC5 RID: 3525 RVA: 0x000AC134 File Offset: 0x000AA334
		public double MaximumArea { get; set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x000AC13D File Offset: 0x000AA33D
		// (set) Token: 0x06000DC7 RID: 3527 RVA: 0x000AC145 File Offset: 0x000AA345
		public Func<ITriangle, double, bool> UserTest { get; set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x000AC14E File Offset: 0x000AA34E
		// (set) Token: 0x06000DC9 RID: 3529 RVA: 0x000AC156 File Offset: 0x000AA356
		public bool VariableArea { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000DCA RID: 3530 RVA: 0x000AC15F File Offset: 0x000AA35F
		// (set) Token: 0x06000DCB RID: 3531 RVA: 0x000AC167 File Offset: 0x000AA367
		public int SteinerPoints { get; set; }
	}
}
