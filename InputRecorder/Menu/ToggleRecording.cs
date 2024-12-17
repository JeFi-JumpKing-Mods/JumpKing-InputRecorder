using JumpKing.PauseMenu.BT.Actions;

namespace InputRecorder.Menu;
public class ToggleRecording : ITextToggle
{
    public ToggleRecording() : base(InputRecorder.IsEnabledRecording)
    {
    }

    protected override string GetName() => "Enabled Recording";

    protected override void OnToggle()
    {
        InputRecorder.IsEnabledRecording = toggle;
    }
}
