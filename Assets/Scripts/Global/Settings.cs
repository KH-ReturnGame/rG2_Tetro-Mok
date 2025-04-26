namespace Global
{
    public static class Settings
    {
        public static Volumes soundSettings = new(1.0f, 1.0f);

        public struct Volumes
        {
            public static float BGMVolume { get; set; }
            public static float SoundEffectVolume { get; set; }

            public Volumes(float bgmVolume, float soundEffectVolume)
            {
                BGMVolume = bgmVolume;
                SoundEffectVolume = soundEffectVolume;
            }
        }
    }
}