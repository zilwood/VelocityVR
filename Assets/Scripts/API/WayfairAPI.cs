using UnityEngine;
using System.Collections;
using com.wayfair;
using System.Collections.Generic;
using System;
using LitJson;

/// <summary>
/// The Wayfair API class that helps fetch product information and models
/// using Wayfair's 3D Model API
/// 
/// TODO: Convert this to a singleton
/// </summary>
public class WayfairAPI : MonoBehaviour {

	/// <summary>
	/// The API username.
	/// </summary>
	public string apiUsername;

	/// <summary>
	/// The API key.
	/// </summary>
	public string apiKey;

	/// <summary>
	/// A loading indicator that gets activated when HTTP requests are in progress.
	/// </summary>
	public GameObject loadingIndicator;

	/// <summary>
	/// Callback delegates that are called when product / model information is ready.
	/// </summary>
	public delegate void CallbackWithModel(ProductModel model);
	public delegate void CallbackWithProducts(List<Product> products);

	private Dictionary<String, String> headers = new Dictionary<String, String> ();

	/// <summary>
	/// Loads the image from URL
	/// </summary>
	/// <returns>The image from URL.</returns>
	/// 
	/// <param name="url">URL - The URL of the image</param>
	/// <param name="image">Image - Unity Image to display the image</param>
//	public static IEnumerator LoadImageFromURL(string url, Image image) {
//		// Start a download of the given image URL
//		WWW www = new WWW(url);
//
//		// Wait for download to complete
//		yield return www;
//
//		yield return www;
//		if (www.error != null) {
//			Debug.LogWarning("Image download had an error:" + www.error);
//		}
//		else {
//			// Assign texture
//			Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
//			image.sprite = sprite;
//		}
//	}

	/// <summary>
	/// Gets products from the Wayfair 3D Model API.
	/// </summary>
	/// <param name="classId">Class identifier - See documentation on what ID to use for the different classes</param>
	/// <param name="page">Page - Page to request</param>
	/// <param name="sku">Sku - Individual SKU if you're looking for a single product</param>
	/// <param name="callback">Callback - Called when product information is ready</param>
	public void GetProducts(int classId, int page, string sku, CallbackWithProducts callback) {
		StartCoroutine (GetProducts (classId, page, sku, loadingIndicator, callback));
	}

	/// <summary>
	/// Gets the model info for SKU.
	/// </summary>
	/// <param name="sku">Sku - SKU for which you want model info</param>
	/// <param name="callback">Callback - Called when model info is ready</param>
	public void GetModelInfoForSKU(string sku, CallbackWithModel callback) {
		StartCoroutine (GetModelInfoForSKU (sku, loadingIndicator, callback));
	}

	IEnumerator GetProducts(int classId, int page, string sku, GameObject spinner, CallbackWithProducts callback){
		string url = ConstructProductInfoUrlWith(classId, page, sku);
		using(WWW www = new WWW(url, null, headers)) {			
			// Cancel WWW if it exceeds timeout
			float timeSinceDownloadStarted = -1f;
			bool requestCancelled = false;

			// Enable the loading spinner
			if (spinner != null) {
				spinner.SetActive (true);
			}
			while (!www.isDone) {
				timeSinceDownloadStarted += 1f;
				if (timeSinceDownloadStarted > Constants.TIMEOUT) {
					Debug.LogWarning ("Download timeout!");
					requestCancelled = true;
					break;
				}
				Debug.Log ("Still downloading... " + www.progress);
				yield return new WaitForSeconds (1f);
			}
			// Once download is complete
			// If request was cancelled due to a timeout
			if (requestCancelled) {
				Debug.LogWarning("Request cancelled");
				StopCoroutine ("GetProducts");
				// Disable the loading spinner
				if (spinner != null) {
					spinner.SetActive (false);
				}
				if (callback != null) {
					callback (null);
				}
				// Done
			} else {
				yield return www;
				// Disable the loading spinner
				if (spinner != null) {
					spinner.SetActive (false);
				}
				// If there was an error with the request
				if (www.error != null) {
					Debug.LogWarning ("WWW download had an error:" + www.error);
					if (callback != null) {
						callback (null);
					}
					// Done
				} else {
					string response = www.text;
					Debug.Log ("Response - " + response);
					// Wayfair API is not returning HTTP status as yet,
					// so if the response was just a string that starts with Error, there was some error
					if (response.StartsWith ("\"Error")) {
						Debug.LogWarning ("Got error - " + response);
						if (callback != null) {
							callback (null);
						}
						// Done
					} else {
						// Try to parse the JSON response
						try {
							JsonData responseJson = JsonMapper.ToObject (response);
							// If response contains an array, its probably a list of products
							if (responseJson.IsArray) {
								List<Product> products = JsonMapper.ToObject<List<Product>> (response);
								Debug.Log ("Got page - " + products);
								if (callback != null) {
									callback (products);
								}
							}
							// Otherwise its a single product
							else if (responseJson.IsObject) {
								Product product = JsonMapper.ToObject<Product> (response);
								Debug.Log ("Got product - " + product);
								if (callback != null) {
									List<Product> products = new List<Product> (); 
									products.Add (product);
									callback (products);
								}
							}
							// Done
						} catch (Exception e) {
							Debug.LogWarning ("JSON Parsing error - " + e.Message);
							if (callback != null) {
								callback (null);
							}
							// Done
						}
					}
				}
			}
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)
	}

