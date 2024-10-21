namespace Naninovel.UI
{
    /// <inheritdoc cref="IPauseUI"/>
    public class PauseUI : CustomUI, IPauseUI
    {
        public override UniTask InitializeAsync ()
        {
            BindInput(InputNames.Pause, ToggleVisibility, new BindInputOptions { WhenHidden = true });
            BindInput(InputNames.Cancel, Hide, new BindInputOptions { OnEnd = true });
            return UniTask.CompletedTask;
        }
    }
}
