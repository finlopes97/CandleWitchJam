using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType {
        MASTER,
        MUSIC,
        SFX
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        switch (volumeType) {
            case VolumeType.MASTER:
                volumeSlider.value = AudioManager.Instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioManager.Instance.musicVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.Instance.sfxVolume;
                break;
            default:
                break;
        }
    }

    public void OnSliderValueChanged()
    {
        switch (volumeType) {
            case VolumeType.MASTER:
                AudioManager.Instance.masterVolume = volumeSlider.value;
                break;
            case VolumeType.MUSIC:
                AudioManager.Instance.musicVolume = volumeSlider.value;
                break;
            case VolumeType.SFX:
                AudioManager.Instance.sfxVolume = volumeSlider.value;
                break;
            default:
                break;
        }
    }
}
