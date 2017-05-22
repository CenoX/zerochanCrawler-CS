using System;
using System.Linq;
using System.Threading;
using ZeroFramework;
using FileManager;

namespace ZerochanCrawler {
    class Draw {
        public static void draw() {
            Console.WriteLine("");
            Console.WriteLine("======= CCCCCCCC       ##  ##  =======");
            Console.WriteLine("======= CCCCCCCC      ##  ##   =======");
            Console.WriteLine("======= CC          ########## =======");
            Console.WriteLine("======= CC           ##  ##    =======");
            Console.WriteLine("======= CC         ##########  =======");
            Console.WriteLine("======= CCCCCCCCC   ##  ##     =======");
            Console.WriteLine("======= CCCCCCCCC  ##  ##      =======");
            Console.WriteLine("");
            Console.WriteLine("======================================");
            Console.WriteLine("==     Kara Hazimaru VS Seikatu     ==");
            Console.WriteLine("==                                  ==");
            Console.WriteLine("==          ZEROCHANCRAWLER         ==");
            Console.WriteLine("==                                  ==");
            Console.WriteLine("==     Project 2017.1.4 by CenoX    ==");
            Console.WriteLine("======================================");
            Console.WriteLine("");
            Console.WriteLine("ZerochanCrawler를 사용해주셔서 감사합니다!");
            Console.WriteLine("너님이 이걸 써서 얻는 모든 피해 등등은 나님이랑 1도 상관없음을 알아주세요!");
            Console.WriteLine("본 툴의 제작자는 'CenoX Kang'이며, 원작자는 'GyungDal'입니다.");
            Console.WriteLine("http://story.kakao.com/fv207");
            Console.WriteLine("Ported by CenoX from Swift project.");
            Console.WriteLine("엔터키를 눌러 계속 진행하세요");
            Console.ReadLine();
        }
    }

    class Program {
        public static string hostName = "zerochan.net";
        public static string appName = "zerochanCrawler";

        static void Main(string[] args) {
            if (args.Count() == 0) {
                Console.WriteLine("No args.");
                return;
            }
            Draw.draw();
            if (args[0] != null && !args[0].Contains(hostName)) {
                Console.WriteLine("This site is not zerochan site.");
                return;
            }
            string[] theme1 = ZeroString.Split(args[0], "/");
            string theme = theme1[theme1.Length - 1];
            FileRecorder.themeName = theme;
            ZeroFileManager.createFolderToDocumentWith(FileRecorder.themeName);
            new Main().main(args[0]);
        }
    }
    public class Main {
        public static Semaphore sema = new Semaphore(0, 1);
        public void main(string arg) {
            Console.WriteLine("Checking page {0}", arg);
            Recorder.args = arg;
            ZeroNet network = new ZeroNet();
            string mainStr = network.GetHTMLSourceFrom(arg);
            int maxPage = GetMaxPage(mainStr);
            if (maxPage == -485) {
                Console.WriteLine("Check failed. Exit");
                return;
            }
            Console.WriteLine(maxPage);
            Counter.maxCount = maxPage;
            new ZeroImageCount(arg, maxPage).start();
            sema.WaitOne();
        }

        private int GetMaxPage(string source) {
            if (source.Contains("pagination")) {
                string pageStr1 = ZeroString.Split(source, "pagination")[1];
                string pageStr2 = ZeroString.Split(pageStr1, "of")[1];
                string lastPageString = ZeroString.Split(pageStr2, "<a")[0];
                string trimed = lastPageString.Trim().Replace(",", "");
                int result;
                if (Int32.TryParse(trimed, out result)) {
                    return result;
                } else {
                    Console.WriteLine("COULD NOT CAST STRING TO INT\n{0}", trimed);
                    return -485;
                }
            } else {
                Console.WriteLine("NO PAGINATION");
                return -485;
            }
        }
    }
}