using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Runtime.Remoting.Messaging;

namespace com.wayfair {
	/// <summary>
	/// Wayfair Product 3D Model
	/// </summary>
	public class ProductModel {
		/// <summary>
		/// The URL of the Android Unity asset bundle.
		/// </summary>
		public string android;

		/// <summary>
		/// The URL of the Web GL Unity asset bundle.
		/// </summary>
		public string web;

		/// <summary>
		/// The URL of the Windows Unity asset bundle.
		/// </summary>
		public string win;

		/// <summary>
		/// Gets the URL to download for this model based on the current platform the app is running on.
		/// </summary>
		/// <value>The URL.</value>
		public string url {
			get {
				if (Application.platform == RuntimePlatform.WindowsPlayer) {
					return win;
				} else if (Application.platform == RuntimePlatform.WebGLPlayer) {
					return web;
				} else {
					return android;
				}
			}
		}
	}
}
