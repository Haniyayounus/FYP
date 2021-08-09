using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Medius.Utility
{
    public static class FileUtility
    {
        private static IWebHostEnvironment _env;
        public static string applicationIssuePath = "/Content/Issues/Application/";

        //private static string AudioExtension = ".wav,.mid,.midi,.wma,.mp3,.ogg,.rma";
        private static string VideoExtension = ".avi,.mp4,.divx,.wmv";
        private static string DocumentExtension = ".pdf,.xlsx,.xls,.doc,.docx,.ppt,.pptx";
        private static string ImageExtension = ".jpg,.jpeg,.png,.JPG";
        //private static string HTMLExtension = ".html";
        private static string ZipExtension = ".zip";
        //private static string GameExtension = ".unity3d";


        public static void CreateFileFolder(string path)
        {
            if (!Directory.Exists(Path.Combine(path)))
            {
                Directory.CreateDirectory(
                    Path.Combine(path));
            }
        }

        public static string SaveFile(IFormFile file, string folderpath, bool audio = false, bool video = false, bool document = false, bool image = false)
        {
            FileUtility.CreateFileFolder(folderpath);

            string fileType = Path.GetExtension(file.FileName);
            if (GetFileExtensions(video, document, image).Contains(fileType) == false)
            {
                //for wrong file extension
                return "false";
            }
            string filePath =
                Path.Combine(
                    (folderpath),
                    file.FileName);

            //file.CopyTo(filePath);
            return folderpath + "/" + file.FileName;
        }

        public static string GetFileExtensions(bool video = false, bool document = false, bool image = false)
        {
            StringBuilder allowedExtensions = new StringBuilder();
            if (video)
            {
                allowedExtensions.Append(VideoExtension);
            }
            if (document)
            {
                allowedExtensions.Append(DocumentExtension);
            }
            if (image)
            {
                allowedExtensions.Append(ImageExtension);
            }
            return allowedExtensions.ToString();
        }

        public static bool IsVideoFile(string fileName)
        {
            return VideoExtension.Split(',').Contains(Path.GetExtension(fileName.ToLower()));
        }

        public static bool IsImageFile(string fileName)
        {
            return ImageExtension.Split(',').Contains(Path.GetExtension(fileName).ToLower());
        }
        public static bool IsZipFile(string fileName)
        {
            return ZipExtension.Split(',').Contains(Path.GetExtension(fileName));
        }


        //If UploadAllFiles=true : Upload all file in collection
        //If UploadAllFiles=false : Upload first file of collection only
        //FileCounter contains the current file index to be processed
        public static List<string> SaveDocument(IFormFileCollection files, string location, string filename, bool uploadAllFiles = false)
        {
            FileUtility.CreateFileFolder(location);
            List<string> fileServerPathsList = new List<string>();

            if (files.Count > 0)
            {
                int filecounter = 0;
                do
                {
                    string fileServerPath = Path.Combine((location),
                        filename);
                    //files[filecounter].SaveAs(fileServerPath);
                    fileServerPathsList.Add(fileServerPath);
                    filecounter++;
                } while (files.Count > filecounter && uploadAllFiles);
            }

            return fileServerPathsList;
        }

        public static bool SaveDocument(IFormFile file, string location, string filename, bool uploadAllFiles = false)
        {
            FileUtility.CreateFileFolder(location);
            string fileServerPath = Path.Combine(location,filename);
            try
            {
                //file.SaveAs(fileServerPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool DeleteDocument(string location)
        {
            if (System.IO.File.Exists(location))
            {
                System.IO.File.Delete(location);
                return true;
            }
            return false;
        }


        public static string GetFolderPath(string type, string grade, int? topic, string subject, string exerciseTitle)
        {
            return "/Content/" + type + "/" + grade + "/" + subject + "/" + topic + "/" + exerciseTitle;
        }

        public static string ReplaceSpecialCharacters(string name)
        {
            string[] specialCharachters = new string[] { "/", "\\", ":", "*", "?", "\"", "<", ">", "|" };

            foreach (var singleChar in specialCharachters)
            {
                name = name.Replace(singleChar, string.Empty);
            }
            name = Regex.Replace(name, @"\t|\n|\r", "");
            return name;
        }
        
    }
}