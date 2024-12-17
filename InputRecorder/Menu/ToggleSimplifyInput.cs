using JumpKing.PauseMenu.BT.Actions;

namespace InputRecorder.Menu;
public class ToggleSimplifyInput : ITextToggle
{
    public ToggleSimplifyInput() : base(InputRecorder.Preferences.IsSimplifyInput)
    {
    }

    protected override string GetName() => "Simplify Input";

    protected override void OnToggle()
    {
        InputRecorder.Preferences.IsSimplifyInput = toggle;
    }
}
