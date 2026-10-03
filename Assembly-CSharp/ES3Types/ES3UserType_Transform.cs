using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x02000302 RID: 770
	[Preserve]
	[ES3Properties(new string[]
	{
		"position",
		"localRotation",
		"parent"
	})]
	public class ES3UserType_Transform : ES3ComponentType
	{
		// Token: 0x0600140E RID: 5134 RVA: 0x000D461D File Offset: 0x000D281D
		public ES3UserType_Transform() : base(typeof(Transform))
		{
			ES3UserType_Transform.Instance = this;
			this.priority = 1;
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x000D463C File Offset: 0x000D283C
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Transform transform = (Transform)obj;
			writer.WriteProperty("position", transform.position, ES3Type_Vector3.Instance);
			writer.WriteProperty("localRotation", transform.localRotation, ES3Type_Quaternion.Instance);
			writer.WritePropertyByRef("parent", transform.parent);
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x000D4698 File Offset: 0x000D2898
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Transform transform = (Transform)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (!(a == "position"))
				{
					if (!(a == "localRotation"))
					{
						if (!(a == "parent"))
						{
							reader.Skip();
						}
						else
						{
							transform.parent = reader.Read<Transform>(ES3UserType_Transform.Instance);
						}
					}
					else
					{
						transform.localRotation = reader.Read<Quaternion>(ES3Type_Quaternion.Instance);
					}
				}
				else
				{
					transform.position = reader.Read<Vector3>(ES3Type_Vector3.Instance);
				}
			}
		}

		// Token: 0x0400246E RID: 9326
		public static ES3Type Instance;
	}
}
