using System;
using System.Net;
using System.IO;
using System.Threading;
using FileManager;
using ZerochanCrawler;
using System.ComponentModel;
using System.Diagnostics;

namespace ZeroFramework {

    class ZeroNet {
        public string GetHTMLSourceFrom(string url) {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string html = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return html;
        }
    }

    class ZeroImageInspecter {
        public string url;

        public ZeroImageInspecter(string url) {
            this.url = url;
        }

        public void start() {
            Thread t1 = new Thread(new ThreadStart(getStaticImageURL));
            t1.Start();
        }

        private void getStaticImageURL() {
            ZeroNet network = new ZeroNet();
            string str = network.GetHTMLSourceFrom(this.url);
            string staticHref1 = ZeroString.Split(str, "static.zerochan.net/")[1];
            string staticHref = ZeroString.Split(staticHref1, "\"")[0];
            string staticURL = "http://static.zerochan.net/" + staticHref;
            string storage = ZeroFileManager.getPathOfFile(staticHref);
            Uri url = new Uri(staticURL);
            WebClient client = new WebClient();
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(completed);
            client.DownloadFileAsync(url, storage, staticHref);
            Counter.startDownload += 1;
            Console.WriteLine("Downloading {0}", staticURL);
        }
        void completed(object sender, AsyncCompletedEventArgs e) {
            Counter.finishDownload += 1;
            string fileName = (string) e.UserState;
            Console.WriteLine("Downloaded {0}. Remaining {1}", fileName, Counter.totalImageCount - Counter.finishDownload);
            if (Counter.totalImageCount == Counter.finishDownload && Counter.totalImageCount == Counter.startDownload && Counter.finishDownload == Counter.startDownload) {
                Main.sema.Close();
                Process.Start(@ZeroFileManager.getPathOfFile(""));
                Environment.Exit(0);
            }
        }
    }

    class ZeroImageCount {
        public string url;
        public int count;

        public ZeroImageCount(string url, int count) {
            this.url = url;
            this.count = count;
        }

        public void start() {
            Thread t1 = new Thread(new ThreadStart(getImageCount));
            t1.Start();
        }

        private void getImageCount() {
            ZeroNet network = new ZeroNet();
            string str = network.GetHTMLSourceFrom(this.url);
            if (str.Contains("thumbs2")) {
                string liStr1 = ZeroString.Split(str, "thumbs2")[1];
                string liStr2 = ZeroString.Split(liStr1, "</ul>")[0];
                string[] liArr = ZeroString.Split(liStr2, "</li>");
                int liImageCount = 0;
                foreach (string li in liArr) {
                    if (li.Contains("href")) {
                        Counter.totalImageCount += 1;
                        liImageCount += 1;
                        string href1 = ZeroString.Split(li, "href=\"")[1];
                        string href = ZeroString.Split(href1, "\" tabi")[0];
                        string hrefURL = "http://www.zerochan.net" + href;
                        new ZeroImageInspecter(hrefURL).start();
                    }
                }
                Console.WriteLine("{0} 페이지에서 {1}개 이미지 발견 | 총 {2} 개의 이미지를 발견", Counter.currentPage, liImageCount, Counter.totalImageCount);
            }
            if (Counter.currentPage < Counter.maxCount) {
                if (str.Contains("pagination")) {
                    string nextPage1 = ZeroString.Split(str, "pagination")[1];
                    string[] nextPage = ZeroString.Split(nextPage1, "href=\"");
                    foreach (string page in nextPage) {
                        if (page.Contains("next")) {
                            string nextPg = ZeroString.Split(page, "\"")[0];
                            string nextURL = Recorder.args + nextPg;
                            Counter.currentPage += 1;
                            new ZeroImageCount(nextURL, Counter.currentPage).start();
                        }
                    }
                }
            }
        }
    }

    public class Counter {
        public static int totalImageCount = 0;
        public static int currentPage = 1;
        public static int maxCount = 0;

        public static int startDownload = 0;
        public static int finishDownload = 0;
    }

    public class Recorder {
        public static string args = "";
    }

    class ZeroString {
        public static string[] Split(string source, string separator) {
            return System.Text.RegularExpressions.Regex.Split(source, separator);
        }
    }
}