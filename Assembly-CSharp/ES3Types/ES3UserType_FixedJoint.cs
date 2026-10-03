using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002E4 RID: 740
	[Preserve]
	[ES3Properties(new string[]
	{
		"connectedBody"
	})]
	public class ES3UserType_FixedJoint : ES3ComponentType
	{
		// Token: 0x060013C3 RID: 5059 RVA: 0x000CD271 File Offset: 0x000CB471
		public ES3UserType_FixedJoint() : base(typeof(FixedJoint))
		{
			ES3UserType_FixedJoint.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x000CD290 File Offset: 0x000CB490
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			FixedJoint fixedJoint = (FixedJoint)obj;
			writer.WritePropertyByRef("connectedBody", fixedJoint.connectedBody);
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x000CD2B8 File Offset: 0x000CB4B8
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			FixedJoint fixedJoint = (FixedJoint)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (a == "connectedBody")
				{
					fixedJoint.connectedBody = reader.Read<Rigidbody>(ES3UserType_Rigidbody.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
		}

		// Token: 0x04002450 RID: 9296
		public static ES3Type Instance;
	}
}
