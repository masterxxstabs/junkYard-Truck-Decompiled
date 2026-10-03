using System;
using ES3Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002FE RID: 766
	[Preserve]
	[ES3Properties(new string[]
	{
		"mass",
		"useGravity",
		"maxDepenetrationVelocity",
		"isKinematic",
		"freezeRotation",
		"constraints",
		"collisionDetectionMode",
		"centerOfMass",
		"detectCollisions",
		"position",
		"rotation",
		"interpolation",
		"solverIterations",
		"sleepThreshold",
		"maxAngularVelocity",
		"solverVelocityIterations"
	})]
	public class ES3UserType_Rigidbody : ES3ComponentType
	{
		// Token: 0x06001404 RID: 5124 RVA: 0x000D3EC5 File Offset: 0x000D20C5
		public ES3UserType_Rigidbody() : base(typeof(Rigidbody))
		{
			ES3UserType_Rigidbody.Instance = this;
			this.priority = 1;
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x000D3EE4 File Offset: 0x000D20E4
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			Rigidbody rigidbody = (Rigidbody)obj;
			writer.WriteProperty("mass", rigidbody.mass, ES3Type_float.Instance);
			writer.WriteProperty("useGravity", rigidbody.useGravity, ES3Type_bool.Instance);
			writer.WriteProperty("maxDepenetrationVelocity", rigidbody.maxDepenetrationVelocity, ES3Type_float.Instance);
			writer.WriteProperty("isKinematic", rigidbody.isKinematic, ES3Type_bool.Instance);
			writer.WriteProperty("freezeRotation", rigidbody.freezeRotation, ES3Type_bool.Instance);
			writer.WriteProperty("constraints", rigidbody.constraints, ES3TypeMgr.GetOrCreateES3Type(typeof(RigidbodyConstraints), true));
			writer.WriteProperty("collisionDetectionMode", rigidbody.collisionDetectionMode, ES3TypeMgr.GetOrCreateES3Type(typeof(CollisionDetectionMode), true));
			writer.WriteProperty("centerOfMass", rigidbody.centerOfMass, ES3Type_Vector3.Instance);
			writer.WriteProperty("detectCollisions", rigidbody.detectCollisions, ES3Type_bool.Instance);
			writer.WriteProperty("position", rigidbody.position, ES3Type_Vector3.Instance);
			writer.WriteProperty("rotation", rigidbody.rotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("interpolation", rigidbody.interpolation, ES3TypeMgr.GetOrCreateES3Type(typeof(RigidbodyInterpolation), true));
			writer.WriteProperty("solverIterations", rigidbody.solverIterations, ES3Type_int.Instance);
			writer.WriteProperty("sleepThreshold", rigidbody.sleepThreshold, ES3Type_float.Instance);
			writer.WriteProperty("maxAngularVelocity", rigidbody.maxAngularVelocity, ES3Type_float.Instance);
			writer.WriteProperty("solverVelocityIterations", rigidbody.solverVelocityIterations, ES3Type_int.Instance);
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x000D40CC File Offset: 0x000D22CC
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			Rigidbody rigidbody = (Rigidbody)obj;
			foreach (object obj2 in reader.Properties)
			{
				string text = (string)obj2;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2596953323U)
				{
					if (num <= 564937055U)
					{
						if (num <= 132777611U)
						{
							if (num != 69535816U)
							{
								if (num == 132777611U)
								{
									if (text == "constraints")
									{
										rigidbody.constraints = reader.Read<RigidbodyConstraints>();
										continue;
									}
								}
							}
							else if (text == "maxDepenetrationVelocity")
							{
								rigidbody.maxDepenetrationVelocity = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
						else if (num != 520558326U)
						{
							if (num == 564937055U)
							{
								if (text == "rotation")
								{
									rigidbody.rotation = reader.Read<Quaternion>(ES3Type_Quaternion.Instance);
									continue;
								}
							}
						}
						else if (text == "isKinematic")
						{
							rigidbody.isKinematic = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num <= 965172509U)
					{
						if (num != 905125296U)
						{
							if (num == 965172509U)
							{
								if (text == "interpolation")
								{
									rigidbody.interpolation = reader.Read<RigidbodyInterpolation>();
									continue;
								}
							}
						}
						else if (text == "freezeRotation")
						{
							rigidbody.freezeRotation = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num != 2471448074U)
					{
						if (num == 2596953323U)
						{
							if (text == "sleepThreshold")
							{
								rigidbody.sleepThreshold = reader.Read<float>(ES3Type_float.Instance);
								continue;
							}
						}
					}
					else if (text == "position")
					{
						rigidbody.position = reader.Read<Vector3>(ES3Type_Vector3.Instance);
						continue;
					}
				}
				else if (num <= 3648333725U)
				{
					if (num <= 3077523073U)
					{
						if (num != 2614105187U)
						{
							if (num == 3077523073U)
							{
								if (text == "centerOfMass")
								{
									rigidbody.centerOfMass = reader.Read<Vector3>(ES3Type_Vector3.Instance);
									continue;
								}
							}
						}
						else if (text == "detectCollisions")
						{
							rigidbody.detectCollisions = reader.Read<bool>(ES3Type_bool.Instance);
							continue;
						}
					}
					else if (num != 3598126642U)
					{
						if (num == 3648333725U)
						{
							if (text == "solverVelocityIterations")
							{
								rigidbody.solverVelocityIterations = reader.Read<int>(ES3Type_int.Instance);
								continue;
							}
						}
					}
					else if (text == "solverIterations")
					{
						rigidbody.solverIterations = reader.Read<int>(ES3Type_int.Instance);
						continue;
					}
				}
				else if (num <= 3774863253U)
				{
					if (num != 3749132497U)
					{
						if (num == 3774863253U)
						{
							if (text == "collisionDetectionMode")
							{
								rigidbody.collisionDetectionMode = reader.Read<CollisionDetectionMode>();
								continue;
							}
						}
					}
					else if (text == "mass")
					{
						rigidbody.mass = reader.Read<float>(ES3Type_float.Instance);
						continue;
					}
				}
				else if (num != 3838993682U)
				{
					if (num == 3866543682U)
					{
						if (text == "maxAngularVelocity")
						{
							rigidbody.maxAngularVelocity = reader.Read<float>(ES3Type_float.Instance);
							continue;
						}
					}
				}
				else if (text == "useGravity")
				{
					rigidbody.useGravity = reader.Read<bool>(ES3Type_bool.Instance);
					continue;
				}
				reader.Skip();
			}
		}

		// Token: 0x0400246A RID: 9322
		public static ES3Type Instance;
	}
}
