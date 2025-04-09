using NouchKill.Models;

namespace NouchKill.ViewModels
{
    public abstract class ActionViewModel : ViewModelBase
    {
        public abstract string DisplayName { get; }
        public static ActionViewModel? Create(Models.Action a)
        {
            if (a is PlaySoundAction psa)
            {
                return new PlaySoundActionViewModel(psa);
            }
            if (a is TakeScreenshotAction tsa)
            {
                return new TakeScreenshotActionViewModel(tsa);
            }
            return null;
        }

        public abstract NouchKill.Models.Action ToAction();
    }



    public class TakeScreenshotActionViewModel : ActionViewModel

    {
        private TakeScreenshotAction tsa;

        public TakeScreenshotActionViewModel(TakeScreenshotAction tsa)
        {
            this.tsa = tsa;
        }

        public override string DisplayName => "TakeScreenshot";

        public override Action ToAction()
        {
            return new TakeScreenshotAction()
            {
                // Add properties here
            };
        }
    }

    public class PlaySoundActionViewModel : ActionViewModel

    {
        public override string DisplayName => "PlaySound";
        private PlaySoundAction psa;

        public PlaySoundActionViewModel(PlaySoundAction psa)
        {
            this.psa = psa;
        }

        public override Action ToAction()
        {
            return new PlaySoundAction()
            {
                // Add properties here
            };
        }
    }
}
