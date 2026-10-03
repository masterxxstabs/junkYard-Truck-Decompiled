using System;

namespace NWH.VehiclePhysics2.Utility
{
	// Token: 0x02000264 RID: 612
	public static class UnitConverter
	{
		// Token: 0x06001014 RID: 4116 RVA: 0x000BA22D File Offset: 0x000B842D
		public static float KmlToL100km(float kml)
		{
			if (kml != 0f)
			{
				return 100f / kml;
			}
			return float.PositiveInfinity;
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x000BA244 File Offset: 0x000B8444
		public static float KmlToMpg(float kml)
		{
			return kml * 2.825f;
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x000BA24D File Offset: 0x000B844D
		public static float L100kmToKml(float l100km)
		{
			if (l100km != 0f)
			{
				return 100f / l100km;
			}
			return 0f;
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x000BA264 File Offset: 0x000B8464
		public static float L100kmToMpg(float l100km)
		{
			if (l100km != 0f)
			{
				return 282.5f / l100km;
			}
			return 0f;
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x000BA27B File Offset: 0x000B847B
		public static float AngularVelocityToRPM(float angularVelocity)
		{
			return angularVelocity * 9.549296f;
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x000BA284 File Offset: 0x000B8484
		public static float RPMToAngularVelocity(float RPM)
		{
			return RPM * 0.10471976f;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x000BA28D File Offset: 0x000B848D
		public static float MpgToKml(float mpg)
		{
			return mpg * 0.354f;
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x000BA296 File Offset: 0x000B8496
		public static float MpgToL100km(float mpg)
		{
			if (mpg != 0f)
			{
				return 282.5f / mpg;
			}
			return float.PositiveInfinity;
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x000BA2AD File Offset: 0x000B84AD
		public static float MphToKph(float value)
		{
			return value * 1.60934f;
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x000BA2B6 File Offset: 0x000B84B6
		public static float MpsToKph(float value)
		{
			return value * 3.6f;
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x000BA2BF File Offset: 0x000B84BF
		public static float MpsToMph(float value)
		{
			return value * 2.23694f;
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x000BA2C8 File Offset: 0x000B84C8
		public static float Speed_kmhToMph(float kmh)
		{
			return kmh * 0.621371f;
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x000BA2D1 File Offset: 0x000B84D1
		public static float Speed_kmhToMs(float kmh)
		{
			return kmh * 0.277778f;
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x000BA2AD File Offset: 0x000B84AD
		public static float Speed_mphToKmh(float mph)
		{
			return mph * 1.60934f;
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x000BA2DA File Offset: 0x000B84DA
		public static float Speed_mphToMs(float mph)
		{
			return mph * 0.44704f;
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x000BA2B6 File Offset: 0x000B84B6
		public static float Speed_msToKph(float ms)
		{
			return ms * 3.6f;
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x000BA2BF File Offset: 0x000B84BF
		public static float Speed_msToMph(float ms)
		{
			return ms * 2.23694f;
		}
	}
}
