using System.Collections.Generic;

namespace InputRecorder;

public struct TasBinding
{
    public string up;
    public string down;
    public string left;
    public string right;
    public string jump;
    public string confirm;
    public string cancel;
    public string pause;
    public string boots;
    public string snake;
    public string restart;

    public TasBinding GetDefault()
    {
        TasBinding res = new TasBinding
        {
            up = "U",
            down = "D",
            left = "L",
            right = "R",
            jump = "J",
            confirm = "K",
            cancel = "C",
            pause = "P",
            boots = "B",
            snake = "S",
            restart = "X"
        };
        return res;
    }
}
