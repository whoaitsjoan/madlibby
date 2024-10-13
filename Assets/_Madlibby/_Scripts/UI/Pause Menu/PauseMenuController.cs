using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Madlibby {
	
	/// <summary>
	/// Manages the pause menu.
	/// </summary>
	public class PauseMenuController : MonoBehaviour {
		
		public static PauseMenuController Instance { get; private set; }

		#region FIELDS - STATE : FLAGS
		/// <summary>
		/// Is the game currently paused?
		/// </summary>
		public bool IsPaused { get; private set; } = false;
		#endregion
		
		#region FIELDS - STATE : DATA
		/// <summary>
		/// A list of pausables that have been registered that aren't already in scenePausables.
		/// </summary>
		private List<IPausable> allPausables = new List<IPausable>();
		#endregion
		
		#region FIELDS - CANVAS
		/// <summary>
		/// Contains all other objects as children.
		/// </summary>
		[SerializeField, TabGroup("Pause", "Canvas")]
		private GameObject allObjects;
		/// <summary>
		/// The panel that should be displayed for the prototype version of the pause menu.
		/// </summary>
		[SerializeField, TabGroup("Pause", "Canvas")]
		private RectTransform prototypePanel;
		/// <summary>
		/// The pause menu items to show in the menu.
		/// </summary>
		[SerializeField, TabGroup("Pause", "Canvas")]
		private List<PauseMenuItem> pauseMenuItems = new List<PauseMenuItem>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			this.ResetState();
		}
		private void Update() {
			// TODO: INTEGRATE THIS BETTER WITH WHATEVER INPUT SYSTEM IS BEING USED.
			bool inputRecieved = Input.GetKeyDown(KeyCode.P);
			if (inputRecieved == true && this.IsPaused == false) {
				this.PauseGame();
			} else if (inputRecieved == true && this.IsPaused == true) {
				this.UnpauseGame();
			}
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the pause menu.
		/// </summary>
		private void ResetState() {
			// Flag the game as unpaused.
			this.IsPaused = false;
			
			// Reset the state of the other pause menu items.
			foreach (PauseMenuItem menuItem in this.pauseMenuItems) {
				menuItem.ResetState();
			}
			
			// Snap the prototype panel to be invisible.
			this.prototypePanel.DOKill(complete: true);
			this.prototypePanel.localScale = Vector3.zero;
			
		}
		/// <summary>
		/// Registers a pausable into the list of pausables.
		/// </summary>
		/// <param name="pausable">The pausable to add to the list.</param>
		public void RegisterPausable(IPausable pausable) {
			// The list should not already have the specified pausable in the list.
			Debug.Assert(this.allPausables.Contains(pausable) == false);
			this.allPausables.Add(pausable);
		}
		/// <summary>
		/// Deregisteres a pausable from the list of pausables.
		/// </summary>
		/// <param name="pausable">The pausable to remove from the list.</param>
		public void DeregisterPausable(IPausable pausable) {
			if (this.allPausables.Contains(pausable) == false) {
				Debug.LogWarning("The specified pausable was not included in the list of registered pausables!");
			} else {
				this.allPausables.Remove(pausable);
			}
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the pause menu into view.
		/// </summary>
		private void PresentMenu() {
			
			// Kill the prototype panel and snap it to its default scale.
			this.prototypePanel.DOKill(complete: true);
			this.prototypePanel.localScale = Vector3.zero;
			
			// Tween the prototype panel in.
			this.prototypePanel
				.DOScale(endValue: 1f, duration: 0.5f)
				.SetEase(Ease.OutElastic);
			
			// Present each of the menu items.
			foreach (PauseMenuItem menuItem in this.pauseMenuItems) {
				menuItem.Present();
			}
			
		}
		/// <summary>
		/// Dismisses the menu from view.
		/// </summary>
		private void DismissMenu() {
			
			// Kill the prototype panel and snap it to its visible scale.
			this.prototypePanel.DOKill(complete: true);
			this.prototypePanel.localScale = Vector3.one;
			
			// Tween the prototype panel in.
			this.prototypePanel
				.DOScale(endValue: 0f, duration: 0.5f)
				.SetEase(Ease.InCirc);
			
			// Dismiss each of the menu items.
			foreach (PauseMenuItem menuItem in this.pauseMenuItems) {
				menuItem.Dismiss();
			}
			
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Pauses the game and tells all of the IPausable's to pause.
		/// </summary>
		public void PauseGame() {
			// Cascade down using the list of registered pausables.
			this.PauseGame(pausables: this.allPausables);
		}
		/// <summary>
		/// Pauses the game and tells the specified list of IPausables to pause.
		/// </summary>
		/// <param name="pausables">The pausables to pause.</param>
		private void PauseGame(List<IPausable> pausables) {
			
			// Mark the game as paused.
			this.IsPaused = true;
			
			// Present the menu.
			this.PresentMenu();
			
			// Iterate through each pausable and pause it.
			foreach (IPausable pausable in pausables) {
				pausable.OnPause();
			}
			
		}
		/// <summary>
		/// Unpauses the game and tells all of the IPausable's to unpause.
		/// </summary>
		public void UnpauseGame() {
			// Cascade down using the list of registered pausables.
			this.UnpauseGame(pausables: this.allPausables);
		}
		/// <summary>
		/// Unpauses the game and tells the specified list of IPausables to unpause.
		/// </summary>
		/// <param name="pausables">The pausables to unpause.</param>
		private void UnpauseGame(List<IPausable> pausables) {
			
			// Mark the game as unpaused.
			this.IsPaused = false;
			
			// Dismiss the menu.
			this.DismissMenu();
			
			// Iterate through each pausable and pause it.
			foreach (IPausable pausable in pausables) {
				pausable.OnUnpause();
			}
			
		}
		#endregion

		#region PAUSE MENU ITEM EVENTS
		/// <summary>
		/// Gets called when Resume is clicked/submitted in the menu.
		/// </summary>
		public void OnResumeClicked() {
			this.UnpauseGame();
		}
		/// <summary>
		/// Gets called when Settings is clicked/submitted in the menu.
		/// </summary>
		public void OnSettingsClicked() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Gets called when Quit To Title is clicked/submitted in the menu.
		/// </summary>
		public void OnQuitToTitleClicked() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Gets called when Exit Game is clicked/submitted in the menu.
		/// </summary>
		public void OnExitGameClicked() {
			Application.Quit();
		}
		#endregion
		
	}
}