using System;
using System.Collections.Generic;

namespace TriangleNet.Logging
{
	// Token: 0x0200022F RID: 559
	public interface ILog<T> where T : ILogItem
	{
		// Token: 0x06000DFE RID: 3582
		void Add(T item);

		// Token: 0x06000DFF RID: 3583
		void Clear();

		// Token: 0x06000E00 RID: 3584
		void Info(string message);

		// Token: 0x06000E01 RID: 3585
		void Error(string message, string info);

		// Token: 0x06000E02 RID: 3586
		void Warning(string message, string info);

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000E03 RID: 3587
		IList<T> Data { get; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000E04 RID: 3588
		LogLevel Level { get; }
	}
}
