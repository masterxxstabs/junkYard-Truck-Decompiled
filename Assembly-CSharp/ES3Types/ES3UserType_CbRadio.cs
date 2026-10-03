using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ES3Types
{
	// Token: 0x020002D6 RID: 726
	[Preserve]
	[ES3Properties(new string[]
	{
		"aSource",
		"clip",
		"interactor",
		"renderer",
		"enabled"
	})]
	public class ES3UserType_CbRadio : ES3ComponentType
	{
		// Token: 0x060013A0 RID: 5024 RVA: 0x000CC904 File Offset: 0x000CAB04
		public ES3UserType_CbRadio() : base(typeof(CbRadio))
		{
			ES3UserType_CbRadio.Instance = this;
			this.priority = 1;
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x000CC924 File Offset: 0x000CAB24
		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			CbRadio cbRadio = (CbRadio)obj;
			writer.WritePropertyByRef("aSource", cbRadio.aSource);
			writer.WriteProperty("clip", cbRadio.clip, ES3Type_AudioClipArray.Instance);
			writer.WritePropertyByRef("interactor", cbRadio.interactor);
			writer.WritePropertyByRef("renderer", cbRadio.renderer);
			writer.WriteProperty("enabled", cbRadio.enabled, ES3Type_bool.Instance);
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x000CC99C File Offset: 0x000CAB9C
		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			CbRadio cbRadio = (CbRadio)obj;
			foreach (object obj2 in reader.Properties)
			{
				string a = (string)obj2;
				if (!(a == "aSource"))
				{
					if (!(a == "clip"))
					{
						if (!(a == "interactor"))
						{
							if (!(a == "renderer"))
							{
								if (!(a == "enabled"))
								{
									reader.Skip();
								}
								else
								{
									cbRadio.enabled = reader.Read<bool>(ES3Type_bool.Instance);
								}
							}
							else
							{
								cbRadio.renderer = reader.Read<Renderer>();
							}
						}
						else
						{
							cbRadio.interactor = reader.Read<Interactor>(ES3UserType_Interactor.Instance);
						}
					}
					else
					{
						cbRadio.clip = reader.Read<AudioClip[]>(ES3Type_AudioClipArray.Instance);
					}
				}
				else
				{
					cbRadio.aSource = reader.Read<AudioSource>();
				}
			}
		}

		// Token: 0x04002442 RID: 9282
		public static ES3Type Instance;
	}
}
