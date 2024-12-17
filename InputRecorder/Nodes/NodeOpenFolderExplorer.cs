using BehaviorTree;
using JumpKing;
using System.Diagnostics;
using System.IO;

namespace InputRecorder.Nodes;
// This code is modified from MoreSaves.Nodes.NodeOpenFolderExplorer by Zebra
public class NodeOpenFolderExplorer : IBTnode
{
    private string _folderPath = "";
    public  NodeOpenFolderExplorer(string folderPath) {
        _folderPath = folderPath;
    }
    protected override BTresult MyRun(TickData p_data)
    {
        if (Directory.Exists(_folderPath)) {
            Process.Start("explorer.exe", _folderPath);
        }
        Game1.instance.contentManager.audio.menu.Select.Play();
        return BTresult.Success;
    }
}
