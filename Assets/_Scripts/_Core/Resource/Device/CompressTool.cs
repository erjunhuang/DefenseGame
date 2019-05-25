using System.IO;
using UnityEngine;

namespace Assets.Editor
{
    public class CompressTool
    {
        string[] paths = new string[]{
        "C:\\Program Files\\7-Zip\\7z.exe",
        "D:\\Program Files\\7-Zip\\7z.exe",
        "C:\\Program Files (x86)\\7-Zip\\7z.exe",
        "D:\\Program Files (x86)\\7-Zip\\7z.exe",
        };

        private string message = string.Empty;
        /// <summary>
        /// 目标文件+ "/res.7z";
        /// </summary>
        public string zipPath = string.Empty;
        /// <summary>
        /// 源路径
        /// </summary>
        public string tempPath = string.Empty;
        public void Compress()
        {
            //if (!running)
            //    return;

            string _appPath = string.Empty;

            foreach (var item in paths)
            {
                if (File.Exists(item))
                {
                    _appPath = item;
                    break;
                }
            }

            if (string.IsNullOrEmpty(_appPath))
            {
                message = "未找到7z.exe";
                return;
            }

            //  string path = assetPath + "/res.7z";

            //if (File.Exists(path))
            //    File.Delete(path);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = _appPath;
            process.StartInfo.Arguments = string.Format("a -tzip -mx5 {0} {1}data/ {1}config/ {1}res/ ", zipPath, tempPath); //

            //process.StartInfo.RedirectStandardInput = true;
            //process.StartInfo.UseShellExecute = false;

            Debug.Log(_appPath);
            Debug.Log(process.StartInfo.Arguments);

            //process.OutputDataReceived += delegate(object sender, System.Diagnostics.DataReceivedEventArgs args)
            //{
            //    Debug.Log(args.Data);
            //};

            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// 不压缩，直接合并
        /// </summary>
        public void Merger()
        {
            string _appPath = string.Empty;
            foreach (var item in paths)
            {
                if (File.Exists(item))
                {
                    _appPath = item;
                    break;
                }
            }

            if (string.IsNullOrEmpty(_appPath))
            {
                message = "未找到7z.exe";
                return;
            }
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = _appPath;
            process.StartInfo.Arguments = string.Format("a -tzip -mx0 {0} {1}data/ {1}config/ {1}res/ ", zipPath, tempPath);
            Debug.Log(_appPath);
            Debug.Log(process.StartInfo.Arguments);
            process.Start();
            process.WaitForExit();
        }

        public void Extract()
        {
            string _appPath = string.Empty;
            foreach (var item in paths)
            {
                if (File.Exists(item))
                {
                    _appPath = item;
                    break;
                }
            }

            if (string.IsNullOrEmpty(_appPath))
            {
                message = "未找到7z.exe";
                return;
            }

            //  string path = assetPath + "/res.7z";



            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = _appPath;
            process.StartInfo.Arguments = string.Format("x {0} -o{1}", zipPath, tempPath);

            //Debug.Log(_appPath);
            //Debug.Log(process.StartInfo.Arguments);

            process.Start();
            process.WaitForExit();
        }


    }
}
