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
        if (PrintState.up && InputRecorder.TasBinding.up!=string.Empty) {elements.Add(InputRecorder.TasBinding.up);}
        if (PrintState.down && InputRecorder.TasBinding.down!=string.Empty) {elements.Add(InputRecorder.TasBinding.down);}
        if (PrintState.left && InputRecorder.TasBinding.left!=string.Empty) {elements.Add(InputRecorder.TasBinding.left);}
        if (PrintState.right && InputRecorder.TasBinding.right!=string.Empty) {elements.Add(InputRecorder.TasBinding.right);}
        if (PrintState.jump && InputRecorder.TasBinding.jump!=string.Empty) {elements.Add(InputRecorder.TasBinding.jump);}
        if (PrintState.confirm && InputRecorder.TasBinding.confirm!=string.Empty) {elements.Add(InputRecorder.TasBinding.confirm);}
        if (PrintState.cancel && InputRecorder.TasBinding.cancel!=string.Empty) {elements.Add(InputRecorder.TasBinding.cancel);}
        if (PrintState.pause && InputRecorder.TasBinding.pause!=string.Empty) {elements.Add(InputRecorder.TasBinding.pause);}
        if (PrintState.boots && InputRecorder.TasBinding.boots!=string.Empty) {elements.Add(InputRecorder.TasBinding.boots);}
        if (PrintState.snake && InputRecorder.TasBinding.snake!=string.Empty) {elements.Add(InputRecorder.TasBinding.snake);}
        if (PrintState.restart && InputRecorder.TasBinding.restart!=string.Empty) {elements.Add(InputRecorder.TasBinding.restart);}

        return string.Join(",", elements);
    }
}