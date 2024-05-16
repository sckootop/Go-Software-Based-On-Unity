using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecognizeImagePanel : BasePanel
{
    public Button btnUploadImage;
    protected override void Init()
    {
        btnUploadImage.onClick.AddListener(() =>
        {
            string image_path = FileOperationByWin32.Get_OpeningImageFilePath();
            if (image_path == null || image_path == "") return;

            string output_sgf_path = Application.streamingAssetsPath;
            output_sgf_path += '/' + image_path.Split('\\').Last().Split('.').First() + ".sgf";//把文件名提取出来并附加到保存路径
            Debug.Log(output_sgf_path);

            GoRecognitionHelper goRecognitionHelper = new GoRecognitionHelper();

            Task<bool> t1 = new Task<bool>(() =>
            {
                return goRecognitionHelper.StartRecognize(image_path, output_sgf_path);
            });
            t1.Start();

            if (t1.Result == true)//这里会阻塞
            {
                if (File.Exists(output_sgf_path) == false) { Debug.Log("识别失败"); UIMgr.Instance.ShowPanel<TipPanel>().InitText("图片识别失败"); return; }
                ChessManual cm = ParseSgfHelper.Sgf2Chessmanual(output_sgf_path);
                RecognizeImageMgr.Instance.cm = cm;
                int size = cm.getSize();
                if (RecognizeImageMgr.Instance.curboardsize != size)
                {
                    RecognizeImageMgr.Instance.curboardsize = size;
                    UIMgr.Instance.HideAll();
                    SceneManager.LoadScene("RecognizeImageScene" + size);
                }
            }
            else
            {
                //显示识别失败提示面板
                UIMgr.Instance.ShowPanel<TipPanel>().InitText("图片识别失败");
                Debug.Log("识别失败");
            }
        });
    }
}
