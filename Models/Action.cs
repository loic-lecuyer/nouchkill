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
    }
    public class TakeScreenshotAction : Action
    {
    }

    public class PlaySoundAction : Action
    {
    }
}
