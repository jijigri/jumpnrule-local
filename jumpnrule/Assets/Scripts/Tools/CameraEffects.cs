using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private CinemachineVirtualCamera _cam;
    private CinemachineBasicMultiChannelPerlin _noise;
    private PixelPerfectCamera _pixelPerfectCamera;
    private ObjectPooler _pooler;

    float _shakeTotalTime;
    float _shakeIntensity;
    float _shakeFrequency;
    float _shakeTimer;
    bool _isFadingOut = false;

    private void Start()
    {
        if (_cam == null)
        {
            _cam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

            if (_noise == null && _cam != null)
            {
                _noise = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        if(_pixelPerfectCamera == null)
        {
            _pixelPerfectCamera = Camera.main.transform.GetComponent<PixelPerfectCamera>();
        }

        _pooler = ObjectPooler.Instance;
    }

    private void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            if (_isFadingOut)
            {
                _noise.m_AmplitudeGain = Mathf.Lerp(_shakeIntensity, 0f, _shakeTimer / _shakeTotalTime);
            }
            else
            {
                _noise.m_AmplitudeGain = _shakeIntensity;
            }
            if (_shakeTimer <= 0)
            {
                _noise.m_AmplitudeGain = 0;
            }
        }
    }

    public void Shake(float time = .2f, float strength = 1f, bool fadeOut = false, float frequency = 5f)
    {
        if (_noise == null)
            return;

        _shakeIntensity = strength;
        _shakeFrequency = frequency;
        _shakeTotalTime = time;
        _isFadingOut = fadeOut;

        _noise.m_FrequencyGain = _shakeFrequency;

        _shakeTimer = time;
    }

    public void Hitstop(float time = .1f)
    {
        StartCoroutine(PlayHitstop(time));
    }

    IEnumerator PlayHitstop(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }

    public void ZoomIn(float zoomStrength, float time)
    {
        //cam.m_Lens.OrthographicSize = 2;
        StopCoroutine(Zoom(zoomStrength, time));
        StartCoroutine(Zoom(zoomStrength, time));
    }

    IEnumerator Zoom(float strength, float time)
    {
        time /= 2;
        float timer = 0;

        float _defaultCameraZoom = _cam.m_Lens.OrthographicSize;

        while (timer < time)
        {
            _cam.m_Lens.OrthographicSize = Mathf.Lerp(_defaultCameraZoom, _defaultCameraZoom - strength, timer / time);

            timer += Time.deltaTime;

            yield return null;
        }

        timer = 0;

        while (timer < time)
        {
            _cam.m_Lens.OrthographicSize = Mathf.Lerp(_defaultCameraZoom - strength, _defaultCameraZoom, timer / time);

            timer += Time.deltaTime;

            yield return null;
        }

        _cam.m_Lens.OrthographicSize = _defaultCameraZoom;

        yield break;
    }

    public void Shockwave(Vector2 position, float size, float ringSize, float magnitude, float speed, float smoothness = 1f)
    {
        ShockwaveEffect shockwave = _pooler.SpawnObject("Shockwave", position, Quaternion.identity).GetComponent<ShockwaveEffect>();
        shockwave.Initialize(size, ringSize, magnitude, speed, smoothness);
    }

    Vector3 _defaultCameraPosition = Vector3.zero;
    Quaternion _defaultCameraRotation = Quaternion.identity;

    bool _isRecoiling = false;

    public void Recoil(Vector2 direction, float inTime, float outTime, float rotation = 0f)
    {
        if (_isRecoiling)
        {
            //_cam.ForceCameraPosition(_defaultCameraPosition, _defaultCameraRotation);

            StopCoroutine(CRT_Recoil(direction, inTime, outTime, rotation));

            _isRecoiling = false;
        }

        StartCoroutine(CRT_Recoil(direction, inTime, outTime, rotation));
    }

    IEnumerator CRT_Recoil(Vector2 direction, float inTime, float outTime, float rotation = 0f)
    {
        _isRecoiling = true;

        float elapsedTime = 0;

        Vector3 defaultCameraPosition = _cam.transform.position;
        Vector3 targetPosition = defaultCameraPosition + (Vector3)direction;

        Quaternion defaultCameraRotation = _cam.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(defaultCameraRotation.x, defaultCameraRotation.y, defaultCameraRotation.z + rotation);

        _defaultCameraPosition = defaultCameraPosition;
        _defaultCameraRotation = defaultCameraRotation;

        while (elapsedTime < inTime)
        {
            Vector3 offsetPosition = Vector3.Lerp(defaultCameraPosition, targetPosition, elapsedTime / inTime);
            offsetPosition.z = -10;
            _cam.ForceCameraPosition(offsetPosition, Quaternion.Lerp(defaultCameraRotation, targetRotation, elapsedTime / inTime));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        elapsedTime = 0;

        while (elapsedTime < outTime)
        {
            Vector3 offsetPosition = Vector3.Lerp(targetPosition, defaultCameraPosition, elapsedTime / inTime);
            offsetPosition.z = -10;
            _cam.ForceCameraPosition(offsetPosition, Quaternion.Lerp(targetRotation, defaultCameraRotation, elapsedTime / outTime));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _isRecoiling = false;

        yield break;
    }
}
