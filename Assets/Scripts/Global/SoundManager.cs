using UnityEngine;

namespace Global
{
    public class SoundManager : MonoBehaviour
    {
        private static AudioClip _gameWin, _gameLose, _point, _remove, _ui, _bgm;
        private static AudioSource _bgmSource, _sfxSource, _gameEndSource;

        public static SoundManager instance;
        public static float bgmVolume, sfxVolume, gameEndVolume;
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
            _gameEndSource = gameObject.AddComponent<AudioSource>();

            _bgm = bgm;
            _gameWin = gameWin;
            _gameLose = gameLose;
            _point = point;
            _remove = remove;
            _ui = ui;

            _bgmSource.clip = _bgm;
            _bgmSource.loop = true;

            bgmVolume = 1f;
            sfxVolume = 1f;
            gameEndVolume = 1f;
            _bgmSource.volume = bgmVolume;
        }

        public static void PlaySound(string soundName)
        {
            switch (soundName)
            {
                case "GameWin":
                    _gameEndSource.PlayOneShot(_gameWin, 0.3f * gameEndVolume);
                    break;
                case "GameLose":
                    _gameEndSource.PlayOneShot(_gameLose, 0.3f * gameEndVolume);
                    break;
                case "Point":
                    _sfxSource.PlayOneShot(_point, sfxVolume);
                    break;
                case "Remove":
                    _sfxSource.PlayOneShot(_remove, 0.5f * sfxVolume);
                    break;
                case "UI":
                    _sfxSource.PlayOneShot(_ui, 2f * sfxVolume);
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
            _gameEndSource.Stop();
        }

        public static void SetVolume(string soundName, float volume)
        {
            if (soundName == "BGM")
            {
                bgmVolume = volume;
                _bgmSource.volume = bgmVolume;
            }
            else if (soundName == "SFX")
            {
                sfxVolume = volume;
            }
            else if (soundName == "GameEnd")
            {
                gameEndVolume = volume;
            }
            else
            {
                //Debug.Log("Wrong Sound Name. Please Check It Again.");
            }
        }
    }
}