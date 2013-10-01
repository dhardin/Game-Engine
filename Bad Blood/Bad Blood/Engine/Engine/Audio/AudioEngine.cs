using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Engine.Audio
{
    public class AudioEngine : GameComponent
    {
        #region Fields

        private static AudioEngine _instance;
        private readonly ContentManager _content;
        private readonly Dictionary<string, SoundEffect> _effects;
        private readonly Dictionary<string, Song> _songs;

        #endregion

        #region Properties

        public static AudioEngine Instance
        {
            get { return _instance; }
        }

        #endregion

        public AudioEngine(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
            _content = game.Content;
            _songs = new Dictionary<string, Song>();
            _effects = new Dictionary<string, SoundEffect>();

            if (_instance != null)
                throw new InvalidOperationException("Only one audio component should be created.");

            _instance = this;
        }

        public Song GetSong(string assetName)
        {
            if (!_songs.Keys.Contains(assetName))
            {
                var song = _content.Load<Song>(assetName);
                _songs.Add(assetName, song);
            }

            return _songs[assetName];
        }

        public SoundEffectInstance GetSoundEffect(string assetName)
        {
            if (!_effects.Keys.Contains(assetName))
            {
                var effect = _content.Load<SoundEffect>(assetName);
                _effects.Add(assetName, effect);
            }
            var effectInstance = _effects[assetName].CreateInstance();
            return effectInstance;
        }
    }
}