using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    private Camera _camera;

    private Slider _positionSlider;
    private GameObject _positionSliderObject;

    private Slider _sizeSlider;
    private GameObject _sizeSliderObject;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        _positionSliderObject = GameObject.Find("CameraPositionSlider");
        _positionSlider = _positionSliderObject.GetComponent<Slider>();
        _positionSlider.onValueChanged.AddListener(delegate { SetCameraPosition(); });

        _sizeSliderObject = GameObject.Find("CameraSizeSlider");
        _sizeSlider = _sizeSliderObject.GetComponent<Slider>();
        _sizeSlider.onValueChanged.AddListener(delegate {SetCameraSize();});
    }

    public void SetCameraPosition()
    {
        transform.position = new Vector3(gameObject.transform.position.x, _positionSlider.value, gameObject.transform.position.z);
    }

    public void SetCameraSize()
    {
        _camera.orthographicSize = _sizeSlider.value;
    }
}