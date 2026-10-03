using System;
using System.Collections.Generic;
using TriangleNet.Logging;

namespace TriangleNet
{
	// Token: 0x020001F6 RID: 502
	public sealed class Log : ILog<LogItem>
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000BFB RID: 3067 RVA: 0x00095455 File Offset: 0x00093655
		// (set) Token: 0x06000BFC RID: 3068 RVA: 0x0009545C File Offset: 0x0009365C
		public static bool Verbose { get; set; }

		// Token: 0x06000BFE RID: 3070 RVA: 0x00095470 File Offset: 0x00093670
		private Log()
		{
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000BFF RID: 3071 RVA: 0x00095483 File Offset: 0x00093683
		public static ILog<LogItem> Instance
		{
			get
			{
				return Log.instance;
			}
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0009548A File Offset: 0x0009368A
		public void Add(LogItem item)
		{
			this.log.Add(item);
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x00095498 File Offset: 0x00093698
		public void Clear()
		{
			this.log.Clear();
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x000954A5 File Offset: 0x000936A5
		public void Info(string message)
		{
			this.log.Add(new LogItem(LogLevel.Info, message));
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x000954B9 File Offset: 0x000936B9
		public void Warning(string message, string location)
		{
			this.log.Add(new LogItem(LogLevel.Warning, message, location));
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x000954CE File Offset: 0x000936CE
		public void Error(string message, string location)
		{
			this.log.Add(new LogItem(LogLevel.Error, message, location));
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000C05 RID: 3077 RVA: 0x000954E3 File Offset: 0x000936E3
		public IList<LogItem> Data
		{
			get
			{
				return this.log;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000C06 RID: 3078 RVA: 0x000954EB File Offset: 0x000936EB
		public LogLevel Level
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x04001DEA RID: 7658
		private List<LogItem> log = new List<LogItem>();

		// Token: 0x04001DEB RID: 7659
		private LogLevel level;

		// Token: 0x04001DEC RID: 7660
		private static readonly Log instance = new Log();
	}
}
