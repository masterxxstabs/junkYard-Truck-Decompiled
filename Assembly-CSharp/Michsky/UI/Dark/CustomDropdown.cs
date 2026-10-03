using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
	// Token: 0x02000311 RID: 785
	public class CustomDropdown : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
	{
		// Token: 0x06001439 RID: 5177 RVA: 0x000D98EC File Offset: 0x000D7AEC
		private void Start()
		{
			this.dropdownAnimator = base.gameObject.GetComponent<Animator>();
			this.itemList = this.itemParent.GetComponent<VerticalLayoutGroup>();
			this.SetupDropdown();
			this.currentListParent = base.transform.parent;
			if (!this.enableIcon)
			{
				this.selectedImage.gameObject.SetActive(false);
			}
			else
			{
				this.selectedImage.gameObject.SetActive(true);
			}
			if (this.enableScrollbar)
			{
				this.itemList.padding.right = 25;
				this.scrollbar.SetActive(true);
			}
			else
			{
				this.itemList.padding.right = 8;
				this.scrollbar.SetActive(false);
			}
			if (this.setHighPriorty)
			{
				base.transform.SetAsLastSibling();
			}
			if (this.saveSelected)
			{
				if (this.invokeAtStart)
				{
					this.dropdownItems[PlayerPrefs.GetInt(this.dropdownTag + "Dropdown")].OnItemSelection.Invoke();
					return;
				}
				this.ChangeDropdownInfo(PlayerPrefs.GetInt(this.dropdownTag + "Dropdown"));
			}
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x000D9A0C File Offset: 0x000D7C0C
		public void SetupDropdown()
		{
			foreach (object obj in this.itemParent)
			{
				Object.Destroy(((Transform)obj).gameObject);
			}
			this.index = 0;
			for (int i = 0; i < this.dropdownItems.Count; i++)
			{
				GameObject go = Object.Instantiate<GameObject>(this.itemObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				go.transform.SetParent(this.itemParent, false);
				this.setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
				this.textHelper = this.dropdownItems[i].itemName;
				this.setItemText.text = this.textHelper;
				Transform transform = go.gameObject.transform.Find("Icon");
				this.setItemImage = transform.GetComponent<Image>();
				this.imageHelper = this.dropdownItems[i].itemIcon;
				this.setItemImage.sprite = this.imageHelper;
				Button component = go.GetComponent<Button>();
				if (this.dropdownItems[i].OnItemSelection != null)
				{
					component.onClick.AddListener(new UnityAction(this.dropdownItems[i].OnItemSelection.Invoke));
				}
				component.onClick.AddListener(new UnityAction(this.Animate));
				component.onClick.AddListener(delegate()
				{
					this.ChangeDropdownInfo(this.index = go.transform.GetSiblingIndex());
					if (this.saveSelected)
					{
						PlayerPrefs.SetInt(this.dropdownTag + "Dropdown", go.transform.GetSiblingIndex());
					}
				});
				if (this.invokeAtStart)
				{
					this.dropdownItems[i].OnItemSelection.Invoke();
				}
			}
			this.selectedText.text = this.dropdownItems[this.selectedItemIndex].itemName;
			this.selectedImage.sprite = this.dropdownItems[this.selectedItemIndex].itemIcon;
			this.currentListParent = base.transform.parent;
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x000D9C4C File Offset: 0x000D7E4C
		public void ChangeDropdownInfo(int itemIndex)
		{
			this.selectedImage.sprite = this.dropdownItems[itemIndex].itemIcon;
			this.selectedText.text = this.dropdownItems[itemIndex].itemName;
			this.selectedItemIndex = itemIndex;
			this.dropdownItems[itemIndex].OnItemSelection.Invoke();
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x000D9CB0 File Offset: 0x000D7EB0
		public void Animate()
		{
			if (!this.isOn)
			{
				if (SceneManager.GetActiveScene().buildIndex == 1)
				{
					this.dropdownAnimator.Play("Dropdown Out");
				}
				else
				{
					this.dropdownAnimator.Play("Dropdown In");
				}
				this.isOn = true;
				if (this.isListItem)
				{
					this.siblingIndex = base.transform.GetSiblingIndex();
					base.gameObject.transform.SetParent(this.listParent, true);
				}
			}
			else if (this.isOn)
			{
				if (SceneManager.GetActiveScene().buildIndex == 1)
				{
					this.dropdownAnimator.Play("Dropdown In");
				}
				else
				{
					this.dropdownAnimator.Play("Dropdown Out");
				}
				this.isOn = false;
				if (this.isListItem)
				{
					base.gameObject.transform.SetParent(this.currentListParent, true);
					base.gameObject.transform.SetSiblingIndex(this.siblingIndex);
				}
			}
			if (this.enableTrigger && !this.isOn)
			{
				this.triggerObject.SetActive(false);
			}
			else if (this.enableTrigger && this.isOn)
			{
				this.triggerObject.SetActive(true);
			}
			if (this.outOnPointerExit)
			{
				this.triggerObject.SetActive(false);
			}
			if (this.setHighPriorty)
			{
				base.transform.SetAsLastSibling();
			}
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x000D9E0A File Offset: 0x000D800A
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.outOnPointerExit)
			{
				if (this.isOn)
				{
					this.Animate();
					this.isOn = false;
				}
				if (this.isListItem)
				{
					base.gameObject.transform.SetParent(this.currentListParent, true);
				}
			}
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x000D9E48 File Offset: 0x000D8048
		public void UpdateValues()
		{
			if (this.enableScrollbar)
			{
				if (this.itemList != null)
				{
					this.itemList.padding.right = 25;
				}
				this.scrollbar.SetActive(true);
			}
			else
			{
				this.itemList.padding.right = 8;
				this.scrollbar.SetActive(false);
			}
			if (!this.enableIcon)
			{
				this.selectedImage.gameObject.SetActive(false);
				return;
			}
			this.selectedImage.gameObject.SetActive(true);
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x000D9ED4 File Offset: 0x000D80D4
		public void CreateNewItem()
		{
			CustomDropdown.Item item = new CustomDropdown.Item();
			item.itemName = this.newItemTitle;
			item.itemIcon = this.newItemIcon;
			this.dropdownItems.Add(item);
			this.SetupDropdown();
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x000D9F14 File Offset: 0x000D8114
		public void CreateNewOption(string title)
		{
			CustomDropdown.Item item = new CustomDropdown.Item();
			item.itemName = title;
			this.dropdownItems.Add(item);
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x000D9F3A File Offset: 0x000D813A
		public void SetItemTitle(string title)
		{
			this.newItemTitle = title;
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x000D9F43 File Offset: 0x000D8143
		public void SetItemIcon(Sprite icon)
		{
			this.newItemIcon = icon;
		}

		// Token: 0x04002481 RID: 9345
		[Header("OBJECTS")]
		public GameObject triggerObject;

		// Token: 0x04002482 RID: 9346
		public TextMeshProUGUI selectedText;

		// Token: 0x04002483 RID: 9347
		public Image selectedImage;

		// Token: 0x04002484 RID: 9348
		public Transform itemParent;

		// Token: 0x04002485 RID: 9349
		public GameObject itemObject;

		// Token: 0x04002486 RID: 9350
		public GameObject scrollbar;

		// Token: 0x04002487 RID: 9351
		public Transform listParent;

		// Token: 0x04002488 RID: 9352
		private Transform currentListParent;

		// Token: 0x04002489 RID: 9353
		private VerticalLayoutGroup itemList;

		// Token: 0x0400248A RID: 9354
		[Header("SETTINGS")]
		public bool enableIcon = true;

		// Token: 0x0400248B RID: 9355
		public bool enableTrigger = true;

		// Token: 0x0400248C RID: 9356
		public bool enableScrollbar = true;

		// Token: 0x0400248D RID: 9357
		public bool setHighPriorty = true;

		// Token: 0x0400248E RID: 9358
		public bool outOnPointerExit;

		// Token: 0x0400248F RID: 9359
		public bool isListItem;

		// Token: 0x04002490 RID: 9360
		public bool invokeAtStart = true;

		// Token: 0x04002491 RID: 9361
		[Header("SAVING")]
		public bool saveSelected;

		// Token: 0x04002492 RID: 9362
		[Tooltip("Note that every Dropdown should has its own unique tag.")]
		public string dropdownTag = "Dropdown";

		// Token: 0x04002493 RID: 9363
		[Space(10f)]
		[Header("CONTENT")]
		public int selectedItemIndex;

		// Token: 0x04002494 RID: 9364
		[SerializeField]
		public List<CustomDropdown.Item> dropdownItems = new List<CustomDropdown.Item>();

		// Token: 0x04002495 RID: 9365
		[Space(10f)]
		private Animator dropdownAnimator;

		// Token: 0x04002496 RID: 9366
		private TextMeshProUGUI setItemText;

		// Token: 0x04002497 RID: 9367
		private Image setItemImage;

		// Token: 0x04002498 RID: 9368
		private Sprite imageHelper;

		// Token: 0x04002499 RID: 9369
		private string textHelper;

		// Token: 0x0400249A RID: 9370
		private string newItemTitle;

		// Token: 0x0400249B RID: 9371
		private Sprite newItemIcon;

		// Token: 0x0400249C RID: 9372
		private bool isOn;

		// Token: 0x0400249D RID: 9373
		public int index;

		// Token: 0x0400249E RID: 9374
		[HideInInspector]
		public int siblingIndex;

		// Token: 0x020004E3 RID: 1251
		[Serializable]
		public class Item
		{
			// Token: 0x04002CAD RID: 11437
			public string itemName = "Dropdown Item";

			// Token: 0x04002CAE RID: 11438
			public Sprite itemIcon;

			// Token: 0x04002CAF RID: 11439
			public UnityEvent OnItemSelection;
		}
	}
}
