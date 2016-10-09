using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.wayfair {
	/// <summary>
	/// A Wayfair product
	/// </summary>
	public class Product {
		/// <summary>
		/// The sku.
		/// </summary>
		public string sku;

		/// <summary>
		/// The name of the product.
		/// </summary>
		public string product_name;

		/// <summary>
		/// The sale price.
		/// </summary>
		public float sale_price;

		/// <summary>
		/// The product description.
		/// </summary>
		public string product_description;

		/// <summary>
		/// The image URL.
		/// </summary>
		public string image_url;

		/// <summary>
		/// The name of the class that this product belongs to.
		/// </summary>
		public string class_name;

		/// <summary>
		/// The product description page on Wayfair.com .
		/// </summary>
		public string pdp;

		/// <summary>
		/// Product fixture - Helps determine where this product can be placed
		/// </summary>
		public enum ProductFixture {
			FLOOR = 0x00,
			WALL = 0x01,
			CEILING = 0x10
		}

		/// <summary>
		/// Gets or sets the type of the fixture for this product.
		/// </summary>
		/// <value>The type of the fixture.</value>
		public ProductFixture fixtureType { get; set; }

		public static Dictionary<int, bool> CEILING_FIXTURE_CLASSES = new Dictionary<int, bool>() {
			{ 6087, true}, // Pendant Lights
			{ 6085, true}, // Chandeliers
			{ 6086, true}, // Flush Mount
			{ 230, true}, // Ceiling fans
			{ 6111, true} // Outdoor hanging lights
		};

		public static Dictionary<int, bool> WALL_FIXTURE_CLASSES = new Dictionary<int, bool>() {
			{ 6117, true}, // Wall Sconces
			{ 6116, true}, // Vanity Lighting
			{ 6114, true}, // Outfoor flush mounts and wall lights
			{ 646, true}, // Picture Frames
			{ 66, true}, // Coat racks and hooks
			{ 1563, true} // Wall Clocks
		};

		/// <summary>
		/// Gets or sets the class identifier for this product.
		/// </summary>
		/// <value>The class identifier.</value>
		public int class_id {
			get {
				return class_id;
			}

			set {
				if (CEILING_FIXTURE_CLASSES.ContainsKey (value)) {
					this.fixtureType = ProductFixture.CEILING;
				} else if (WALL_FIXTURE_CLASSES.ContainsKey (value)) {
					this.fixtureType = ProductFixture.WALL;
				} else {
					this.fixtureType = ProductFixture.FLOOR;
				}
			}
		}
	}
}
