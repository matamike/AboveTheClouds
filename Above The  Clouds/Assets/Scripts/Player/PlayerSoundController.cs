using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundController : Singleton<PlayerSoundController> {
    [SerializeField] private AudioClip walkSFX;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip fallSFX;
    [SerializeField] private AudioClip landSFX;

    public enum PLAYER_SFX_TYPE {
        WALK,
        JUMP,
        FALL,
        LAND,
    }

    private Dictionary<PLAYER_SFX_TYPE, AudioClip> sfxTypeToAudioClipDictionary = new Dictionary<PLAYER_SFX_TYPE, AudioClip>() { };

    private AudioSource _audioSource;
    private void Start(){
        _audioSource = GetComponent<AudioSource>();

        //Populate Dictionary Entries
        sfxTypeToAudioClipDictionary[PLAYER_SFX_TYPE.WALK] = walkSFX;
        sfxTypeToAudioClipDictionary[PLAYER_SFX_TYPE.JUMP] = jumpSFX;
        sfxTypeToAudioClipDictionary[PLAYER_SFX_TYPE.FALL] = fallSFX;
        sfxTypeToAudioClipDictionary[PLAYER_SFX_TYPE.LAND] = landSFX;
    }

    public void ForceStopAudio(){
        if (_audioSource.isPlaying){
            _audioSource.Stop();
        }
    }

    public void PlayPlayerSFX(PLAYER_SFX_TYPE sfxType) {
        if (_audioSource.clip != sfxTypeToAudioClipDictionary[sfxType]){
            ForceStopAudio();
            SetPitch(1.0f);
            _audioSource.clip = sfxTypeToAudioClipDictionary[sfxType];
        }

        if (!_audioSource.isPlaying){
            _audioSource.Play();
        }
    }

    public void SetPitch(float pitch) => _audioSource.pitch = pitch;
}
