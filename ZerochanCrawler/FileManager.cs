using System;
using ZerochanCrawler;

namespace FileManager {

    public class ZeroFileManager {
        public static void createFolderToDocumentWith(string name) {
            string pic = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string defaultPath = System.IO.Path.Combine(pic, Program.appName);
            if (!System.IO.Directory.Exists(defaultPath)) {
                System.IO.Directory.CreateDirectory(defaultPath);
            }
            string folder = System.IO.Path.Combine(defaultPath, name);
            if (!System.IO.Directory.Exists(folder)) {
                System.IO.Directory.CreateDirectory(folder);
            }
        }
        public static string getPathOfFile(string name) {
            string pic = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string defaultPath = System.IO.Path.Combine(pic, Program.appName);
            string folder = System.IO.Path.Combine(defaultPath, FileRecorder.themeName);
            string path = System.IO.Path.Combine(folder, name);
            return path;
        }
    }

    public class FileRecorder {
        public static string themeName = "";
    }
}