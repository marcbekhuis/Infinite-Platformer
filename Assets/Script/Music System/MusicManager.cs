using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource songSource1;
    [SerializeField] private AudioSource songSource2;

    [SerializeField] private SongEntry[] songEntries;

    private void Start()
    {
        StartCoroutine(StartNextSong(null, GetNextSong(), songSource1));
    }

    private IEnumerator StartFade(SongEntry song, AudioSource audioSource)
    {
        float fadeDuration = song.startFade.keys.Last().time;
        Debug.LogError("Start fade: " + song.name + " Fade duration: " + fadeDuration);
        for (float i = 0; i < fadeDuration; i += Time.deltaTime)
        {
            audioSource.volume = song.startFade.Evaluate(i);
            Debug.Log("Volume: " + audioSource.volume + " Fade time: " + i.ToString("F2"));
            yield return null;
        }
        audioSource.volume = song.startFade.Evaluate(fadeDuration);
    }

    private IEnumerator EndFade(SongEntry song, AudioSource audioSource)
    {
        float fadeDuration = song.startFade.keys.Last().time;
        yield return new WaitForSeconds(song.song.length - fadeDuration);
        Debug.LogError("End fade: " + song.name + " Fade duration: " + fadeDuration);
        for (float i = 0; i < fadeDuration; i += Time.deltaTime)
        {
            audioSource.volume = song.endFade.Evaluate(i);
            Debug.Log("Volume: " + audioSource.volume + " Fade time: " + i.ToString("F2"));
            yield return null;
        }
        audioSource.volume = song.startFade.Evaluate(fadeDuration);
    }

    private IEnumerator StartNextSong(SongEntry oldSong, SongEntry currentSong, AudioSource audioSource)
    {
        if (oldSong != null)
        {
            yield return new WaitForSeconds(oldSong.song.length + currentSong.startOffset);
        }
        Debug.LogError("Starting: " + currentSong.name);
        audioSource.PlayOneShot(currentSong.song);
        if(!SongLooping(oldSong,currentSong))
            StartCoroutine(StartFade(currentSong,audioSource));

        SongEntry newSong = GetNextSong();
        if (!SongLooping(currentSong, newSong))
            StartCoroutine(EndFade(currentSong, audioSource));
        StartCoroutine(StartNextSong(currentSong, newSong, GetNextSource(audioSource)));
    }

    private SongEntry GetNextSong()
    {
        return songEntries[Random.Range(0,songEntries.Length)];
    }

    private AudioSource GetNextSource(AudioSource audioSource)
    {
        return audioSource == songSource1 ? songSource2 : songSource1;
    }

    private bool SongLooping(SongEntry oldSong, SongEntry newSong)
    {
        if(oldSong == newSong)
        {
            return newSong.loopAble;
        }

        return false;
    }
}
