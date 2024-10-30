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
	/// Provides high level functionality to the Mad Lib parts of the levels.
	/// </summary>
	public class MadLibCanvasController : MonoBehaviour {
		
		public static MadLibCanvasController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The dictionary holding the words that are currently part of the dictionary.
		/// </summary>
		private Dictionary<MadLibWordIDType, string> CurrentWordDict { get; set; } = new Dictionary<MadLibWordIDType, string>();
		#endregion
		
		#region FIELDS - CONFIG
		/// <summary>
		/// The base sentence that will be manipulated during gameplay.
		/// </summary>
		[SerializeField, TabGroup("Canvas", "Config")]
		private string baseSentence = "I feel {Word1} when I'm with {Word2}!!";
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label that actually displays the sentence itself.
		/// </summary>
		[SerializeField, TabGroup("Canvas", "Scene References")]
		private SuperTextMesh madLibSentenceLabel;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			this.RebuildSentenceLabel();
		}
		#endregion

		#region STATE CHECKS
		/// <summary>
		/// Checks whether or not a word is stored in the dictionary for the provided ID type.
		/// </summary>
		/// <param name="wordIDType">The ID type of the word to check for.</param>
		/// <returns></returns>
		private bool HasWord(MadLibWordIDType wordIDType) {
			bool hasWord = this.CurrentWordDict.ContainsKey(wordIDType);
			return hasWord;
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			// Clear out the dictionary.
			this.CurrentWordDict = new Dictionary<MadLibWordIDType, string>();
			// Clear out the text.
			this.madLibSentenceLabel.text = "";
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Rebuilds the label on the sentence.
		/// </summary>
		public void RebuildSentenceLabel() {
			this.madLibSentenceLabel.text = this.RewriteSentence();
		}
		#endregion
		
		#region HELPERS
		/// <summary>
		/// Determines what the new sentence should be based on the current state of the dictionary.
		/// </summary>
		private string RewriteSentence() {
			string sentence = baseSentence;
			// Identify the placeholders in the base sentence
			var placeholders = System.Text.RegularExpressions.Regex.Matches(baseSentence, @"\{(\w+)\}");
			foreach (System.Text.RegularExpressions.Match placeholder in placeholders) {
				string keyString = placeholder.Groups[1].Value;
				if (Enum.TryParse<MadLibWordIDType>(keyString, out var key)) {
					string replacement;
					if (CurrentWordDict.TryGetValue(key, out var value)) {
						replacement = value;
					} else {
						replacement = new string('_', keyString.Length);
					}
					sentence = sentence.Replace(placeholder.Value, replacement);
				}
			}
			this.madLibSentenceLabel.text = sentence;
			return sentence;
		}
		#endregion
		
	}

}