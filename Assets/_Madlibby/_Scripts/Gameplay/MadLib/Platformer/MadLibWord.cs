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
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Madlibby {
	
	/// <summary>
	/// One of the words that is out on the field in game.
	/// </summary>
	public class MadLibWord : MonoBehaviour, IGrabbable {

		#region FIELDS - CONFIG
		/// <summary>
		/// The actual word string for this word.
		/// </summary>
		[SerializeField, TabGroup("Word", "Config")]
		private string baseWord = "";
		/// <summary>
		/// The ID type of the word.
		/// </summary>
		[FormerlySerializedAs("wordType")]
		[SerializeField, TabGroup("Word", "Config")]
		private MadLibWordIDType wordIDType = MadLibWordIDType.None;
		#endregion

		#region PROPERTIES - CONFIG
		/// <summary>
		/// The actual word string for this word.
		/// </summary>
		public string BaseWord => this.baseWord;
		/// <summary>
		/// The ID type of the word.
		/// </summary>
		public MadLibWordIDType WordIDType => this.wordIDType;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label that shows the word itself.
		/// </summary>
		[SerializeField, TabGroup("Word", "Scene References")]
		private SuperTextMesh wordLabel;
		#endregion

	
		#region GAMEPLAY - EVENTS
		/// <summary>
		/// Gets called when the word is grabbed by Libby.
		/// </summary>
		public void OnGrabbed() {
			// Send the word over to the word dictionary in the controller.
			MadLibCanvasController.Instance.UpdateWordDictionary(
				wordIDType: this.wordIDType, 
				baseWord: this.baseWord);
		}
		#endregion

		
	}
	
}