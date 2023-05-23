using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public EventController eventController;
    private RectTransform _rectTransform;
    private Transform followTransform;
    private Camera minimapCamera;
    private RenderTexture minimapRenderTexture;
    private RawImage minimapRawImage;

    private void Awake()
    {
    }

    void Start () {
       
        StartMinimap(eventController,eventController.MainPlayer.transform);
    }
    public void StartMinimap(EventController ec, Transform transformm)
    {
        eventController = ec;
        followTransform = transformm;
        followTransform = transformm;
        _rectTransform = gameObject.GetOrAddComponent<RectTransform>();
        // Create the minimapCamera
        minimapCamera = new GameObject("MinimapCamera").AddComponent<Camera>();
        minimapCamera.orthographic = true;
        minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        minimapCamera.orthographicSize = 9.0f;
        minimapCamera.transform.position = new Vector3(0f, 80f, 0f);

        minimapCamera.transform.SetParent(transform);


        // Create the minimapRenderTexture
        minimapRenderTexture = new RenderTexture((int)_rectTransform.rect.width, (int)_rectTransform.rect.height, 0);
        minimapRenderTexture.Create();

        // Set the camera to render in the minimapRenderTexture
        minimapCamera.targetTexture = minimapRenderTexture;

        // Create the minimapRawImage
        minimapRawImage = gameObject.GetComponent<RawImage>();
        minimapRawImage.texture = minimapRenderTexture;
    }


    void LateUpdate () {
        // Make the minimapCamera follow the followTransform
        minimapCamera.transform.position = new Vector3(followTransform.position.x, minimapCamera.transform.position.y, followTransform.position.z);
        minimapCamera.transform.rotation = Quaternion.Euler(90f, followTransform.eulerAngles.y, 0f);
    }
}