using System;

namespace TriangleNet.Logging
{
	// Token: 0x02000231 RID: 561
	public class LogItem : ILogItem
	{
		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x000AE970 File Offset: 0x000ACB70
		public DateTime Time
		{
			get
			{
				return this.time;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x000AE978 File Offset: 0x000ACB78
		public LogLevel Level
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x000AE980 File Offset: 0x000ACB80
		public string Message
		{
			get
			{
				return this.message;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000E0C RID: 3596 RVA: 0x000AE988 File Offset: 0x000ACB88
		public string Info
		{
			get
			{
				return this.info;
			}
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x000AE990 File Offset: 0x000ACB90
		public LogItem(LogLevel level, string message) : this(level, message, "")
		{
		}

		// Token: 0x06000E0E RID: 3598 RVA: 0x000AE99F File Offset: 0x000ACB9F
		public LogItem(LogLevel level, string message, string info)
		{
			this.time = DateTime.Now;
			this.level = level;
			this.message = message;
			this.info = info;
		}

		// Token: 0x04001EF8 RID: 7928
		private DateTime time;

		// Token: 0x04001EF9 RID: 7929
		private LogLevel level;

		// Token: 0x04001EFA RID: 7930
		private string message;

		// Token: 0x04001EFB RID: 7931
		private string info;
	}
}
