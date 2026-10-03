using System;
using TriangleNet.Geometry;

namespace TriangleNet
{
	// Token: 0x020001F5 RID: 501
	public interface IPredicates
	{
		// Token: 0x06000BF7 RID: 3063
		double CounterClockwise(Point a, Point b, Point c);

		// Token: 0x06000BF8 RID: 3064
		double InCircle(Point a, Point b, Point c, Point p);

		// Token: 0x06000BF9 RID: 3065
		Point FindCircumcenter(Point org, Point dest, Point apex, ref double xi, ref double eta);

		// Token: 0x06000BFA RID: 3066
		Point FindCircumcenter(Point org, Point dest, Point apex, ref double xi, ref double eta, double offconstant);
	}
}
