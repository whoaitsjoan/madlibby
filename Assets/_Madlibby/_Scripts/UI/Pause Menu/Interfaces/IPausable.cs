using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Madlibby {
	
	/// <summary>
	/// Anything that needs to be paused in game should implement this.
	/// </summary>
	public interface IPausable {

		#region MAIN CALLS
		/// <summary>
		/// Gets called when this object is to be paused.
		/// </summary>
		public void OnPause();
		/// <summary>
		/// Gets called when this object is to be unpaused.
		/// </summary>
		public void OnUnpause();
		#endregion
		

	}
}