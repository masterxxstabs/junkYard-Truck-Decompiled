using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2.Input
{
	// Token: 0x020002C4 RID: 708
	public class SteeringWheel : MonoBehaviour
	{
		// Token: 0x06001329 RID: 4905 RVA: 0x000CA222 File Offset: 0x000C8422
		private void Start()
		{
			this._rectT = this.steeringWheelGraphic.rectTransform;
			this.InitEventsSystem();
			this.UpdateRect();
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x000CA244 File Offset: 0x000C8444
		private void Update()
		{
			if (!this._wheelBeingHeld && !Mathf.Approximately(0f, this._wheelAngle))
			{
				float num = this.returnToCenterSpeed * Time.deltaTime;
				if (Mathf.Abs(num) > Mathf.Abs(this._wheelAngle))
				{
					this._wheelAngle = 0f;
				}
				else if (this._wheelAngle > 0f)
				{
					this._wheelAngle -= num;
				}
				else
				{
					this._wheelAngle += num;
				}
			}
			this._rectT.localEulerAngles = Vector3.back * this._wheelAngle;
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x000CA2E0 File Offset: 0x000C84E0
		private void UpdateRect()
		{
			Vector3[] array = new Vector3[4];
			this._rectT.GetWorldCorners(array);
			for (int i = 0; i < 4; i++)
			{
				array[i] = RectTransformUtility.WorldToScreenPoint(null, array[i]);
			}
			Vector3 vector = array[0];
			Vector3 vector2 = array[2];
			float width = vector2.x - vector.x;
			float height = vector2.y - vector.y;
			Rect rect = new Rect(vector.x, vector2.y, width, height);
			this._centerPoint = new Vector2(rect.x + rect.width * 0.5f, rect.y - rect.height * 0.5f);
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x000CA3A4 File Offset: 0x000C85A4
		public void DragEvent(BaseEventData eventData)
		{
			Vector2 position = ((PointerEventData)eventData).position;
			float num = Vector2.Angle(Vector2.up, position - this._centerPoint);
			if (Vector2.Distance(position, this._centerPoint) > 20f)
			{
				if (position.x > this._centerPoint.x)
				{
					this._wheelAngle += num - this._wheelPrevAngle;
				}
				else
				{
					this._wheelAngle -= num - this._wheelPrevAngle;
				}
			}
			this._wheelAngle = Mathf.Clamp(this._wheelAngle, -this.maximumSteeringAngle, this.maximumSteeringAngle);
			this._wheelPrevAngle = num;
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x000CA44B File Offset: 0x000C864B
		public float GetClampedValue()
		{
			return this._wheelAngle / this.maximumSteeringAngle;
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x000CA45C File Offset: 0x000C865C
		public void PressEvent(BaseEventData eventData)
		{
			Vector2 position = ((PointerEventData)eventData).position;
			this._wheelBeingHeld = true;
			this._wheelPrevAngle = Vector2.Angle(Vector2.up, position - this._centerPoint);
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x000CA498 File Offset: 0x000C8698
		public void ReleaseEvent(BaseEventData eventData)
		{
			this.DragEvent(eventData);
			this._wheelBeingHeld = false;
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x000CA4A8 File Offset: 0x000C86A8
		private void InitEventsSystem()
		{
			EventTrigger eventTrigger = this.steeringWheelGraphic.gameObject.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = this.steeringWheelGraphic.gameObject.AddComponent<EventTrigger>();
			}
			if (eventTrigger.triggers == null)
			{
				eventTrigger.triggers = new List<EventTrigger.Entry>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
			UnityAction<BaseEventData> call = new UnityAction<BaseEventData>(this.PressEvent);
			triggerEvent.AddListener(call);
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback = triggerEvent;
			eventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			triggerEvent = new EventTrigger.TriggerEvent();
			call = new UnityAction<BaseEventData>(this.DragEvent);
			triggerEvent.AddListener(call);
			entry.eventID = EventTriggerType.Drag;
			entry.callback = triggerEvent;
			eventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			triggerEvent = new EventTrigger.TriggerEvent();
			call = new UnityAction<BaseEventData>(this.ReleaseEvent);
			triggerEvent.AddListener(call);
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback = triggerEvent;
			eventTrigger.triggers.Add(entry);
		}

		// Token: 0x04002399 RID: 9113
		[Tooltip("    Maximum angle that the steering wheel can be turned to towards either side in degrees.")]
		public float maximumSteeringAngle = 200f;

		// Token: 0x0400239A RID: 9114
		[Tooltip("    Speed at which wheel is returned to center in degrees per second.")]
		public float returnToCenterSpeed = 400f;

		// Token: 0x0400239B RID: 9115
		public Graphic steeringWheelGraphic;

		// Token: 0x0400239C RID: 9116
		private Vector2 _centerPoint;

		// Token: 0x0400239D RID: 9117
		private RectTransform _rectT;

		// Token: 0x0400239E RID: 9118
		private float _wheelAngle;

		// Token: 0x0400239F RID: 9119
		private bool _wheelBeingHeld;

		// Token: 0x040023A0 RID: 9120
		private float _wheelPrevAngle;
	}
}
