using Management;
using System.Collections;
using UnityEngine;

//galaga sound manager
public class SoundManager : SceneManager<SoundManager> {
    public AudioSource asPlayer, asEnemy;

    public AudioClip[] acEnemyDestroy = new AudioClip[9];
    public AudioClip acPlayerDestroy;
    public AudioClip acPlayerKidnap;
    public AudioClip acPlayerRescue;

    //player death
    public IEnumerator PlayPlayerDestroy() {
        asPlayer.clip = acPlayerDestroy;
        asPlayer.Play();
        yield return null;
    }

    //kidnap
    public IEnumerator PlayPlayerKidnap() {
        if (asPlayer.isPlaying) asPlayer.Stop();
        asPlayer.clip = acPlayerKidnap;
        asPlayer.Play();
        yield return null;
    }

    //player rescue
    public IEnumerator PlayPlayerRescue() {
        if (asPlayer.isPlaying) asPlayer.Stop();
        asPlayer.clip = acPlayerRescue;
        asPlayer.Play();
        yield return null;
    }

    //enemy death
    public IEnumerator PlayEnemyDestory(int n) {
        asEnemy.clip = acEnemyDestroy[n];
        asEnemy.Play();
        yield return null;
    }

    public override void GameStart() { }

    public override void GameOver() { }
}
