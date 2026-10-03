using System;

namespace TriangleNet.Logging
{
	// Token: 0x02000230 RID: 560
	public interface ILogItem
	{
		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000E05 RID: 3589
		DateTime Time { get; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000E06 RID: 3590
		LogLevel Level { get; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000E07 RID: 3591
		string Message { get; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000E08 RID: 3592
		string Info { get; }
	}
}
