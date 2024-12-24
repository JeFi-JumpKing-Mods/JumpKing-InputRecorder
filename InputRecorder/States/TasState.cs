using System.Collections.Generic;
using JumpKing.Controller;

namespace InputRecorder.States;
public class TasState
{
    public int Ticks;
    public PadState PrintState;

    public TasState(PadState padState, int ticks = 1){
        PrintState = padState;
        Ticks = ticks;
    }
    public bool Extends(TasState newTasState) {
        if (newTasState.PrintState.Equals(PrintState)) {
            Ticks++;
            return true;
        }
        return false;
    }
    public string ToTasString() {
        List<string> elements = new();

        elements.Add($"{Ticks,4}");
        if (PrintState.up) {elements.Add(InputRecorder.TasBinding.up);}
        if (PrintState.down) {elements.Add(InputRecorder.TasBinding.down);}
        if (PrintState.left) {elements.Add(InputRecorder.TasBinding.left);}
        if (PrintState.right) {elements.Add(InputRecorder.TasBinding.right);}
        if (PrintState.jump) {elements.Add(InputRecorder.TasBinding.jump);}
        if (PrintState.confirm) {elements.Add(InputRecorder.TasBinding.confirm);}
        if (PrintState.cancel) {elements.Add(InputRecorder.TasBinding.cancel);}
        if (PrintState.pause) {elements.Add(InputRecorder.TasBinding.pause);}
        if (PrintState.boots) {elements.Add(InputRecorder.TasBinding.boots);}
        if (PrintState.snake) {elements.Add(InputRecorder.TasBinding.snake);}
        if (PrintState.restart) {elements.Add(InputRecorder.TasBinding.restart);}

        return string.Join(",", elements);
    }
}