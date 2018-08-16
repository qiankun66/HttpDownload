using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace DownLoadDemo
{
    public class HttpFileRequest
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize = 0;
        /// <summary>
        /// 下载进度
        /// </summary>
        public double Process = 0;
        /// <summary>
        /// 进度更新委托
        /// </summary>
        /// <param name="process"></param>
        public delegate void ProcessUpdate(double process);
        /// <summary>
        /// 下载结束委托
        /// </summary>
        public delegate void LoadSuccess();
        /// <summary>
        /// 进度更新事件
        /// </summary>
        public ProcessUpdate MainUpdateProcess;
        /// <summary>
        /// 完成通知事件
        /// </summary>
        public LoadSuccess MainLoadSuccess;
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="url">网络地址</param>
        /// <param name="file">本地文件地址</param>
        public void DownLoad(string url, string file)
        {
            FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate);
            Stream webStream = null;
            try
            {
                long localFilePosition = 0;
                localFilePosition = fileStream.Length;
                fileStream.Seek(localFilePosition, SeekOrigin.Current);
                HttpWebRequest webRequest = (HttpWebRequest) HttpWebRequest.Create(url);
                if (localFilePosition > 0)
                {
                    if (DownLoadSuccess(localFilePosition,url))
                    {
                        return;
                    }
                    webRequest.AddRange(localFilePosition);
                }
                WebResponse webResponse = webRequest.GetResponse();
                //文件长度
                FileSize = webResponse.ContentLength;
                webStream = webResponse.GetResponseStream();
                if (webStream != null && FileSize > 0)
                {
                    byte[] bt = new byte[512];
                    int currentLength = webStream.Read(bt, 0, bt.Length);
                    while (currentLength > 0)
                    {
                        fileStream.Write(bt, 0, currentLength);
                        currentLength = webStream.Read(bt, 0, bt.Length);
                        Process = (Convert.ToDouble(fileStream.Length) / Convert.ToDouble(FileSize)) * 100;
                        MainUpdateProcess(Process > 100 ? 100 : Process);
                    }
                }
            }
            catch (Exception e)
            {
                //

            }
            finally
            {
                if (webStream!=null)
                {
                    webStream.Close();
                }
                fileStream.Close();
                //下载完成
                MainLoadSuccess();
            }
        }

        /// <summary>
        /// 验证是否下载完成
        /// </summary>
        /// <param name="localFileSize"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool DownLoadSuccess(long localFileSize,string url)
        {
            HttpWebRequest webRequest =(HttpWebRequest)HttpWebRequest.Create(url);
            long fileSize=webRequest.GetResponse().ContentLength;
            return localFileSize == fileSize;
        }
    }
}
