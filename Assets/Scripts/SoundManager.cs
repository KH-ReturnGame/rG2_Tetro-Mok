using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioClip _gameWin, _gameLose, _point, _remove, _ui;
    private static AudioSource _audioSource, _bgm;
    [SerializeField] private AudioClip gameWin, gameLose, point, remove, ui;
    [SerializeField] private AudioSource bgm;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _gameWin = gameWin;
        _gameLose = gameLose;
        _point = point;
        _remove = remove;
        _ui = ui;
        _bgm = bgm;
    }


    public static void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "GameWin":
                _audioSource.PlayOneShot(_gameWin, 0.5f);
                break;
            case "GameLose":
                _audioSource.PlayOneShot(_gameLose, 0.5f);
                break;
            case "Point":
                _audioSource.PlayOneShot(_point);
                break;
            case "Remove":
                _audioSource.PlayOneShot(_remove, 0.5f);
                break;
            case "UI":
                _audioSource.PlayOneShot(_ui);
                break;
        }
    }

    public static void PauseResumeBgm()
    {
        if (_bgm.isPlaying) _bgm.Pause();
        else _bgm.UnPause();
    }
}