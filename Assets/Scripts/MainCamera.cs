using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    public static Vector2 mousePos;
    public new static Camera camera;
    public new static Transform transform;
    public static float width;
    public static float height;

    [Header("Positioning")]
    public float z;
    public Transform camRig;
    public float moveLerpSpeed;
    public float maxDisplacementFromPlayer;

    [Header("Kick")]
    public float kickDiminishSpeed;
    [HideInInspector]
    public Vector2 kick;//knockback

    [Header("Player")]
    public Transform player;
    public float playerToMouseRatio;
    public bool isTrackingMouse = false;
    public bool isUnderPlayerControl = true;

    [Header("Audio")]
    public float masterVolume;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        transform = GetComponent<Transform>();

        if (height > 0) camera.orthographicSize = height;
        else height = camera.orthographicSize;

        width = height * camera.aspect;
    }

    private void Start()
    {
        //camRig.position = player.transform.position + Vector3.forward * z;
        AudioListener.volume = masterVolume;
    }

    /*
    private void FixedUpdate()
    {
        if (isUnderPlayerControl)
        {
            if (isTrackingMouse)
            {
                Vector2 target = Vector2.LerpUnclamped(player.transform.position, mousePos, playerToMouseRatio);
                Vector2 targetDelta = target - (Vector2)player.transform.position;

                if (targetDelta.sqrMagnitude >= maxDisplacementFromPlayer * maxDisplacementFromPlayer)
                {
                    target = (Vector2)player.transform.position + targetDelta.normalized * maxDisplacementFromPlayer;
                }

               camRig.position = (Vector3)(Vector2.Lerp(transform.position, target, Time.fixedDeltaTime * moveLerpSpeed) + kick) + Vector3.forward * z;
            }
            else
            {
               camRig.position = (Vector3)(Vector2.Lerp(transform.position, player.transform.position, Time.fixedDeltaTime * moveLerpSpeed) + kick) + Vector3.forward * z;
            }
        }
    }
    */
    private void Update()
    {
        mousePos = GetMouseWorld2DPoint();
        kick -= kick * kickDiminishSpeed * Time.deltaTime;
    }

    public void SetHeight(float value)
    {
        height = value;
        width = height * camera.aspect;
        camera.orthographicSize = height;
    }

    public void SetPosition(Vector3 position)
    {
       camRig.position = new Vector3(position.x, position.y, position.z);
    }
    public void SetPosition(Vector2 position)
    {
       camRig.position = new Vector3(position.x, position.y,camRig.position.z);
    }

    private Vector2 GetMouseWorld2DPoint()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }

    public IEnumerator HoldPosition(float duration)
    {
        isUnderPlayerControl = false;
        yield return new WaitForSeconds(duration);
        isUnderPlayerControl = true;
    }
}
