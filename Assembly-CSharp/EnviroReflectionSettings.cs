using System;
using UnityEngine;

// Token: 0x0200007E RID: 126
[Serializable]
public class EnviroReflectionSettings
{
	// Token: 0x040005EF RID: 1519
	[Header("Global Reflections Settings")]
	[Tooltip("Enable/disable enviro reflection probe..")]
	public bool globalReflections = true;

	// Token: 0x040005F0 RID: 1520
	[Header("Global Reflections Custom Rendering")]
	[Tooltip("Enable/disable if enviro reflection probe should render in custom mode to support clouds and other enviro effects.")]
	public bool globalReflectionCustomRendering = true;

	// Token: 0x040005F1 RID: 1521
	[Tooltip("Enable/disable if enviro reflection probe should render with fog.")]
	public bool globalReflectionUseFog;

	// Token: 0x040005F2 RID: 1522
	[Tooltip("Set if enviro reflection probe should update faces individual on different frames.")]
	public bool globalReflectionTimeSlicing = true;

	// Token: 0x040005F3 RID: 1523
	[Header("Global Reflections Updates Settings")]
	[Tooltip("Enable/disable enviro reflection probe updates based on gametime changes..")]
	public bool globalReflectionsUpdateOnGameTime = true;

	// Token: 0x040005F4 RID: 1524
	[Tooltip("Enable/disable enviro reflection probe updates based on transform position changes..")]
	public bool globalReflectionsUpdateOnPosition = true;

	// Token: 0x040005F5 RID: 1525
	[Tooltip("Reflection probe intensity.")]
	[Range(0f, 2f)]
	public float globalReflectionsIntensity = 0.5f;

	// Token: 0x040005F6 RID: 1526
	[Tooltip("Reflection probe update rate.")]
	public float globalReflectionsUpdateTreshhold = 0.025f;

	// Token: 0x040005F7 RID: 1527
	[Tooltip("Reflection probe intensity.")]
	[Range(0.1f, 10f)]
	public float globalReflectionsScale = 1f;

	// Token: 0x040005F8 RID: 1528
	[Tooltip("Reflection probe resolution.")]
	public EnviroReflectionSettings.GlobalReflectionResolution globalReflectionResolution = EnviroReflectionSettings.GlobalReflectionResolution.R256;

	// Token: 0x040005F9 RID: 1529
	[Tooltip("Reflection probe rendered Layers.")]
	public LayerMask globalReflectionLayers;

	// Token: 0x040005FA RID: 1530
	[Tooltip("Set the quality of clouds in reflection rendering. Leave empty to use global settings.")]
	public EnviroVolumeCloudsQuality reflectionCloudsQuality;

	// Token: 0x0200037D RID: 893
	public enum GlobalReflectionResolution
	{
		// Token: 0x040026FF RID: 9983
		R16,
		// Token: 0x04002700 RID: 9984
		R32,
		// Token: 0x04002701 RID: 9985
		R64,
		// Token: 0x04002702 RID: 9986
		R128,
		// Token: 0x04002703 RID: 9987
		R256,
		// Token: 0x04002704 RID: 9988
		R512,
		// Token: 0x04002705 RID: 9989
		R1024,
		// Token: 0x04002706 RID: 9990
		R2048
	}
}
