using System;
using System.Reflection;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Demo
{
	// Token: 0x0200025C RID: 604
	public class DemoTelemetry : MonoBehaviour
	{
		// Token: 0x06000FE3 RID: 4067 RVA: 0x000B9584 File Offset: 0x000B7784
		private void LateUpdate()
		{
			if (Time.frameCount % 5 != 0)
			{
				return;
			}
			this.vc = VehicleChanger.ActiveVehicleController;
			if (this.vc == null)
			{
				return;
			}
			this.AddTitle("Vehicle", '_');
			this.PrintProperties(this.vc, "");
			this.AddTitle("Engine", '_');
			this.PrintProperties(this.vc.powertrain.engine, "");
			this.AddTitle("Forced Induction", ' ');
			this.PrintProperties(this.vc.powertrain.engine.forcedInduction, "");
			this.AddSpace();
			this.AddTitle("Clutch", '_');
			this.PrintProperties(this.vc.powertrain.clutch, "");
			this.AddSpace();
			this.AddTitle("Transmission", '_');
			this.PrintProperties(this.vc.powertrain.transmission, "");
			this.AddSpace();
			this.AddTitle("Axles", '_');
			int num = 0;
			foreach (WheelGroup wheelGroup in this.vc.powertrain.wheelGroups)
			{
				this.AddTitle("Axle " + num, '_');
				this.PrintProperties(wheelGroup, "");
				this.AddTitle("Left Wheel", ' ');
				this.PrintProperties(wheelGroup.LeftWheel, "");
				this.PrintProperties(wheelGroup.LeftWheel.wheelController, "");
				this.PrintProperties(wheelGroup.LeftWheel.wheelController.forwardFriction, "Long. ");
				this.PrintProperties(wheelGroup.LeftWheel.wheelController.sideFriction, "Lat. ");
				this.PrintProperties(wheelGroup.LeftWheel.wheelController.wheel, "");
				this.PrintProperties(wheelGroup.LeftWheel.wheelController.spring, "Spring ");
				this.PrintProperties(wheelGroup.LeftWheel.wheelController.damper, "Damper ");
				this.AddTitle("Right Wheel:", ' ');
				this.PrintProperties(wheelGroup.RightWheel, "");
				this.PrintProperties(wheelGroup.RightWheel.wheelController, "");
				this.PrintProperties(wheelGroup.RightWheel.wheelController.forwardFriction, "Long. ");
				this.PrintProperties(wheelGroup.RightWheel.wheelController.sideFriction, "Lat. ");
				this.PrintProperties(wheelGroup.RightWheel.wheelController.wheel, "");
				this.PrintProperties(wheelGroup.RightWheel.wheelController.spring, "Spring ");
				this.PrintProperties(wheelGroup.RightWheel.wheelController.damper, "Damper");
				num++;
			}
			this.textUI.text = this.text;
			this.text = "";
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x000B98B0 File Offset: 0x000B7AB0
		private void PrintProperties(object obj, string prefix = "")
		{
			foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty))
			{
				if (fieldInfo.IsDefined(typeof(ShowInTelemetry), false))
				{
					if (fieldInfo.FieldType == typeof(float))
					{
						string value = ((float)fieldInfo.GetValue(obj)).ToString("0.00");
						this.AddLine(prefix + fieldInfo.Name, value);
					}
					else
					{
						try
						{
							this.AddLine(prefix + fieldInfo.Name, fieldInfo.GetValue(obj).ToString());
						}
						catch
						{
						}
					}
				}
			}
			foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty))
			{
				if (propertyInfo.IsDefined(typeof(ShowInTelemetry), false))
				{
					if (propertyInfo.PropertyType == typeof(float))
					{
						string value2 = ((float)propertyInfo.GetValue(obj, null)).ToString("0.00");
						this.AddLine(propertyInfo.Name, value2);
					}
					else
					{
						try
						{
							this.AddLine(propertyInfo.Name, propertyInfo.GetValue(obj, null).ToString());
						}
						catch
						{
						}
					}
				}
			}
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x000B9A2C File Offset: 0x000B7C2C
		private void AddLine(string name, string value = "")
		{
			name = this.Truncate(name, 23);
			this.text = this.text + string.Format("{0,-26}{1,14}", DemoTelemetry.ChangeCase(name), value) + "\n";
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x000B9A60 File Offset: 0x000B7C60
		private void AddLine(string name, float value)
		{
			string value2 = value.ToString("0.0");
			this.AddLine(name, value2);
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x000B9A82 File Offset: 0x000B7C82
		private void AddTitle(string title, char filler = '_')
		{
			this.text = this.text + "\n" + this.CenterString(title, 40, filler);
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x000B9AA4 File Offset: 0x000B7CA4
		private void AddSpace()
		{
			this.text += "\n";
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x000B9ABC File Offset: 0x000B7CBC
		private string CenterString(string stringToCenter, int totalLength, char filler)
		{
			return stringToCenter.PadLeft((totalLength - stringToCenter.Length) / 2 + stringToCenter.Length, filler).PadRight(totalLength, filler) + "\n";
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x000B9AE7 File Offset: 0x000B7CE7
		public string Truncate(string value, int maxChars)
		{
			if (value.Length > maxChars)
			{
				return value.Substring(0, maxChars) + "..";
			}
			return value;
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x000B9B08 File Offset: 0x000B7D08
		public static string ChangeCase(string str)
		{
			if (str == null)
			{
				return null;
			}
			if (str.Length > 1)
			{
				return char.ToUpper(str[0]).ToString() + str.Substring(1);
			}
			return str.ToUpper();
		}

		// Token: 0x0400207E RID: 8318
		public Text textUI;

		// Token: 0x0400207F RID: 8319
		private string text;

		// Token: 0x04002080 RID: 8320
		private VehicleController vc;
	}
}
