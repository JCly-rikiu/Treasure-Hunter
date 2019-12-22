using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChanger : MonoBehaviour
{
 	public AudioSource AudioSrc;
 	public float AudioVolume = 0.5f;
 
 void Start () {
  AudioSrc = GetComponent<AudioSource>();
 }
 
 void Update () {
  AudioSrc.volume = AudioVolume;
 }
 
 public void SetVolume(float vol)
 {
  AudioVolume = vol;
 }
}
