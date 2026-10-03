using System;

namespace TriangleNet
{
	// Token: 0x020001EF RID: 495
	public class Configuration
	{
		// Token: 0x06000BF0 RID: 3056 RVA: 0x000953A4 File Offset: 0x000935A4
		public Configuration() : this(() => RobustPredicates.Default, () => new TrianglePool())
		{
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x000953F5 File Offset: 0x000935F5
		public Configuration(Func<IPredicates> predicates) : this(predicates, () => new TrianglePool())
		{
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0009541D File Offset: 0x0009361D
		public Configuration(Func<IPredicates> predicates, Func<TrianglePool> trianglePool)
		{
			this.Predicates = predicates;
			this.TrianglePool = trianglePool;
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000BF3 RID: 3059 RVA: 0x00095433 File Offset: 0x00093633
		// (set) Token: 0x06000BF4 RID: 3060 RVA: 0x0009543B File Offset: 0x0009363B
		public Func<IPredicates> Predicates { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000BF5 RID: 3061 RVA: 0x00095444 File Offset: 0x00093644
		// (set) Token: 0x06000BF6 RID: 3062 RVA: 0x0009544C File Offset: 0x0009364C
		public Func<TrianglePool> TrianglePool { get; set; }
	}
}
