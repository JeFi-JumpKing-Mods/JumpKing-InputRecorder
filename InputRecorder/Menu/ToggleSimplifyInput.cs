using JumpKing.PauseMenu.BT.Actions;

namespace InputRecorder.Menu;
public class ToggleSimplifyInput : ITextToggle
{
    public ToggleSimplifyInput() : base(InputRecorder.Prefs.IsSimplifyInput)
    {
    }

    protected override string GetName() => "Simplify Input";

    protected override void OnToggle()
    {
        InputRecorder.Prefs.IsSimplifyInput = toggle;
    }
}
