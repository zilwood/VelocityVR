using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using com.wayfair;
using System;

/// <summary>
/// Sample App that ties everything together
/// </summary>
public class SampleApp : MonoBehaviour {

	public GameObject modelPlaceholder;
	public LoadingAnimator loadingAnimator;
	public GameObject shelf;
	private float counter=0;
	private WayfairAPI wayfairAPI;
	private WayfairModelLoader wayfairModelLoader;
	private List <GameObject> furnitures = new List<GameObject>();
	void Awake() {
		// Get the Wayfair components
		wayfairAPI = GetComponent<WayfairAPI> ();
		wayfairModelLoader = GetComponent<WayfairModelLoader> ();
	}

	void Start () {
		// You can get this map from the documentation
		// 1227 is Accent Pillows
		// 54 is Dining Chairs
		// 441 is Coffee Tables
		int[] classes = {0, 1227, 54, 441};

		// Add click listener to search button
//		searchButton.onClick.AddListener (delegate() {
//			wayfairAPI.GetProducts (classes[classDropdown.value], int.Parse (page.text), "", PopulateSearchResults);
//		});
		wayfairAPI.GetProducts (classes[2], 1, "", PopulateSearchResults);
	}

	/// <summary>
	/// Populates the UI with search results i.e list of products
	/// </summary>
	/// <param name="products">Products returned from API</param>
	void PopulateSearchResults(List<Product> products) {
		if (products == null) {
			// Do nothing
			return;
		}

		// Create a button for each product returned
		foreach (Product product in products) {

			if (products.IndexOf (product) < 10) {

				Product localProduct = product;
				ShowProduct (localProduct);

			} else {
				break;
			}
		}
	}

	/// <summary>
	/// Shows the product's 3D model
	/// </summary>
	/// <param name="product">Product to show</param>
	void ShowProduct(Product product) {
		counter = counter+3f;

		// Get the model info from API
		wayfairAPI.GetModelInfoForSKU (product.sku, (ProductModel productModel) => {
			// If there was an error, do nothing
			if (productModel == null) {
				return;
			}

			// Destroy the previous product model
			if (modelPlaceholder.transform.childCount > 0) {
				GameObject.Destroy (modelPlaceholder.transform.GetChild (0).gameObject);

			}

			GameObject myobject = new GameObject();
			myobject.AddComponent<BoxCollider>();
			furnitures.Add(myobject);
			myobject.transform.position = new Vector3(counter,0,0);
			// Load the model using the model loader
			wayfairModelLoader.LoadModel(productModel.url, myobject, loadingAnimator, (GameObject model) => {
				// Do nothing in callback
				//myobject.transform.position = new Vector3(counter,0,0);
				float counter2 = 0;
				Vector3 startPosition = shelf.transform.position;
				startPosition.x -= 15.0f;
				foreach (GameObject furniture in furnitures){
					furniture.transform.position=new Vector3(startPosition.x +counter2,startPosition.y,startPosition.z);
					counter2+=3f;
				}
			});
		});
	}


}
