using UnityEngine;
using com.wayfair;
using System.Collections;
using System.Net;
using System;

/// <summary>
/// Loading animator that works with the WayfairModelLoader that shows the model's download progress
/// </summary>
public class LoadingAnimator : MonoBehaviour {
	private float normalizedTargetTime = 1f;
	private static float ANIMATION_SPEED_FACTOR = 30f;
	private float normalizedCurrentTime;
	private Animator loadingAnimation;
	private string animationClip = "Loading"; 
	private GameObject loaderUI;
	private Boolean isShown = false;

	void Awake () {
		this.loadingAnimation = GetComponent<Animator> ();
		this.loaderUI = this.transform.root.gameObject;
		this.loaderUI.SetActive (false);
	}

	public void StartPlaying () {
		Reset ();
		this.loaderUI.SetActive (true);
		StartCoroutine (Play());
		isShown = true;
	}

	public void JumpTo(float percent) {
		this.normalizedTargetTime = percent;
	}

	private IEnumerator Play () {
		while (this.normalizedCurrentTime < 1) {
			this.normalizedCurrentTime = Mathf.Lerp(normalizedCurrentTime, normalizedTargetTime, Time.deltaTime * ANIMATION_SPEED_FACTOR);
			this.loadingAnimation.Play(animationClip, -1, this.normalizedCurrentTime);
			yield return new WaitForEndOfFrame ();
		}
		isShown = false;
		this.loaderUI.SetActive (false);
		yield return true;
	}

	void Reset () {
		this.normalizedCurrentTime = 0;
		this.normalizedTargetTime = 0;
		isShown = false;
	}
}
