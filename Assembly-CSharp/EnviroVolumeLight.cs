using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

// Token: 0x020000AA RID: 170
[AddComponentMenu("Enviro/Volume Light")]
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class EnviroVolumeLight : MonoBehaviour
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x0600040B RID: 1035 RVA: 0x00029F04 File Offset: 0x00028104
	// (remove) Token: 0x0600040C RID: 1036 RVA: 0x00029F3C File Offset: 0x0002813C
	public event Action<EnviroSkyRendering, EnviroVolumeLight, CommandBuffer, Matrix4x4> CustomRenderEvent;

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600040D RID: 1037 RVA: 0x00029F71 File Offset: 0x00028171
	public Light Light
	{
		get
		{
			return this._light;
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x0600040E RID: 1038 RVA: 0x00029F79 File Offset: 0x00028179
	public Material VolumetricMaterial
	{
		get
		{
			return this._material;
		}
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00029F81 File Offset: 0x00028181
	private void Start()
	{
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStation4 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.XboxOne)
		{
			this._reversedZ = true;
		}
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00029FC0 File Offset: 0x000281C0
	private void OnEnable()
	{
		if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.LW)
		{
			base.enabled = false;
			return;
		}
		this._commandBuffer = new CommandBuffer();
		this._commandBuffer.name = "Light Command Buffer";
		this._cascadeShadowCommandBuffer = new CommandBuffer();
		this._cascadeShadowCommandBuffer.name = "Dir Light Command Buffer";
		this._cascadeShadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
		this._light = base.GetComponent<Light>();
		if (this._light.type == LightType.Directional)
		{
			this._light.AddCommandBuffer(LightEvent.BeforeScreenspaceMask, this._commandBuffer);
			this._light.AddCommandBuffer(LightEvent.AfterShadowMap, this._cascadeShadowCommandBuffer);
		}
		else
		{
			this._light.AddCommandBuffer(LightEvent.AfterShadowMap, this._commandBuffer);
		}
		if (this.volumeLightShader == null)
		{
			this.volumeLightShader = Shader.Find("Enviro/Standard/VolumeLight");
		}
		if (this.volumeLightShader == null)
		{
			throw new Exception("Critical Error: \"Enviro/VolumeLight\" shader is missing.");
		}
		if (this._light.type != LightType.Directional)
		{
			this._material = new Material(this.volumeLightShader);
		}
		EnviroSkyRendering.PreRenderEvent += this.VolumetricLightRenderer_PreRenderEvent;
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x0002A0F4 File Offset: 0x000282F4
	private void OnDisable()
	{
		if (this._light != null && this._commandBuffer != null)
		{
			if (this._light.type == LightType.Directional)
			{
				this._light.RemoveCommandBuffer(LightEvent.BeforeScreenspaceMask, this._commandBuffer);
				this._light.RemoveCommandBuffer(LightEvent.AfterShadowMap, this._cascadeShadowCommandBuffer);
			}
			else
			{
				this._light.RemoveCommandBuffer(LightEvent.AfterShadowMap, this._commandBuffer);
			}
		}
		EnviroSkyRendering.PreRenderEvent -= this.VolumetricLightRenderer_PreRenderEvent;
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0002A16E File Offset: 0x0002836E
	public void OnDestroy()
	{
		Object.DestroyImmediate(this._material);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x0002A17C File Offset: 0x0002837C
	private void VolumetricLightRenderer_PreRenderEvent(EnviroSkyRendering renderer, Matrix4x4 viewProj, Matrix4x4 viewProjSP)
	{
		if (EnviroSky.instance == null)
		{
			return;
		}
		if (this._light == null || this._light.gameObject == null)
		{
			EnviroSkyRendering.PreRenderEvent -= this.VolumetricLightRenderer_PreRenderEvent;
			return;
		}
		if (!this._light.gameObject.activeInHierarchy || !this._light.enabled)
		{
			return;
		}
		if (this._material == null)
		{
			this._material = new Material(this.volumeLightShader);
		}
		this._material.SetVector("_CameraForward", Camera.current.transform.forward);
		this._material.SetInt("_SampleCount", this.SampleCount);
		this._material.SetVector("_NoiseVelocity", new Vector4(EnviroSky.instance.volumeLightSettings.noiseVelocity.x, EnviroSky.instance.volumeLightSettings.noiseVelocity.y) * EnviroSky.instance.volumeLightSettings.noiseScale);
		this._material.SetVector("_NoiseData", new Vector4(EnviroSky.instance.volumeLightSettings.noiseScale, EnviroSky.instance.volumeLightSettings.noiseIntensity, EnviroSky.instance.volumeLightSettings.noiseIntensityOffset));
		this._material.SetVector("_MieG", new Vector4(1f - this.Anistropy * this.Anistropy, 1f + this.Anistropy * this.Anistropy, 2f * this.Anistropy, 0.07957747f));
		float x = this.ScatteringCoef;
		if (this.scaleWithTime)
		{
			x = this.ScatteringCoef * (1f - EnviroSky.instance.GameTime.solarTime);
		}
		this._material.SetVector("_VolumetricLight", new Vector4(x, this.ExtinctionCoef, this._light.range, 1f));
		this._material.SetTexture("_CameraDepthTexture", renderer.GetVolumeLightDepthBuffer());
		this._material.SetFloat("_ZTest", 8f);
		if (this._light.type == LightType.Point)
		{
			this.SetupPointLight(renderer, viewProj, viewProjSP);
			return;
		}
		if (this._light.type == LightType.Spot)
		{
			this.SetupSpotLight(renderer, viewProj, viewProjSP);
		}
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x0002A3D4 File Offset: 0x000285D4
	private void SetupPointLight(EnviroSkyRendering renderer, Matrix4x4 viewProj, Matrix4x4 viewProjSP)
	{
		this._commandBuffer.Clear();
		int num = 0;
		if (!this.IsCameraInPointLightBounds())
		{
			num = 2;
		}
		this._material.SetPass(num);
		Mesh pointLightMesh = EnviroSkyRendering.GetPointLightMesh();
		float num2 = this._light.range * 2f;
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, this._light.transform.rotation, new Vector3(num2, num2, num2));
		this._material.SetMatrix("_WorldViewProj", viewProj * matrix4x);
		this._material.SetMatrix("_WorldViewProj_SP", viewProjSP * matrix4x);
		if (this.Noise)
		{
			this._material.EnableKeyword("NOISE");
		}
		else
		{
			this._material.DisableKeyword("NOISE");
		}
		this._material.SetVector("_LightPos", new Vector4(this._light.transform.position.x, this._light.transform.position.y, this._light.transform.position.z, 1f / (this._light.range * this._light.range)));
		this._material.SetColor("_LightColor", this._light.color * this._light.intensity);
		if (this._light.cookie == null)
		{
			this._material.EnableKeyword("POINT");
			this._material.DisableKeyword("POINT_COOKIE");
		}
		else
		{
			Matrix4x4 inverse = Matrix4x4.TRS(this._light.transform.position, this._light.transform.rotation, Vector3.one).inverse;
			this._material.SetMatrix("_MyLightMatrix0", inverse);
			this._material.EnableKeyword("POINT_COOKIE");
			this._material.DisableKeyword("POINT");
			this._material.SetTexture("_LightTexture0", this._light.cookie);
		}
		bool flag = false;
		if ((this._light.transform.position - EnviroSky.instance.PlayerCamera.transform.position).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if (this._light.shadows != LightShadows.None && !flag)
		{
			if (XRSettings.enabled)
			{
				if (EnviroSky.instance.singlePassVR)
				{
					this._material.EnableKeyword("SHADOWS_CUBE");
					this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
					this._commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
					this._commandBuffer.DrawMesh(pointLightMesh, matrix4x, this._material, 0, num);
					if (this.CustomRenderEvent != null)
					{
						this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
						return;
					}
				}
				else
				{
					this._material.DisableKeyword("SHADOWS_CUBE");
					renderer.GlobalCommandBuffer.DrawMesh(pointLightMesh, matrix4x, this._material, 0, num);
					if (this.CustomRenderEvent != null)
					{
						this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
						return;
					}
				}
			}
			else
			{
				this._material.EnableKeyword("SHADOWS_CUBE");
				this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
				this._commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
				this._commandBuffer.DrawMesh(pointLightMesh, matrix4x, this._material, 0, num);
				if (this.CustomRenderEvent != null)
				{
					this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
					return;
				}
			}
		}
		else
		{
			this._material.DisableKeyword("SHADOWS_DEPTH");
			if (EnviroSky.instance.PlayerCamera.actualRenderingPath == RenderingPath.Forward)
			{
				renderer.GlobalCommandBufferForward.SetRenderTarget(renderer.GetVolumeLightBuffer());
				renderer.GlobalCommandBufferForward.DrawMesh(pointLightMesh, matrix4x, this._material, 0, num);
				if (this.CustomRenderEvent != null)
				{
					this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBufferForward, viewProj);
					return;
				}
			}
			else
			{
				renderer.GlobalCommandBuffer.DrawMesh(pointLightMesh, matrix4x, this._material, 0, num);
				if (this.CustomRenderEvent != null)
				{
					this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
				}
			}
		}
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x0002A824 File Offset: 0x00028A24
	private void SetupSpotLight(EnviroSkyRendering renderer, Matrix4x4 viewProj, Matrix4x4 viewProjSP)
	{
		this._commandBuffer.Clear();
		int shaderPass = 1;
		if (!this.IsCameraInSpotLightBounds())
		{
			shaderPass = 3;
		}
		Mesh spotLightMesh = EnviroSkyRendering.GetSpotLightMesh();
		float range = this._light.range;
		float num = Mathf.Tan((this._light.spotAngle + 1f) * 0.5f * 0.017453292f) * this._light.range;
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, new Vector3(num, num, range));
		Matrix4x4 inverse = Matrix4x4.TRS(this._light.transform.position, this._light.transform.rotation, Vector3.one).inverse;
		Matrix4x4 lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1f));
		Matrix4x4 rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, 0f, 1f);
		this._material.SetMatrix("_MyLightMatrix0", lhs * rhs * inverse);
		this._material.SetMatrix("_WorldViewProj", viewProj * matrix4x);
		this._material.SetMatrix("_WorldViewProj_SP", viewProjSP * matrix4x);
		this._material.SetVector("_LightPos", new Vector4(this._light.transform.position.x, this._light.transform.position.y, this._light.transform.position.z, 1f / (this._light.range * this._light.range)));
		this._material.SetVector("_LightColor", this._light.color * this._light.intensity);
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		float value = -Vector3.Dot(position + forward * this._light.range, forward);
		this._material.SetFloat("_PlaneD", value);
		this._material.SetFloat("_CosAngle", Mathf.Cos((this._light.spotAngle + 1f) * 0.5f * 0.017453292f));
		this._material.SetVector("_ConeApex", new Vector4(position.x, position.y, position.z));
		this._material.SetVector("_ConeAxis", new Vector4(forward.x, forward.y, forward.z));
		this._material.EnableKeyword("SPOT");
		if (this.Noise)
		{
			this._material.EnableKeyword("NOISE");
		}
		else
		{
			this._material.DisableKeyword("NOISE");
		}
		if (this._light.cookie == null)
		{
			this._material.SetTexture("_LightTexture0", EnviroSkyRendering.GetDefaultSpotCookie());
		}
		else
		{
			this._material.SetTexture("_LightTexture0", this._light.cookie);
		}
		bool flag = false;
		if ((this._light.transform.position - EnviroSky.instance.PlayerCamera.transform.position).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if (this._light.shadows != LightShadows.None && !flag)
		{
			if (XRSettings.enabled)
			{
				if (EnviroSky.instance.singlePassVR)
				{
					lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
					if (this._reversedZ)
					{
						rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.range, this._light.shadowNearPlane);
					}
					else
					{
						rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.shadowNearPlane, this._light.range);
					}
					Matrix4x4 lhs2 = lhs * rhs;
					ref Matrix4x4 ptr = ref lhs2;
					ptr[0, 2] = ptr[0, 2] * -1f;
					ptr = ref lhs2;
					ptr[1, 2] = ptr[1, 2] * -1f;
					ptr = ref lhs2;
					ptr[2, 2] = ptr[2, 2] * -1f;
					ptr = ref lhs2;
					ptr[3, 2] = ptr[3, 2] * -1f;
					this._material.SetMatrix("_MyWorld2Shadow", lhs2 * inverse);
					this._material.SetMatrix("_WorldView", lhs2 * inverse);
					this._material.EnableKeyword("SHADOWS_DEPTH");
					this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
					this._commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
					this._commandBuffer.DrawMesh(spotLightMesh, matrix4x, this._material, 0, shaderPass);
					if (this.CustomRenderEvent != null)
					{
						this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
						return;
					}
				}
				else
				{
					this._material.DisableKeyword("SHADOWS_DEPTH");
					renderer.GlobalCommandBuffer.DrawMesh(spotLightMesh, matrix4x, this._material, 0, shaderPass);
					if (this.CustomRenderEvent != null)
					{
						this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
						return;
					}
				}
			}
			else
			{
				lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
				if (this._reversedZ)
				{
					rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.range, this._light.shadowNearPlane);
				}
				else
				{
					rhs = Matrix4x4.Perspective(this._light.spotAngle, 1f, this._light.shadowNearPlane, this._light.range);
				}
				Matrix4x4 lhs3 = lhs * rhs;
				ref Matrix4x4 ptr = ref lhs3;
				ptr[0, 2] = ptr[0, 2] * -1f;
				ptr = ref lhs3;
				ptr[1, 2] = ptr[1, 2] * -1f;
				ptr = ref lhs3;
				ptr[2, 2] = ptr[2, 2] * -1f;
				ptr = ref lhs3;
				ptr[3, 2] = ptr[3, 2] * -1f;
				this._material.SetMatrix("_MyWorld2Shadow", lhs3 * inverse);
				this._material.SetMatrix("_WorldView", lhs3 * inverse);
				this._material.EnableKeyword("SHADOWS_DEPTH");
				this._commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
				this._commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
				this._commandBuffer.DrawMesh(spotLightMesh, matrix4x, this._material, 0, shaderPass);
				if (this.CustomRenderEvent != null)
				{
					this.CustomRenderEvent(renderer, this, this._commandBuffer, viewProj);
					return;
				}
			}
		}
		else
		{
			this._material.DisableKeyword("SHADOWS_DEPTH");
			if (EnviroSky.instance.PlayerCamera.actualRenderingPath == RenderingPath.Forward)
			{
				renderer.GlobalCommandBufferForward.SetRenderTarget(renderer.GetVolumeLightBuffer());
				renderer.GlobalCommandBufferForward.DrawMesh(spotLightMesh, matrix4x, this._material, 0, shaderPass);
				if (this.CustomRenderEvent != null)
				{
					this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBufferForward, viewProj);
					return;
				}
			}
			else
			{
				renderer.GlobalCommandBuffer.DrawMesh(spotLightMesh, matrix4x, this._material, 0, shaderPass);
				if (this.CustomRenderEvent != null)
				{
					this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
				}
			}
		}
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x0002B040 File Offset: 0x00029240
	private bool IsCameraInPointLightBounds()
	{
		float sqrMagnitude = (this._light.transform.position - EnviroSky.instance.PlayerCamera.transform.position).sqrMagnitude;
		float num = this._light.range + 1f;
		return sqrMagnitude < num * num;
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x0002B098 File Offset: 0x00029298
	private bool IsCameraInSpotLightBounds()
	{
		float num = Vector3.Dot(this._light.transform.forward, Camera.current.transform.position - this._light.transform.position);
		float num2 = this._light.range + 1f;
		return num <= num2 && Mathf.Acos(Vector3.Dot(base.transform.forward, (Camera.current.transform.position - this._light.transform.position).normalized)) * 57.29578f <= (this._light.spotAngle + 3f) * 0.5f;
	}

	// Token: 0x04000831 RID: 2097
	private Light _light;

	// Token: 0x04000832 RID: 2098
	public Material _material;

	// Token: 0x04000833 RID: 2099
	public Shader volumeLightShader;

	// Token: 0x04000834 RID: 2100
	public Shader volumeLightBlurShader;

	// Token: 0x04000835 RID: 2101
	private CommandBuffer _commandBuffer;

	// Token: 0x04000836 RID: 2102
	private CommandBuffer _cascadeShadowCommandBuffer;

	// Token: 0x04000837 RID: 2103
	public RenderTexture temp;

	// Token: 0x04000838 RID: 2104
	[Range(1f, 64f)]
	public int SampleCount = 8;

	// Token: 0x04000839 RID: 2105
	public bool scaleWithTime = true;

	// Token: 0x0400083A RID: 2106
	[Range(0f, 1f)]
	public float ScatteringCoef = 0.5f;

	// Token: 0x0400083B RID: 2107
	[Range(0f, 0.1f)]
	public float ExtinctionCoef = 0.01f;

	// Token: 0x0400083C RID: 2108
	[Range(0f, 0.999f)]
	public float Anistropy = 0.1f;

	// Token: 0x0400083D RID: 2109
	[Header("3D Noise")]
	public bool Noise;

	// Token: 0x0400083E RID: 2110
	[HideInInspector]
	public float NoiseScale = 0.015f;

	// Token: 0x0400083F RID: 2111
	[HideInInspector]
	public float NoiseIntensity = 1f;

	// Token: 0x04000840 RID: 2112
	[HideInInspector]
	public float NoiseIntensityOffset = 0.3f;

	// Token: 0x04000841 RID: 2113
	[HideInInspector]
	public Vector2 NoiseVelocity = new Vector2(3f, 3f);

	// Token: 0x04000842 RID: 2114
	private bool _reversedZ;
}
