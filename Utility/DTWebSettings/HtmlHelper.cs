using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Beanber.Utility.DTWebSettings
{
    public class HtmlHelper
    {
        public static void Alert(string Message, Page Page)
        {
            if (Message != null)
            {
                Page.RegisterStartupScript("AlertScript", "<script>alert(" + EscapeQuote(Message) + ")</script>");
            }
        }
        public static void Alert(string Message)
        {
            if (Message != null)
            {
                HttpContext.Current.Response.Write("<script>alert(" + EscapeQuote(Message) + ")</script>");
            }
        }

        public static void Back(int Step, Page Page)
        {
            Page.RegisterStartupScript("BackScript", "<script>history.go(" + Step + ")</script>");
        }

        public static void Confirm(string Message, string ifTrue, string ifFalse, Page Page)
        {
            if (Message != null)
            {
                Page.RegisterStartupScript("ConfirmScript", string.Format("<script>\r\nif (window.confirm({0}))\r\n{{{1}}}\r\nelse\r\n{{{2}}}\r\n</script>", EscapeQuote(Message), ifTrue, ifFalse));
            }
        }

        public static string EscapeQuote(string S)
        {
            if (S == null)
            {
                return "''";
            }
            StringBuilder builder = new StringBuilder("'", S.Length + 2);
            foreach (char ch in S)
            {
                switch (ch)
                {
                    case '\'':
                        builder.Append(@"\'");
                        break;

                    case '\r':
                        builder.Append(@"\r");
                        break;

                    case '\n':
                        builder.Append(@"\n");
                        break;

                    default:
                        if (ch > '~')
                        {
                            builder.Append("&#" + Convert.ToInt16(ch) + ";");
                        }
                        else
                        {
                            builder.Append(ch);
                        }
                        break;
                }
            }
            builder.Append("'");
            return builder.ToString();
        }

        public static string FormatDateTime(DateTime d)
        {
            return d.ToString("dd/MM/yyyy");
        }

        public static string GetContentOfClass(string strEditor, string strClass)
        {
            try
            {
                string str = string.Empty;
                string str2 = "class=\"" + strClass + "\"";
                int index = strEditor.IndexOf(str2);
                if (index != -1)
                {
                    index = strEditor.LastIndexOf("<", index);
                    if (index != -1)
                    {
                        index++;
                        int length = 0;
                        for (char ch = strEditor[index + length]; ch != ' '; ch = strEditor[index + length])
                        {
                            length++;
                        }
                        if (length != 0)
                        {
                            str = strEditor.Substring(index, length).Trim();
                            str2 = ">";
                            int startIndex = strEditor.IndexOf(str2, index + length);
                            if (startIndex != -1)
                            {
                                startIndex++;
                                str2 = "</" + str;
                                int num4 = strEditor.IndexOf(str2, startIndex);
                                if (num4 != -1)
                                {
                                    string str3 = strEditor.Substring(startIndex, num4 - startIndex);
                                    str2 = "<" + str;
                                    if (str3.IndexOf(str2, 0) != -1)
                                    {
                                        str2 = "</" + str;
                                        num4 = strEditor.IndexOf(str2, num4 + str2.Length);
                                        if (num4 != -1)
                                        {
                                            str3 = strEditor.Substring(startIndex, (num4 - startIndex) + 1);
                                        }
                                    }
                                    return str3;
                                }
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static DateTime GetDateFromRequestString(string RequestString, DateTime DefaultDate)
        {
            RequestString = HttpContext.Current.Request.QueryString[RequestString];
            if (RequestString != null)
            {
                try
                {
                    DateTime time = DateTime.Parse(RequestString);
                    if (time < DefaultDate)
                    {
                        DefaultDate = time;
                    }
                }
                catch
                {
                }
            }
            return DefaultDate.Date;
        }

        public static int GetIdFromRequestString(string RequestString, int DefaultID)
        {
            RequestString = HttpContext.Current.Request.QueryString[RequestString];
            if (RequestString != null)
            {
                try
                {
                    return int.Parse(RequestString);
                }
                catch
                {
                }
            }
            return DefaultID;
        }

        public static int GetImageID(string strEditor)
        {
            try
            {
                string str = "img.aspx?ImageID=";
                int index = strEditor.IndexOf(str, 0);
                if (index == -1)
                {
                    return 0;
                }
                int length = 0;
                index += str.Length;
                for (char ch = strEditor[index + length]; (ch >= '0') && (ch <= '9'); ch = strEditor[index + length])
                {
                    length++;
                }
                if (length == 0)
                {
                    return 0;
                }
                return int.Parse(strEditor.Substring(index, length));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string GetParagraph(string Content, string ClassName, bool RemoveFormatting)
        {
            string str = Content.ToLower();
            string str2 = ClassName.ToLower();
            int startIndex = 0;
            int index = 0;
            while ((startIndex != -1) && (index != -1))
            {
                startIndex = str.IndexOf("<p class=" + str2, index);
                if (startIndex != -1)
                {
                    index = str.IndexOf("</p>", startIndex);
                    int num3 = Content.IndexOf("<p", startIndex + 1, index - startIndex);
                    if (num3 != -1)
                    {
                        index = num3;
                    }
                    if (index != -1)
                    {
                        string str3 = RemoveFormatting ? StripHtml(Content.Substring(startIndex, index - startIndex)) : StripParagraph(Content.Substring(startIndex, index - startIndex));
                        if (str3.Length != 0)
                        {
                            return str3;
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static string GetRequestString(string RequestString, string DefaultString)
        {
            RequestString = HttpContext.Current.Request.QueryString[RequestString];
            if ((RequestString != null) && (RequestString != string.Empty))
            {
                return RequestString;
            }
            return DefaultString;
        }

        public static byte[] GetThumbnail(System.Drawing.Image Img, int Max)
        {
            if (Img.Width > Img.Height)
            {
                return GetThumbnail(Img, Max, (Max * Img.Height) / Img.Width);
            }
            return GetThumbnail(Img, (Max * Img.Width) / Img.Height, Max);
        }

        public static byte[] GetThumbnail(System.Drawing.Image Img, int Width, int Height)
        {
            if (Width == -1)
            {
                Width = Img.Width;
            }
            if (Height == -1)
            {
                Height = Img.Height;
            }
            MemoryStream stream = new MemoryStream();
            Bitmap image = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(Img, -1, -1, (int)(Width + 2), (int)(Height + 2));
            image.Save(stream, ImageFormat.Jpeg);
            byte[] buffer = stream.ToArray();
            stream.Close();
            return buffer;
        }

        public static byte[] GetWatermark(System.Drawing.Image Img, string Copyright, int Transparent)
        {
            int width = Img.Width;
            int height = Img.Height;
            MemoryStream stream = new MemoryStream();
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(Img, -1, -1, (int)(width + 2), (int)(height + 2));
            Font font = null;
            SizeF ef = new SizeF();
            for (int i = 100; i > 1; i--)
            {
                font = new Font("Arial", (float)i, FontStyle.Bold);
                if (((ushort)graphics.MeasureString(Copyright, font).Width) < ((ushort)width))
                {
                    break;
                }
            }
            float y = height / 2;
            float x = width / 2;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            SolidBrush brush = new SolidBrush(Color.FromArgb(Transparent, 0, 0, 0));
            graphics.DrawString(Copyright, font, brush, new PointF(x + 1f, y + 1f), format);
            SolidBrush brush2 = new SolidBrush(Color.FromArgb(Transparent, 0xff, 0xff, 0xff));
            graphics.DrawString(Copyright, font, brush2, new PointF(x, y), format);
            image.Save(stream, ImageFormat.Jpeg);
            byte[] buffer = stream.ToArray();
            stream.Close();
            return buffer;
        }

        public static bool IsEmptyTextBox(TextBox textBox)
        {
            return (textBox.Text.Trim().Length == 0);
        }

        public static string JoinTwoStrings(string s1, string s2, string separator)
        {
            if (s1 == null)
            {
                return s2;
            }
            if (s2 == null)
            {
                return s1;
            }
            return (s1 + separator + s2);
        }

        public static void Navigate(string Href, Page Page)
        {
            Page.RegisterStartupScript("NavigateScript", "<script>location.href='" + Href + "'</script>");
        }

        public static string NormalizeHtml(string S)
        {
            int startIndex = 0;
            int num2 = 0;
            while (((startIndex = S.IndexOf("/ImageView.aspx?", num2)) != -1) && ((num2 = S.LastIndexOf('"', startIndex)) != -1))
            {
                S = S.Remove(num2 + 1, startIndex - num2);
            }
            return S.Replace("<P>", "<P class=pBody>");
        }

        public static int ParseHtmlInt(string Source, string Prefix, string Surfix, int Start, int Default)
        {
            int index = Source.IndexOf(Prefix, Start);
            if (index != -1)
            {
                index += Prefix.Length;
                int num2 = Source.IndexOf(Surfix, index);
                if (num2 == -1)
                {
                    return Default;
                }
                try
                {
                    return int.Parse(Source.Substring(index, num2 - index));
                }
                catch
                {
                }
            }
            return Default;
        }

        public static void Script(string Script, Page Page)
        {
            if (Script != null)
            {
                Page.RegisterStartupScript("Script", "<script>" + Script + "</script>");
            }
        }

        public static string StripHtml(string S)
        {
            int startIndex = 0;
            int num2 = 0;
            while (((startIndex = S.IndexOf("<", startIndex)) != -1) && ((num2 = S.IndexOf(">", startIndex)) != -1))
            {
                S = S.Remove(startIndex, (num2 - startIndex) + 1);
            }
            return S.Trim();
        }

        public static string StripParagraph(string S)
        {
            int index = S.IndexOf(">");
            if (index != -1)
            {
                return S.Substring(index + 1).Trim();
            }
            return S.Trim();
        }

        public static DateTime TryParse(string s, DateTime def)
        {
            try
            {
                return DateTime.Parse(s);
            }
            catch
            {
                return def;
            }
        }

        public static int TryParse(string s, int def)
        {
            try
            {
                return int.Parse(s);
            }
            catch
            {
                return def;
            }
        }
    }
}