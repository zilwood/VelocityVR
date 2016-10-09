using System;
using UnityEngine;
using System.Collections;
using com.wayfair;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.IO;

namespace com.wayfair {
	/// <summary>
	/// Wayfair model loader - Loads and displays models from Wayfair over the network
	/// </summary>
	public class WayfairModelLoader : MonoBehaviour {
		/// <summary>
		/// Regexes for detecting versions of meshes on the models. Used for Shader assignment
		/// </summary>
	    static Regex GLASS_MESH_NAME_V1 = new Regex(@"^clear_obj_\d+_([0-9]*\.?[0-9]+)_(v1|v5|1).*");
	    static Regex OPAQUE_MESH_NAME_V1 = new Regex(@"^not_clear_obj_\d+_(v1|v5|1).*");
	    static Regex SHADOW_MESH_NAME_V5 = new Regex(@"^shadow_plane_v5.*");

		/// <summary>
		/// Shaders used to render Wayfair's models
		/// </summary>
	    static string WAYFAIR_SHADER_GLASS = "Wayfair/Glass";
	    public static string WAYFAIR_SHADER_SPECULAR = "Wayfair/StandardSpecular";
	    public static string SHADOW_PLANE_SHADER = "Mobile/Particles/Multiply";
	    public static string GLASS_MAT_SUFFIX = "_Glass";

		/// <summary>
		/// Cache flag - Set this to false if you want models to be cached over sessions
		/// </summary>
	    public bool cleanCache = true;

		/// <summary>
		/// Callback to be called when model is done loading
		/// </summary>
		public delegate void PostLoadCallback(GameObject model);

		/// <summary>
		/// Loads a model from Wayfair and adds it to scene
		/// </summary>
		/// <param name="modelUrl">Model URL - The URL of the model file</param>
		/// <param name="attachTo">Attach to - The GameObject you want to attach this model to in your scene</param>
		/// <param name="loadingAnimator">Loading animator - The loading animator to show download progress of the model</param>
		/// <param name="callback">Callback - Callback that is called when model loading is done</param>
	    public void LoadModel(string modelUrl, GameObject attachTo, LoadingAnimator loadingAnimator, PostLoadCallback callback) {
			StartCoroutine(DownloadAndCacheModel(modelUrl, attachTo, loadingAnimator, callback));
		}

		IEnumerator DownloadAndCacheModel (string modelUrl, GameObject attachTo, LoadingAnimator loadingAnimator, PostLoadCallback callback){
			// Wait for the Caching system to be ready
			while (!Caching.ready) {
				yield return null;
			}

			// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
			Debug.Log ("Downloading - " + modelUrl);
			using(WWW www = WWW.LoadFromCacheOrDownload (modelUrl, 1)) {
				// Cancel WWW if it exceeds timeout
				float downloadStartTime = Time.realtimeSinceStartup;
				bool requestCancelled = false;

				// Start animation
				if (loadingAnimator != null) {
					loadingAnimator.StartPlaying ();
					loadingAnimator.JumpTo (0.0f);
				}
				while (!www.isDone) {
					float elapsedTime = Time.realtimeSinceStartup - downloadStartTime;
					if (elapsedTime == 0) {
						elapsedTime = 0.1f;
					}
					if (elapsedTime > Constants.TIMEOUT) {
						Debug.LogWarning ("Model download timeout!");
						requestCancelled = true;
						break;
					}
					Debug.Log ("Still downloading... " + www.progress + "," + elapsedTime);
					// Animate loading
					if (loadingAnimator != null) {
						loadingAnimator.JumpTo (www.progress);
					}
					yield return new WaitForEndOfFrame();
				}

				// Once download is complete
				if (requestCancelled) {
					Debug.LogWarning ("Request cancelled");
					StopCoroutine ("DownloadAndCacheAsset");
					// Do nothing
				} else {
					yield return www;
					if (www.error != null) {
						Debug.LogWarning ("WWW download had an error:" + modelUrl + ":" + www.error);
						if (callback != null) {
							callback (null);
						}
					} else {
						AssetBundle bundle = www.assetBundle;
						Debug.Log("Downloaded bundle - " + modelUrl);

						// Load the object asynchronously
						AssetBundleRequest request = bundle.LoadAssetAsync(bundle.GetAllAssetNames()[0], typeof(GameObject));

						// Wait for completion
						yield return request;

						// Get the reference to the loaded object
						GameObject prefab = request.asset as GameObject;

						GameObject model = (GameObject.Instantiate(prefab));
						model.transform.SetParent(attachTo.transform);
						model.transform.rotation = attachTo.transform.rotation;

						attachTo.transform.position = new Vector3(0, 0, 0);
						model.transform.position = new Vector3(0, 0, 0);
						attachTo.transform.localPosition = new Vector3(0, 0, 0);
						model.transform.localPosition = new Vector3(0, 0, 0);

						// Need to rotate all models by 180.
						model.transform.Rotate(0, 180, 0);

						BoxCollider collider = model.GetComponent<BoxCollider>();
						BoxCollider newCollider = attachTo.AddComponent<BoxCollider>();
						newCollider.center = collider.center;
						newCollider.size = collider.size;
						GameObject.Destroy(collider);

						// Re-assign shaders to materials found on the model
						Shader glassShader = Shader.Find(WAYFAIR_SHADER_GLASS);
						Shader specularShader = Shader.Find(WAYFAIR_SHADER_SPECULAR);
						Renderer renderer;
						// Go through all sub meshes
						if (model.transform.childCount > 0) {
							for (int i = 0; i < model.transform.childCount; i++) {
								GameObject subModel = model.transform.GetChild(i).gameObject;
								renderer = subModel.GetComponent<Renderer>() as Renderer;
								if (renderer) {
									if (GLASS_MESH_NAME_V1.Match(renderer.name).Success) {
										renderer.sharedMaterial.shader = glassShader;
									}
									else if (OPAQUE_MESH_NAME_V1.Match(renderer.name).Success) {
										renderer.sharedMaterial.shader = specularShader;
									}
									else if (SHADOW_MESH_NAME_V5.Match(renderer.name).Success) {
										renderer.sharedMaterial.shader = Shader.Find(SHADOW_PLANE_SHADER);
									}
								}
								// Go one level deeper since Unity may have broken down meshes > 65k
								else {
									for (int j = 0; j < subModel.transform.childCount; j++) {
										renderer = subModel.transform.GetChild(j).gameObject.GetComponent<Renderer>() as Renderer;
										if (renderer) {
											if (GLASS_MESH_NAME_V1.Match(renderer.name).Success) {
												renderer.sharedMaterial.shader = glassShader;
												foreach (Material material in renderer.materials) {
													material.shader = glassShader;
												}
											}
											else if (OPAQUE_MESH_NAME_V1.Match(renderer.name).Success) {
												renderer.sharedMaterial.shader = specularShader;
											}
										}
									}
								}
							}
						}

						if (loadingAnimator != null) {
							loadingAnimator.JumpTo(1); // Update the animation to the last frame
						}
						if (callback != null) {
							callback(attachTo);
						}

						// Unload the AssetBundles compressed contents to conserve memory
						bundle.Unload(false);
					}
				}
			} // memory is freed from the web stream (www.Dispose() gets called implicitly)
		}
			
		public void Start() {
			if (this.cleanCache) {
				Caching.CleanCache ();
			}
		}
	}
}