	IEnumerator GetModelInfoForSKU (string sku, GameObject spinner, CallbackWithModel callback){
		string url = ConstructModelInfoUrlWith(sku);
		using(WWW www = new WWW(url, null, headers)) {			
			// Cancel WWW if it exceeds timeout
			float timeSinceDownloadStarted = -1f;
			bool requestCancelled = false;

			// Enable the loading spinner
			if (spinner != null) {
				spinner.SetActive (true);
			}
			while (!www.isDone) {
				timeSinceDownloadStarted += 1f;
				if (timeSinceDownloadStarted > Constants.TIMEOUT) {
					Debug.LogWarning ("Download timeout!");
					requestCancelled = true;
					break;
				}
				Debug.Log ("Still downloading... " + www.progress);
				yield return new WaitForSeconds (1f);
			}
			// Once download is complete
			// If request was cancelled due to a timeout
			if (requestCancelled) {
				Debug.LogWarning("Request cancelled");
				StopCoroutine ("GetModelInternal");
				// Disable the loading spinner
				if (spinner != null) {
					spinner.SetActive (false);
				}
				if (callback != null) {
					callback (null);
				}
				// Done
			} else {
				yield return www;
				// Disable the loading spinner
				if (spinner != null) {
					spinner.SetActive (false);
				}
				// If there was an error with the request
				if (www.error != null) {
					Debug.LogWarning ("WWW download had an error:" + www.error);
					if (callback != null) {
						callback (null);
					}
					// Done
				} else {
					string response = www.text;
					Debug.Log ("Response - " + response);
					// Wayfair API is not returning HTTP status as yet,
					// so if the response was just a string that starts with Error, there was some error
					if (response.StartsWith ("\"Error")) {
						Debug.LogWarning ("Got error - " + response);
						if (callback != null) {
							callback (null);
						}
						// Done
					} else {
						// Try to parse the JSON response
						try {
							JsonData responseJson = JsonMapper.ToObject (response);
							// Response is a JSON object with the sku as key
							if (responseJson.IsObject) {
								JsonData productModelData = responseJson[sku.ToUpper ()];
								// Convert the data to a ProductModel
								ProductModel productModel = JsonMapper.ToObject<ProductModel> (productModelData.ToJson ());
								Debug.Log ("Got model data - " + productModel);
								if (callback != null) {
									callback (productModel);
								}
							}
							// Done
						} catch (Exception e) {
							Debug.LogWarning ("JSON Parsing error - " + e.Message);
							if (callback != null) {
								callback (null);
							}
							// Done
						}
					}
				}
			}
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)
	}
		
	private string ConstructProductInfoUrlWith(int classId, int page, string sku) {
		return Constants.PRODUCT_INFO_URL + "?" +
		(classId == 0 ? "" : "class_id=" + classId + "&") +
		(page == 0 ? "" : "page=" + page + "&") +
		(sku.Length == 0 ? "" : "sku=" + sku);
	}

	private string ConstructModelInfoUrlWith(string sku) {
		return Constants.MODEL_INFO_URL + "?" +
			(sku.Length == 0 ? "" : "sku=" + sku);
	}

	void Awake () {
		// Set HTTP headers for this app
		if (apiUsername.Length == 0 || apiUsername.Length == 0) {
			Debug.LogError ("No username / key!!!");
		} else {
			headers.Add ("Accept", "application/json");
			headers.Add ("Authorization", "Basic " + System.Convert.ToBase64String (
				System.Text.Encoding.ASCII.GetBytes (apiUsername + ":" + apiKey)));
		}
	}

}
