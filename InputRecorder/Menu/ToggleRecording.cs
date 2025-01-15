using JumpKing.PauseMenu.BT.Actions;

namespace InputRecorder.Menu;
public class ToggleRecording : ITextToggle
{
    public ToggleRecording() : base(InputRecorder.Prefs.IsEnableRecording)
    {
    }

    protected override string GetName() => "Enable Recording";

    protected override void OnToggle()
    {
        InputRecorder.Prefs.IsEnableRecording = toggle;
    }
}
