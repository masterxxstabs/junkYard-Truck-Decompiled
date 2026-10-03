using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.VehiclePhysics2.SceneManagement;
using NWH.WheelController3D;
using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Demo
{
	// Token: 0x0200025B RID: 603
	public class DemoSettings : MonoBehaviour
	{
		// Token: 0x06000FD7 RID: 4055 RVA: 0x000B8AD8 File Offset: 0x000B6CD8
		private void Redraw()
		{
			this.vc = VehicleChanger.ActiveVehicleController;
			if (this.vc == null)
			{
				return;
			}
			this.AddTitle("Engine", false);
			this.AddSettings(this.vc.powertrain.engine);
			this.AddSettings(this.vc.powertrain.engine.forcedInduction);
			this.AddTitle("Clutch", false);
			this.AddSettings(this.vc.powertrain.clutch);
			this.AddTitle("Transmission", false);
			this.AddSettings(this.vc.powertrain.transmission);
			this.AddSettings(this.vc.powertrain.transmission.gearingProfile);
			this.AddTitle("Differentials", false);
			for (int i = 0; i < this.vc.powertrain.differentials.Count; i++)
			{
				this.AddSettings(this.vc.powertrain.differentials[i]);
			}
			for (int j = 0; j < 2; j++)
			{
				WheelGroup wheelGroup = this.vc.powertrain.wheelGroups[j];
				if (wheelGroup == null)
				{
					return;
				}
				this.AddTitle("Axle " + j, false);
				this.AddSettings(wheelGroup);
				this.AddTitle("Left Wheel, Axle " + j, true);
				WheelController wheelController = wheelGroup.LeftWheel.wheelController;
				this.AddSettings(wheelController);
				this.AddSettings(wheelController.wheel);
				this.AddSettings(wheelController.spring);
				this.AddSettings(wheelController.damper);
				this.AddSettings(wheelController.forwardFriction);
				this.AddSettings(wheelController.sideFriction);
				this.AddTitle("Right Wheel, Axle " + j, true);
				WheelController wheelController2 = wheelGroup.RightWheel.wheelController;
				this.AddSettings(wheelController2);
				this.AddSettings(wheelController2.wheel);
				this.AddSettings(wheelController2.spring);
				this.AddSettings(wheelController2.damper);
				this.AddSettings(wheelController2.forwardFriction);
				this.AddSettings(wheelController2.sideFriction);
			}
			this.AddTitle("Steering", false);
			this.AddSettings(this.vc.steering);
			this.AddTitle("Brakes", false);
			this.AddSettings(this.vc.brakes);
			this.AddTitle("Modules:", false);
			for (int k = 0; k < this.vc.moduleManager.modules.Count; k++)
			{
				this.AddTitle(this.vc.moduleManager.modules[k].GetType().Name, false);
				this.AddSettings(this.vc.moduleManager.modules[k]);
			}
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x000B8DB0 File Offset: 0x000B6FB0
		public void Clear()
		{
			foreach (DemoSettings.Setting setting in this.settingList)
			{
				Object.Destroy(setting.settingObject);
			}
			this.settingList.Clear();
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x000B8E10 File Offset: 0x000B7010
		private void Start()
		{
			this.Redraw();
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x000B8E18 File Offset: 0x000B7018
		private void Update()
		{
			if (VehicleChanger.ActiveVehicleController != this.vc)
			{
				this.vc = VehicleChanger.ActiveVehicleController;
				this.Redraw();
			}
			if (VehicleChanger.ActiveVehicleController == null)
			{
				this.Clear();
				return;
			}
			foreach (DemoSettings.Setting setting in this.settingList)
			{
				if (setting.valueField != null)
				{
					setting.valueField.text = setting.field.GetValue(setting.obj).ToString();
				}
			}
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x000B8ECC File Offset: 0x000B70CC
		private void AddSettings(object obj)
		{
			foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty))
			{
				if (fieldInfo.IsDefined(typeof(ShowInSettings), false))
				{
					this.AddSetting(fieldInfo, obj);
				}
			}
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x000B8F18 File Offset: 0x000B7118
		public void AddTitle(string text, bool subtitle = false)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = text;
			Text text2 = gameObject.AddComponent<Text>();
			text2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			text2.text = text;
			text2.fontSize = 12;
			text2.alignment = TextAnchor.MiddleLeft;
			if (!subtitle)
			{
				text2.fontStyle = FontStyle.Bold;
			}
			gameObject.transform.SetParent(base.gameObject.transform, false);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(260f, 25f);
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(0f, 1f);
			this.settingList.Add(new DemoSettings.Setting
			{
				settingObject = gameObject
			});
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x000B8FDC File Offset: 0x000B71DC
		public void AddSetting(FieldInfo field, object obj)
		{
			DemoSettings.Setting setting = new DemoSettings.Setting();
			setting.field = field;
			setting.obj = obj;
			setting.settingObject = Object.Instantiate<GameObject>(this.settingPrefab, base.gameObject.transform, false);
			setting.settingObject.name = field.Name + "Setting";
			setting.nameField = setting.settingObject.transform.GetChild(1).GetComponent<Text>();
			setting.valueField = setting.settingObject.transform.GetChild(2).GetComponent<Text>();
			setting.leftButton = setting.settingObject.transform.GetChild(3).GetComponent<Button>();
			setting.rightButton = setting.settingObject.transform.GetChild(4).GetComponent<Button>();
			setting.nameField.text = field.Name;
			ShowInSettings showInSettings = field.GetCustomAttributes(typeof(ShowInSettings), false).Cast<ShowInSettings>().FirstOrDefault<ShowInSettings>();
			if (showInSettings == null)
			{
				return;
			}
			if (showInSettings.name != null)
			{
				setting.nameField.text = showInSettings.name;
			}
			if (field.FieldType == typeof(float))
			{
				setting.valueField.text = ((float)field.GetValue(obj)).ToString("0.00");
				setting.min = showInSettings.min;
				setting.max = showInSettings.max;
				setting.step = showInSettings.step;
				setting.leftButton.onClick.AddListener(delegate()
				{
					this.IncrementFloat(setting, false);
				});
				setting.rightButton.onClick.AddListener(delegate()
				{
					this.IncrementFloat(setting, true);
				});
			}
			else if (field.FieldType == typeof(int))
			{
				setting.valueField.text = field.GetValue(obj).ToString();
				setting.min = (float)((int)showInSettings.min);
				setting.max = (float)((int)showInSettings.max);
				setting.step = (float)((int)showInSettings.step);
				setting.leftButton.onClick.AddListener(delegate()
				{
					this.IncrementInt(setting, false);
				});
				setting.rightButton.onClick.AddListener(delegate()
				{
					this.IncrementInt(setting, true);
				});
			}
			else if (field.FieldType == typeof(bool))
			{
				setting.valueField.text = field.GetValue(obj).ToString();
				setting.leftButton.onClick.AddListener(delegate()
				{
					this.ToggleBool(setting);
				});
				setting.rightButton.onClick.AddListener(delegate()
				{
					this.ToggleBool(setting);
				});
			}
			else if (field.FieldType.IsEnum)
			{
				Type fieldType = field.FieldType;
				setting.min = 0f;
				setting.max = (float)(fieldType.GetFields(BindingFlags.Static | BindingFlags.Public).Length - 1);
				setting.step = 1f;
				setting.leftButton.onClick.AddListener(delegate()
				{
					this.IncrementEnum(setting, false);
				});
				setting.rightButton.onClick.AddListener(delegate()
				{
					this.IncrementEnum(setting, true);
				});
			}
			this.settingList.Add(setting);
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x000B93D8 File Offset: 0x000B75D8
		public void IncrementEnum(DemoSettings.Setting setting, bool increment)
		{
			float num = (float)((int)setting.field.GetValue(setting.obj));
			num += (float)((int)setting.step * (increment ? 1 : -1));
			if (num < 0f)
			{
				num = (float)((int)setting.max);
			}
			else if (num >= setting.max)
			{
				num = 0f;
			}
			object obj = num;
			obj = Enum.Parse(setting.field.FieldType, obj.ToString());
			setting.field.SetValue(setting.obj, obj);
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x000B9464 File Offset: 0x000B7664
		public void ToggleBool(DemoSettings.Setting setting)
		{
			bool flag = (bool)setting.field.GetValue(setting.obj);
			flag = !flag;
			setting.field.SetValue(setting.obj, flag);
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x000B94A4 File Offset: 0x000B76A4
		public void IncrementFloat(DemoSettings.Setting setting, bool increment)
		{
			float num = (float)setting.field.GetValue(setting.obj);
			num = Mathf.Clamp(num + setting.step * (increment ? 1f : -1f), setting.min, setting.max);
			setting.field.SetValue(setting.obj, num);
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x000B950C File Offset: 0x000B770C
		public void IncrementInt(DemoSettings.Setting setting, bool increment)
		{
			float num = (float)((int)setting.field.GetValue(setting.obj));
			num = Mathf.Clamp(num + (float)((int)setting.step * (increment ? 1 : -1)), (float)((int)setting.min), (float)((int)setting.max));
			setting.field.SetValue(setting.obj, num);
		}

		// Token: 0x0400207B RID: 8315
		private VehicleController vc;

		// Token: 0x0400207C RID: 8316
		public List<DemoSettings.Setting> settingList = new List<DemoSettings.Setting>();

		// Token: 0x0400207D RID: 8317
		public GameObject settingPrefab;

		// Token: 0x020004AB RID: 1195
		public class Setting
		{
			// Token: 0x04002BC9 RID: 11209
			public FieldInfo field;

			// Token: 0x04002BCA RID: 11210
			public object obj;

			// Token: 0x04002BCB RID: 11211
			public Text nameField;

			// Token: 0x04002BCC RID: 11212
			public Text valueField;

			// Token: 0x04002BCD RID: 11213
			public Button leftButton;

			// Token: 0x04002BCE RID: 11214
			public Button rightButton;

			// Token: 0x04002BCF RID: 11215
			public GameObject settingObject;

			// Token: 0x04002BD0 RID: 11216
			public float min;

			// Token: 0x04002BD1 RID: 11217
			public float max;

			// Token: 0x04002BD2 RID: 11218
			public float step;
		}
	}
}
