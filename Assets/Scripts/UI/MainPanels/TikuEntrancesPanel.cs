using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TikuEntrancesPanel : BasePanel
{
    public ScrollRect sr;
    private string LibraryPath;
    protected override void Init()
    {
        LibraryPath = Application.persistentDataPath + @"\Ã‚ø‚";
        string[] dirs = Directory.GetDirectories(LibraryPath);
        foreach (string dir in dirs)
        {
            TikuEntrance entrance = Instantiate(Resources.Load<GameObject>("UI/TikuEntrance")).GetComponent<TikuEntrance>();
            entrance.InitEntrance(dir);
            entrance.gameObject.transform.SetParent(sr.content.transform, false);
        }
    }
}
