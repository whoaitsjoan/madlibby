using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Madlibby {
	
	/// <summary>
	/// An item that appears on the pause menu's list of selections.
	/// </summary>
	public class PauseMenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The name of the item that should be displayed.
		/// </summary>
		[SerializeField, TabGroup("Item", "Toggles"), Title("General")]
		private string itemName = "";
		/// <summary>
		/// The color for the backing image when this menu item is highlighted.
		/// </summary>
		[SerializeField, TabGroup("Item", "Toggles"), Title("Colors")]
		private Color backingHighlightColor;
		/// <summary>
		/// The color for the backing image when this menu item is dehighlighted.
		/// </summary>
		[SerializeField, TabGroup("Item", "Toggles")]
		private Color backingDehighlightColor;
		/// <summary>
		/// The color for the label when this menu item is highlighted.
		/// </summary>
		[SerializeField, TabGroup("Item", "Toggles")]
		private Color labelHighlightColor;
		/// <summary>
		/// The color for the label when this menu item is dehighlighted.
		/// </summary>
		[SerializeField, TabGroup("Item", "Toggles")]
		private Color labelDehighlightColor;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the visuals that are contained inside of this menu item.
		/// Does not contain the actual parts that make the button work (like the selectable.)
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The selectable for this menu item.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Selectable selectable;
		/// <summary>
		/// The image that shows the backing for this menu item.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image menuItemBackingImage;
		/// <summary>
		/// The label that shows the text for this item.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh menuItemLabel;
		#endregion

		#region FIELDS - UNITY EVENTS
		/// <summary>
		/// The event to run when this item is clicked/submitted.
		/// </summary>
		[SerializeField, TabGroup("Item", "Events")]
		private UnityEvent onClickAction = new UnityEvent();
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this component.
		/// </summary>
		public void ResetState() {
			// Dehighlight the menu item.
			this.Dehighlight();
			// Make the selectable uninteractable.
			this.selectable.interactable = false;
		}
		#endregion

		#region PRESENTATION - GENERAL
		/// <summary>
		/// Presents this item into view.
		/// </summary>
		public void Present() {
			// Set the interactability to be on.
			this.selectable.interactable = true;
		}
		/// <summary>
		/// Dismisses this item from view.
		/// </summary>
		public void Dismiss() {
			// Turn the interactable off.
			this.selectable.interactable = false;
			// Dehighlight the option.
			this.Dehighlight();
		}
		#endregion

		#region PRESENTATION - HIGHLIGHTING
		/// <summary>
		/// Highlights this item when it's being hovered over/selected.
		/// </summary>
		private void Highlight() {
			// Change the colors on the backing image and the label.
			this.menuItemBackingImage.color = this.backingHighlightColor;
			this.menuItemLabel.color = this.labelHighlightColor;
			// Set the label text.
			this.menuItemLabel.text = this.itemName;
		}
		/// <summary>
		/// Dehighlight this item when its not hovered over.
		/// </summary>
		private void Dehighlight() {
			// Change the colors on the backing image and the label.
			this.menuItemBackingImage.color = this.backingDehighlightColor;
			this.menuItemLabel.color = this.labelDehighlightColor;
			// Set the label text.
			this.menuItemLabel.text = this.itemName;
		}
		#endregion
		
		#region EVENT SYSTEM CALLS
		public void OnPointerClick(PointerEventData eventData) {
			Debug.Log("Item clicked: " + this.itemName);
			this.onClickAction.Invoke();
		}
		public void OnPointerEnter(PointerEventData eventData) {
			this.Highlight();
		}
		public void OnPointerExit(PointerEventData eventData) {
			this.Dehighlight();
		}
		#endregion

		
	}
	
}