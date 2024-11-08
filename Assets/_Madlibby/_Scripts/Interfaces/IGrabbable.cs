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
	/// Anything that needs to be grabbed should implement this.
	/// </summary>
	public interface IGrabbable {

		#region MAIN CALLS
		/// <summary>
		/// The function to call when the object is grabbed.
		/// </summary>
		void OnGrabbed();
		#endregion

	}
}