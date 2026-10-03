using System;
using UnityEngine;

namespace TSD.uTireRuntime
{
	// Token: 0x02000342 RID: 834
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		// Token: 0x17000205 RID: 517
		// (get) Token: 0x0600156E RID: 5486 RVA: 0x000E0610 File Offset: 0x000DE810
		public static T Instance
		{
			get
			{
				if (Singleton<T>.m_instance == null)
				{
					Singleton<T>.m_instance = (T)((object)Object.FindObjectOfType(typeof(T)));
					if (Object.FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopening the scene might fix it.");
						return Singleton<T>.m_instance;
					}
					if (Singleton<T>.m_instance == null)
					{
						GameObject gameObject = new GameObject();
						Singleton<T>.m_instance = gameObject.AddComponent<T>();
						gameObject.name = "(singleton) " + typeof(T).ToString();
					}
				}
				return Singleton<T>.m_instance;
			}
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x000E06B1 File Offset: 0x000DE8B1
		protected void Awake()
		{
			if (Singleton<T>.m_instance == null)
			{
				Singleton<T>.m_instance = (this as T);
				return;
			}
			if (Singleton<T>.m_instance != this)
			{
				Object.Destroy(this);
			}
		}

		// Token: 0x04002611 RID: 9745
		private static T m_instance;
	}
}
