using UnityEngine;

namespace Global
{
    public class SoundManager : MonoBehaviour
    {
        private static AudioClip _gameWin, _gameLose, _point, _remove, _ui, _bgm;
        private static AudioSource _sfxSource, _bgmSource;

        public static SoundManager instance;
        [SerializeField] private AudioClip gameWin, gameLose, point, remove, ui, bgm;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this);

            _bgmSource = gameObject.AddComponent<AudioSource>();
            _sfxSource = gameObject.AddComponent<AudioSource>();

            _bgm = bgm;
            _gameWin = gameWin;
            _gameLose = gameLose;
            _point = point;
            _remove = remove;
            _ui = ui;

            _bgmSource.clip = _bgm;
            _bgmSource.loop = true;
        }

        public static void PlaySound(string soundName)
        {
            switch (soundName)
            {
                case "GameWin":
                    _sfxSource.PlayOneShot(_gameWin, 0.5f);
                    break;
                case "GameLose":
                    _sfxSource.PlayOneShot(_gameLose, 0.5f);
                    break;
                case "Point":
                    _sfxSource.PlayOneShot(_point);
                    break;
                case "Remove":
                    _sfxSource.PlayOneShot(_remove, 0.5f);
                    break;
                case "UI":
                    _sfxSource.PlayOneShot(_ui);
                    break;
            }
        }

        public static void PauseResumeBgm()
        {
            if (_bgmSource.isPlaying) _bgmSource.Pause();
            else _bgmSource.UnPause();
        }

        public static void PlayBgm()
        {
            _bgmSource.Play();
        }

        public static void StopAllSounds()
        {
            _bgmSource.Stop();
            _sfxSource.Stop();
        }
    }
}