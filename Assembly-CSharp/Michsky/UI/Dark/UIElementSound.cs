using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Dark
{
	// Token: 0x02000329 RID: 809
	[RequireComponent(typeof(AudioSource))]
	public class UIElementSound : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler
	{
		// Token: 0x060014B7 RID: 5303 RVA: 0x000DBFF8 File Offset: 0x000DA1F8
		private void Start()
		{
			if (this.audioSource == null)
			{
				try
				{
					this.audioSource = base.gameObject.GetComponent<AudioSource>();
					this.audioSource.ignoreListenerPause = true;
					this.audioSource.playOnAwake = false;
				}
				catch
				{
					Debug.LogError("UI Element Sound - Cannot initalize AudioSource due to missing resources.", this);
				}
			}
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x000DC05C File Offset: 0x000DA25C
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.enableHoverSound && this.audioSource != null)
			{
				int num = Random.Range(0, this.hoverSound.Length);
				this.audioSource.PlayOneShot(this.hoverSound[num]);
			}
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x000DC0A1 File Offset: 0x000DA2A1
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.enableClickSound && this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.clickSound);
			}
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x000DC0CA File Offset: 0x000DA2CA
		public void Notification()
		{
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.notificationSound);
			}
		}

		// Token: 0x04002521 RID: 9505
		[Header("RESOURCES")]
		public AudioSource audioSource;

		// Token: 0x04002522 RID: 9506
		public AudioClip[] hoverSound;

		// Token: 0x04002523 RID: 9507
		public AudioClip clickSound;

		// Token: 0x04002524 RID: 9508
		public AudioClip notificationSound;

		// Token: 0x04002525 RID: 9509
		[Header("SETTINGS")]
		public bool enableHoverSound = true;

		// Token: 0x04002526 RID: 9510
		public bool enableClickSound = true;
	}
}
