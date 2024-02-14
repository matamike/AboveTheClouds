using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class TileAudio : MonoBehaviour{
    [SerializeField] private AudioClip sfxInteraction;
    [SerializeField] private AudioClip sfxActivation;
    private AudioSource tileAudioSource;

    public enum TILE_SFX_TYPE{
        Interaction,
        Activation,
    }

    private Dictionary<TILE_SFX_TYPE, AudioClip> sfxTypeToAudioClipDictionary = new Dictionary<TILE_SFX_TYPE, AudioClip>() { };

    private void Awake(){
        tileAudioSource = GetComponent<AudioSource>();
    }

    private void Start(){
        sfxTypeToAudioClipDictionary[TILE_SFX_TYPE.Interaction] = sfxInteraction;
        sfxTypeToAudioClipDictionary[TILE_SFX_TYPE.Activation] = sfxActivation;
    }

    public void PlayTileSFX(TILE_SFX_TYPE sfxType, bool loop = false){
        if (sfxTypeToAudioClipDictionary[sfxType] is null) return;

        StopTileSFX();
        tileAudioSource.clip = sfxTypeToAudioClipDictionary[sfxType];
        tileAudioSource.loop = loop;
        tileAudioSource.Play();
    }

    public bool IsPlaying() => tileAudioSource.isPlaying;

    public void StopTileSFX(){
        if (tileAudioSource.isPlaying) tileAudioSource.Stop();
    }
}