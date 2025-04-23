using NouchKill.IO;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;

namespace NouchKill.Models
{
    public enum ActionType
    {
        TakeScreenshot,
        PlaySound
    }
    [JsonDerivedType(typeof(TakeScreenshotAction), typeDiscriminator: "TakeScreenshot")]
    [JsonDerivedType(typeof(PlaySoundAction), typeDiscriminator: "PlaySound")]
    public abstract class Action
    {
        internal static Action? Create(ActionType? selectedActionType)
        {
            switch (selectedActionType)
            {
                case ActionType.TakeScreenshot:
                    return new TakeScreenshotAction();
                case ActionType.PlaySound:
                    return new PlaySoundAction();
            }
            return null;
        }

        public abstract void Run(List<Prediction> e, WebcamStream stream);
    }
    public class TakeScreenshotAction : Action
    {
        public override void Run(List<Prediction> e, WebcamStream stream)
        {
            Debug.WriteLine("TakeScreenshotAction");
            stream.TakeScreenshot();
        }
    }

    public class PlaySoundAction : Action
    {
        public override void Run(List<Prediction> e, WebcamStream stream)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string soundsDir = System.IO.Path.Combine(appPath, "Sounds");
            string mp3File = System.IO.Path.Combine(soundsDir, "dog.wav");
            using var audioEngine = new MiniAudioEngine(44100, Capability.Playback);

            try {
                // Create a SoundPlayer and load an audio file
                var player = new SoundPlayer(new StreamDataProvider(File.OpenRead(mp3File)));

                // Add the player to the master mixer
                Mixer.Master.AddComponent(player);

                // Start playback
                Debug.WriteLine("Start Play  Sound ");
                player.Play();
            } catch (Exception ex) {
                Debug.WriteLine("Erreur " + ex.Message);
            }
           
        }
    }
}
