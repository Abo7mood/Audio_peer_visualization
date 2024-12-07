using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoiseFlowfield))]
public class AudioFlowfield : MonoBehaviour
{
    NoiseFlowfield _noiseFlowfield;
    public AudioPeer _audioPeer;
    [Header("Speed")]
    public bool _useSpeed;
    public Vector2 _moveSpeedMinMax, _rotateSpeedMinMax;
    [Header("Scale")]
    public bool _useScale;
    public Vector2 _scaleMinMax;


    // Start is called before the first frame update
    void Start()
    {
        _noiseFlowfield = GetComponent<NoiseFlowfield>();
        int countBand = 0;
        for (int i = 0; i < _noiseFlowfield._amountOfParticles; i++)
        {
            int band = countBand % 8;
            _noiseFlowfield._particles[i]._audioBand = band;    
            countBand++;
        }
           
    }

    // Update is called once per frame
    void Update()
    {
        if (_useSpeed)
        {
            _noiseFlowfield._particleMoveSpeed = Mathf.Lerp(_moveSpeedMinMax.x, _moveSpeedMinMax.y, _audioPeer._AmplitudeBuffer);
            _noiseFlowfield._particleRotateSpeed = Mathf.Lerp(_rotateSpeedMinMax.x, _rotateSpeedMinMax.y, _audioPeer._AmplitudeBuffer);
        }
        for (int i = 0; i < _noiseFlowfield._amountOfParticles; i++)
        {
            if (_useScale)
            {
                float scale = Mathf.Lerp(_scaleMinMax.x, _scaleMinMax.y, _audioPeer._audioBandBuffer[_noiseFlowfield._particles[i]._audioBand]);
                _noiseFlowfield._particles[i].transform.localScale = new Vector3(scale, scale, scale);
            }
        }

    }
} 
