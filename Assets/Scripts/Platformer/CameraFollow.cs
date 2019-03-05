using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	//	VARS

		//	PUBLIC

	public Controller2D target;
	public float VerticalOffset;
	public float lookAheadDistanceX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	public Vector2 focusAreaSize;

    public float zoomDuration;
    [Range(0,200)]
    public float zoomDistance;

		//	PRIVATE

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirectionX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;

    bool zoomedOut;

	//	GETTERS

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR

	void Start () {
  		focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
	}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z)){
            SetCameraZoom(!zoomedOut);
        }
    }

    void LateUpdate () {
        if(zoomedOut){
            return;
        }
		focusArea.Update(target.collider.bounds);
		Vector2 focusPosition = focusArea.center + Vector2.up * VerticalOffset;

		if(focusArea.velocity.x != 0) {
			lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);
			if(Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0) {
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
			}
			else{
				if(!lookAheadStopped){
					targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX)/4;
					lookAheadStopped = true;
				}
			}
		}
		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX; 
		transform.position = (Vector3)focusPosition + Vector3.forward * -30;
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color(1,0,0,0.5f);
		Gizmos.DrawCube(focusArea.center, focusAreaSize);
	}
	//	METHODS

		//	PUBLIC

    //public void TriggerZoom(){
    //    Se
    //}

    public void SetCameraZoom(bool zoomOut){
        StartCoroutine(ZoomCoroutine(zoomOut));
    }

		//	PRIVATE

    IEnumerator ZoomCoroutine(bool zoomOut){
        float elapsedTime = 0;
        zoomedOut = true;
        target.movable = false;
        Vector3 startPosition = transform.position;
        while(elapsedTime < zoomDuration){
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, startPosition + ((zoomOut) ? Vector3.back * zoomDistance : Vector3.forward * zoomDistance), elapsedTime / zoomDuration);
            yield return null;
        }
        zoomedOut = zoomOut;
        target.movable = !zoomOut;
    }

	struct FocusArea {
		public Vector2 center;
		public Vector2 velocity;
		float left, right;
		float top, bottom;

		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			center = new Vector2((left + right)/2, (top + bottom)/2);
			velocity = Vector2.zero;
		}

		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if(targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			}
			else if(targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if(targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			}
			else if(targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			bottom += shiftY;
			top += shiftY;

			center = new Vector2((left + right)/2, (top + bottom)/2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}
}