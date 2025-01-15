using JumpKing.PauseMenu.BT.Actions;

namespace InputRecorder.Menu;
public class ToggleRecording : ITextToggle
{
    public ToggleRecording() : base(InputRecorder.Prefs.IsEnabledRecording)
    {
    }

    protected override string GetName() => "Enabled Recording";

    protected override void OnToggle()
    {
        InputRecorder.Prefs.IsEnabledRecording = toggle;
    }
}
