using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Beanber.Utility.DTWebSettings
{
    public class IOHelper
    {

        public static string[] FileUploader(string sPath, FileUpload[] fuControl, string[] sFileType)
        {
            string[] sFileUrl = { "" };
            string sFileNotFits = string.Empty;
            if (sFileType != null)
            {
                for (int i = 0; i < fuControl.Length; i++)
                {
                    string[] sExtends = fuControl[i].FileName.Split('.');
                    string sExtend = sExtends[sExtends.Length - 1].ToLower();
                    //Kiem tra xem file co thuoc dinh dang cho phep hay ko
                    for (int j = 0; j < sFileType.Length; j++)
                    {
                        if (sFileType[j].EndsWith(sExtend))
                        {
                            sFileUrl[i] = UploadProcess(sPath, fuControl[i]);
                            break;
                        }
                        else
                        {
                            if (i != 0)
                            {
                                sFileNotFits += ";" + fuControl[i].FileName;
                            }
                            else
                            {
                                sFileNotFits = fuControl[i].FileName;
                            }
                        }
                    }
                }
                if (sFileNotFits.Length != 0)
                {
                    DTWebSettings.HtmlHelper.Alert("File \"" + sFileNotFits + "\" is not fit extension!", new System.Web.UI.Page());
                }
            }
            else
            {
                for (int i = 0; i < fuControl.Length; i++)
                {
                    sFileUrl[i] = UploadProcess(sPath, fuControl[i]);
                }
            }
            return sFileUrl;
        }

        public static string[] FileUploader(string sPath, FileUpload[] fuControl)
        {
            return FileUploader(sPath, fuControl, null);
        }

        public static string FileUploader(string sPath, FileUpload fuControl)
        {
            string[] sFileUrl = FileUploader(sPath, new FileUpload[] { fuControl }, null);
            return sFileUrl[0];
        }

        public static string FileUploader(string sPath, FileUpload fuControl, string[] sFileType)
        {
            string[] sFileUrl = FileUploader(sPath, new FileUpload[] { fuControl }, sFileType);
            return sFileUrl[0];
        }

        //Xu ly upload tra ve duong dan cua file
        protected static string UploadProcess(string sPath, FileUpload fuControl)
        {
            string sImageURL = String.Empty;
            string sFileName = String.Empty;
            //Kiem tra control ton tai duong dan thi moi up file
            if (fuControl.FileName != string.Empty)
            {
                String sNewName = DTWebSettings.ConvertUtility.ToDateTimeFormat(DateTime.Now, "ddMMyyy_HHmmss");
                sFileName = fuControl.FileName;
                //Kiem tra ten file hien tai co ton tai hay ko
                if (IsExistFile(fuControl.FileName, sPath, new System.Web.UI.Page()))
                {
                    sFileName = sNewName + "_" + sFileName;
                    fuControl.SaveAs(GetUserFloder(sPath) + sFileName);
                }
                else
                {
                    fuControl.SaveAs(GetUserFloder(sPath) + sFileName);
                }
                sImageURL = sPath + "/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + sFileName;
            }

            return sImageURL;
        }

        //Kiem tra file co trung ten hay ko
        protected static bool IsExistFile(string sFileName, string sPath, Page sPage)
        {
            bool isExist = false;
            string[] file = Directory.GetFiles(GetUserFloder(sPath));
            if (file.Length != 0)
            {
                //Kiem tra su trung lap file o thu muc hien tai
                for (int i = 0; i < file.Length; i++)
                {
                    string[] sFile = file[i].Split('\\');
                    if (sFileName == sFile[(sFile.Length - 1)])
                    {
                        isExist = true;
                        break;
                    }
                }
            }
            else
            {
                isExist = false;
            }
            return isExist;
        }

        //Lấy đường dẫn ảnh làm việc cho user, nếu chưa có thư mục hiện tại sẽ tạo mới
        protected static string GetUserFloder(string sPath)
        {

            string sflodertodo = DTWebSettings.ApplicationURL.ToFloder(sPath);

            sflodertodo = sflodertodo.Replace(" ", "").ToLower();
            sflodertodo = DTWebSettings.UnicodeUtility.ConvertUnicodeToNoSign(sflodertodo);
            sflodertodo = sflodertodo.Replace("/", "//");
            if (!Directory.Exists(new System.Web.UI.Page().Server.MapPath(sflodertodo)))
            {
                Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(sflodertodo));
            }

            string yearfloder = sflodertodo + DateTime.Now.Year.ToString();
            string monthfloder = yearfloder + "//" + DateTime.Now.Month.ToString();

            string[] yearfloders = Directory.GetDirectories(new System.Web.UI.Page().Server.MapPath(sflodertodo));

            if (Directory.Exists(new System.Web.UI.Page().Server.MapPath(yearfloder)))
            {
                string[] monthfloders = Directory.GetDirectories(new System.Web.UI.Page().Server.MapPath(yearfloder));

                if (!Directory.Exists(new System.Web.UI.Page().Server.MapPath(monthfloder)))
                {
                    Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(monthfloder));
                }
            }
            else
            {
                Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(yearfloder));
                Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(monthfloder));
            }


            return new System.Web.UI.Page().Server.MapPath(monthfloder + "//");
        }
        /// <summary>
        /// Xoa file 
        /// </summary>
        /// <param name="sDelDirectory">Đường dẫn file cần xóa, có cả tên file</param>
        public static bool Delete(string sVirtualPath)
        {
            try
            {
                File.Delete(HttpContext.Current.Server.MapPath(sVirtualPath));
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
            return true;
        }
        /// <summary>
        /// Lấy tên file
        /// </summary>
        /// <param name="hif">HtmlInputFile</param>
        /// <returns>Trả về tên file</returns>
        public static string GetFileName(HtmlInputFile hif)
        {
            try
            {
                return Path.GetFileName(hif.PostedFile.FileName);
            }
            catch (Exception ex)
            {
                throw ex;
                return string.Empty;
            }
        }
        /// <summary>
        /// Lấy kích cỡ file
        /// </summary>
        /// <param name="hif">HtmlInputFile</param>
        /// <returns>Trả về kích cỡ file</returns>
        public static int GetFileSize(HtmlInputFile hif)
        {
            try
            {
                return hif.PostedFile.ContentLength;
            }
            catch (Exception ex)
            {
                throw ex;
                return 0;
            }
        }
        /// <summary>
        /// Lấy kiểu định dạng file
        /// </summary>
        /// <param name="hif">HtmlInputFile</param>
        /// <returns>Trả về kiểu định dạng file</returns>
        public static string GetFileType(HtmlInputFile hif)
        {
            try
            {
                return hif.PostedFile.ContentType;
            }
            catch (Exception ex)
            {
                throw ex;
                return string.Empty;
            }

        }

        public static bool FileExist(string sVirtualPath)
        {
            try
            {
                return File.Exists(HttpContext.Current.Server.MapPath(sVirtualPath));
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }

        }

        // Create Folder in Server
        public static void CreateFolderInServer(String strPath)
        {
            String sThuMucLuuFileTrenServer = strPath.ToLower().Trim() + "/";
            if (!Directory.Exists(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer)))
            {
                Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer));
            }
        }

        // Resize Image to FileUrl In Server
        public static string FileUploadAndResizeFromStream(string ImageSavePath, int MaxSideSize, Stream Buffer)
        {
            String sFileName = DTWebSettings.ConvertUtility.ToDateTimeFormat(DateTime.Now, "ddMMyyy_HHmmss") + ".jpg";
            String sThuMucLuuFileTrenServer = ImageSavePath.Trim() + "/";
            sThuMucLuuFileTrenServer += DateTime.Now.Year.ToString();
            if (!Directory.Exists(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer)))
            {
                Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer));
            }
            sThuMucLuuFileTrenServer += "/" + DateTime.Now.Month.ToString();
            if (!Directory.Exists(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer)))
            {
                Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer));
            }
            sThuMucLuuFileTrenServer += "/" + DateTime.Now.Day.ToString();
            if (!Directory.Exists(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer)))
            {
                Directory.CreateDirectory(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer));
            }

            sThuMucLuuFileTrenServer += "/" + sFileName.Trim();

            int intNewWidth;
            int intNewHeight;
            System.Drawing.Image imgInput = System.Drawing.Image.FromStream(Buffer);
            //Determine image format 
            ImageFormat fmtImageFormat = imgInput.RawFormat;
            //get image original width and height 
            int intOldWidth = imgInput.Width;
            int intOldHeight = imgInput.Height;
            //determine if landscape or portrait 
            int intMaxSide;
            if (intOldWidth >= intOldHeight)
            {
                intMaxSide = intOldWidth;
            }
            else
            {
                intMaxSide = intOldHeight;
            }
            if (intMaxSide > MaxSideSize)
            {
                //set new width and height 
                double dblCoef = MaxSideSize / (double)intMaxSide;
                intNewWidth = Convert.ToInt32(dblCoef * intOldWidth);
                intNewHeight = Convert.ToInt32(dblCoef * intOldHeight);
            }
            else
            {
                intNewWidth = intOldWidth;
                intNewHeight = intOldHeight;
            }
            //create new bitmap 
            Bitmap bmpResized = new Bitmap(imgInput, intNewWidth, intNewHeight);
            //save bitmap to disk 
            bmpResized.Save(new System.Web.UI.Page().Server.MapPath(DTWebSettings.ApplicationURL.Root + sThuMucLuuFileTrenServer), fmtImageFormat);
            //release used resources 
            imgInput.Dispose();
            bmpResized.Dispose();
            Buffer.Close();

            return sThuMucLuuFileTrenServer;
        }

        // Get last Date modified
        public static DateTime GetLatDateModified(string sVirtualPath)
        {
            FileInfo file = new FileInfo(sVirtualPath);
            return file.LastWriteTime;
        }

    }
}