using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Beanber.Utility.DTWebSettings
{
    public class ApplicationSetting
    {
        //Get Current Language
        public static int GetCurrentLang(string sLangSessionName)
        {
            string sLang = HttpContext.Current.Session[sLangSessionName].ToString();
            if (!sLang.Equals("2"))
            {
                return 1;
            }
            return 2;
        }

        public static int GetCurrentLang()
        {
            return GetCurrentLang("LANG");
        }
        //Lấy đường dẫn thư mục gốc
        public static string URLRoot
        {
            get
            {
                String strPath = HttpContext.Current.Request.ApplicationPath;
                return strPath.EndsWith("/") ? strPath : strPath + "/";
            }
        }

        public static string URLPath(string sPath)
        {
            return URLRoot + sPath;
        }

        # region GetValueFromKey Code

        //Chuyền vào key để lấy ra value từ appSetting
        public static string GetValueFromKey(string strKey)
        {
            return GetValueFromKey(strKey, String.Empty);
        }

        //Chuyền vào key để lấy ra value từ appSetting nếu không có key sẽ lấy giá trị mặc định đưa vào
        public static string GetValueFromKey(string strKey, string strDefValue)
        {
            string ca = ConfigurationSettings.AppSettings[strKey];
            if (ca == null || ca.Length == 0)
                return strDefValue;
            else
                return ca.Trim();
        }

        //Sử dụng cho việc thêm Section mới trong webconfig
        private static string GetValueFromKey(string strSection, string strKey, string strDefValue)
        {
            NameValueCollection nvCol = ConfigurationSettings.GetConfig(strSection) as NameValueCollection;
            if (nvCol != null)
            {
                string ca = nvCol[strKey];
                if (ca != null && ca.Length != 0)
                {
                    return ca.Trim();
                }
            }
            return strDefValue;
        }

        # endregion
    }
    public class DatabaseHelper
    {
        //Lấy ngày tháng của hệ thống chèn vào database
        public const string GetDate = "GetDate()";

        # region "Xử lý chuỗi"

        //Kiểm tra một đối tượng có null hay không nếu null trả về giá trị default đưa vào
        public static int IsNull(object obValue, int intDefault)
        {
            return isnull(obValue) ? intDefault : (int)obValue;
        }

        public static bool isnull(object obValue)
        {
            return (obValue == null || obValue == DBNull.Value);
        }

        //Cắt hết các ký tự đặc biệt ở đầu và cuối chuỗi (Các ký tự trong ds bên dưới)
        public static string TrimString(string strValue)
        {
            if (strValue == null)
                return null;

            return strValue.Trim(new char[] { '[', ']', '*', '.', ' ' });
        }

        //Đưa vào một chuỗi, kiểm tra nếu có ký tự đặc biệt thì trả về nguyên chuỗi, ngược lại trả về [chuỗi]
        public static string EscapeName(string S)
        {
            if (S.IndexOfAny(new char[] { '[', ']', '*', '.', ' ' }) != -1)
                return S;
            else
                return "[" + S + "]";
        }

        public static string EscapeQuote(string S)
        {
            return "'" + S.Trim().Replace("'", "''") + "'";
        }

        public static string EscapeQuoteButNotTrim(string S)
        {
            return "'" + S.Replace("'", "''") + "'";
        }

        //Trả về một chuỗi thêm (chuỗi)
        public static string EscapeBoolBit(string Str)
        {
            return "(" + Str + ")";
        }

        //Ghép thêm ký tự "N" vào trước chuỗi khi chèn vào CSDL
        public static string EscapeUnicode(string Str)
        {
            return "N" + EscapeQuote(Str);
        }

        # endregion

        # region  Biến toàn cục
        //Lấy ConnectionString trong WebConfig
        protected static string connectionString_ = ConfigurationManager.ConnectionStrings["SystemContext"].ConnectionString;
        protected static string ShowDetailError_ = ApplicationSetting.GetValueFromKey("ShowDetailError");
        private static string SQLstring_;
        private static SqlConnection _sConnection;
        # endregion

        # region Execute DatabaseHelper Code

        //Thư viện
        public static int Execute(string QueryString)
        {
            SqlConnection connection = new SqlConnection(connectionString_);
            connection.Open();
            int result;
            //try
            {
                SqlCommand command = new SqlCommand(QueryString, connection);
                SQLstring_ = command.CommandText;
                result = command.ExecuteNonQuery();
            }
            //catch (Exception ex)
            //{
            //    return (int) ShowError(ex);
            //}
            //finally
            //{
            //    connection.Close();
            //}
            return result;
        }

        # endregion

        # region Update DatabaseHelper Code

        public static int Update(string[] SetValue, string TableName, string Criterias)
        {
            if (SetValue == null || SetValue.Length == 0)
                return (int)ShowError("Invalid Update");

            string setvalues = string.Join(", ", SetValue);
            string criterias = (Criterias != null ? " WHERE " + Criterias : string.Empty);

            return Execute(string.Format("UPDATE {1} SET {0}{2}", setvalues, EscapeName(TableName), criterias));
        }

        public static int Update(string[] FieldNames, string[] FieldValues, string TableName, string Criterias)
        {
            if (FieldNames == null || FieldNames.Length == 0 || FieldValues == null || FieldValues.Length == 0 ||
                FieldNames.Length != FieldValues.Length)
                return (int)ShowError("Invalid Update! Fields,Values Error!");

            return Execute(FormatSql_UPDATE(FieldNames, FieldValues, TableName, Criterias));
        }

        //Thực thi một câu lệnh thay đổi database trả về kết quả có thực hiện được hay không
        public static int Update(string QueryString)
        {
            return Execute(QueryString);
        }

        # endregion

        /*-------------------------*/

        # region InsertIdentity DatabaseHelper Code

        public static int InsertIdentity(string QueryString)
        {
            SqlConnection connection = new SqlConnection(connectionString_);
            connection.Open();
            try
            {
                SqlCommand command = new SqlCommand(QueryString, connection);
                SQLstring_ = command.CommandText;
                command.ExecuteNonQuery();
                command.CommandText = "SELECT @@Identity";
                SQLstring_ = command.CommandText;
                int result = Convert.ToInt32(command.ExecuteScalar());
                return result;
            }
            catch (Exception ex)
            {
                return (int)ShowError(ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public static int InsertIdentity(string[] FieldNames, string[] FieldValues, string TableName)
        {
            if (FieldNames == null || FieldNames.Length == 0 || FieldValues == null || FieldValues.Length == 0 ||
                FieldNames.Length != FieldValues.Length)
                return (int)ShowError("Invalid Insert! Fields,Values Error!");
            return InsertIdentity(FormatSql_INSERT(FieldNames, FieldValues, TableName));
        }

        # endregion

        /*-------------------------*/

        # region Insert DatabaseHelper Code

        public static int Insert(string[] FieldNames, string[] FieldValues, string TableName)
        {
            if (FieldNames == null || FieldNames.Length == 0 || FieldValues == null || FieldValues.Length == 0 ||
                FieldNames.Length != FieldValues.Length)
                return (int)ShowError("Invalid Insert! Fields,Values Error!");
            return Execute(FormatSql_INSERT(FieldNames, FieldValues, TableName));
        }

        public static int Insert(string QueryString)
        {
            return Execute(QueryString);
        }

        # endregion

        # region Delete DatabaseHelper Code

        public static int Delete(string TableName, string Criterias)
        {
            return Execute(FormatSql_DELETE(TableName, Criterias));
        }

        public static int Delete(string QueryString)
        {
            return Execute(QueryString);
        }

        # endregion

        # region Thực thi store procedure
        //Hàm trả về SqlCommand thực thi StoreProcedure
        private static SqlCommand GetCommand(IDataParameter[] _iDataParameter, string sStoreProcedureName)
        {
            SqlCommand _sCommand = new SqlCommand(sStoreProcedureName, _sConnection);
            _sCommand.CommandType = CommandType.StoredProcedure;
            _sCommand.CommandTimeout = 1000; //Max timeout is about 15mins
            if (_iDataParameter != null)
            {
                if (_iDataParameter.Length > 0)
                {
                    foreach (SqlParameter _sParameter in _iDataParameter)
                    {
                        _sCommand.Parameters.Add(_sParameter);
                    }
                }
            }
            //else
            //{
            //    _sCommand = GetCommand(sStoreProcedureName);
            //}
            return _sCommand;
        }
        private static SqlCommand GetCommand(string sStoreProcedure)
        {
            return GetCommand(null, sStoreProcedure);
        }
        //Hàm thực thi StoreProcedure tra ve datatable
        public static DataTable SelectDataTable(IDataParameter[] _iDataParameter, string sStoreProcedureName)
        {
            _sConnection = new SqlConnection(DatabaseHelper.connectionString_);
            DataTable dt = new DataTable();
            SqlCommand _sCommand = null;
            try
            {
                if (_iDataParameter == null || _iDataParameter.Length == 0)
                {
                    //Goi dang co 1 tham so
                    _sCommand = GetCommand(sStoreProcedureName);
                }
                else
                {
                    //Goi dang co 2 tham so
                    _sCommand = GetCommand(_iDataParameter, sStoreProcedureName);
                }

                SqlDataAdapter _sDataAdapter = new SqlDataAdapter(_sCommand);
                _sDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                return (DataTable)ShowError(ex);
            }
            finally
            {
                _sConnection.Close();
                _sConnection.Dispose();

            }
            return dt;
        }
        //Gọi ra datatable ko có điều kiện đưa vào
        public static DataTable SelectDataTable(string sStoreProcedureName)
        {
            return SelectDataTable(null, sStoreProcedureName);
        }

        //Đọc ra dataReader có điều kiện đưa vào
        public static SqlDataReader SelectDataReader(IDataParameter[] _iDataParameter, string sStoreProcedureName)
        {
            _sConnection = new SqlConnection(DatabaseHelper.connectionString_);
            SqlDataReader _sDataReader;
            SqlCommand _sCommand = null;
            if (_iDataParameter == null || _iDataParameter.Length == 0)
            {
                //Goi dang co 1 tham so
                _sCommand = GetCommand(sStoreProcedureName);
            }
            else
            {
                //Goi dang co 2 tham so
                _sCommand = GetCommand(_iDataParameter, sStoreProcedureName);
            }

            try
            {

                _sConnection.Open();
                _sDataReader = _sCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                return (SqlDataReader)ShowError(ex);
            }

            return _sDataReader;
        }
        public static SqlDataReader SelectDataReader(string sStoreProcedureName)
        {
            return SelectDataReader(null, sStoreProcedureName);
        }

        //Thực thi StoreProcedure thay đổi CSDL (Insert,Upadte,Delete)
        public static int ExcuteCommand(string sStoreProcedureName, IDataParameter[] _iDataParameter)
        {
            int result = 0;
            _sConnection = new SqlConnection(DatabaseHelper.connectionString_);
            SqlCommand _sCommand = null;
            if (_iDataParameter == null || _iDataParameter.Length == 0)
            {
                //Goi dang co 1 tham so
                _sCommand = GetCommand(sStoreProcedureName);
            }
            else
            {
                //Goi dang co 2 tham so
                _sCommand = GetCommand(_iDataParameter, sStoreProcedureName);
            }
            try
            {
                _sConnection.Open();
                result = _sCommand.ExecuteNonQuery();
            }
            catch
            {
                return result;
            }
            _sConnection.Close();
            _sConnection.Dispose();
            return result;
        }
        public static int ExcuteCommand(string sStoreProcedureName)
        {
            return ExcuteCommand(sStoreProcedureName, null);
        }

        //Đọc ra dataset
        public static DataSet SelectDataSet(string sStoreProcedureName, IDataParameter[] _iDataParameter, string tblName)
        {
            DataSet ds = new DataSet();
            _sConnection = new SqlConnection(DatabaseHelper.connectionString_);
            SqlCommand _sCommand = null;
            if (_iDataParameter == null || _iDataParameter.Length == 0)
            {
                //Goi dang co 1 tham so
                _sCommand = GetCommand(sStoreProcedureName);
            }
            else
            {
                //Goi dang co 2 tham so
                _sCommand = GetCommand(_iDataParameter, sStoreProcedureName);
            }
            SqlDataAdapter oAdapter = new SqlDataAdapter(_sCommand);
            oAdapter.Fill(ds, tblName);
            return ds;

        }
        public static DataSet SelectDataSet(string sStoreProcedureName, string tblName)
        {
            return SelectDataSet(sStoreProcedureName, null, tblName);
        }

        #endregion

        # region Select đữ liệu bằng query string
        public static SqlDataReader SelectReader(string QueryString)
        {
            SqlConnection connection = new SqlConnection(connectionString_);
            connection.Open();
            try
            {
                SQLstring_ = QueryString;
                SqlCommand command = new SqlCommand(SQLstring_, connection);
                command.CommandTimeout = 1000; //Max timeout is about 15mins
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                if (ShowDetailError_ != null && ShowDetailError_ == "1")
                    throw new Exception(SQLstring_ + " - DatabaseHelper Error! - " + ex.Message);
                else
                    throw new Exception("DatabaseHelper Error! - " + ex.Message);
            }
        }

        public static SqlDataReader SelectReader(string[] Fields, string TableName, string Criterias, string Priorities)
        {
            return SelectReader(FormatSql_SELECT(Fields, TableName, Criterias, Priorities));
        }

        public static SqlDataReader SelectReader(string[] Fields, string TableName, string Criterias)
        {
            return SelectReader(Fields, TableName, Criterias, null);
        }

        public static SqlDataReader SelectReader(string[] Fields, string TableName)
        {
            return SelectReader(Fields, TableName, null, null);
        }


        public static DataTable Select(string QueryString)
        {
            SqlConnection connection = new SqlConnection(connectionString_);
            DataTable table;
            try
            {
                SQLstring_ = QueryString;
                SqlDataAdapter adapter = new SqlDataAdapter(SQLstring_, connection);
                table = new DataTable();
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                return (DataTable)ShowError(ex);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return table;
        }

        public static DataTable Select(string[] Fields, string TableName, string Criterias, string Priorities)
        {
            return Select(FormatSql_SELECT(Fields, TableName, Criterias, Priorities));
        }

        public static DataTable Select(string[] Fields, string TableName, string Criterias)
        {
            return Select(Fields, TableName, Criterias, null);
        }

        public static DataTable Select(string[] Fields, string TableName)
        {
            return Select(Fields, TableName, null, null);
        }

        public static DataSet SelectDataSet(string QueryString)
        {
            SqlConnection connection = new SqlConnection(connectionString_);
            DataSet table;
            try
            {
                SQLstring_ = QueryString;
                SqlDataAdapter adapter = new SqlDataAdapter(SQLstring_, connection);
                table = new DataSet();
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                return (DataSet)ShowError(ex);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return table;
        }

        # endregion


        # region SelectScalar DatabaseHelper Code

        public static object SelectScalar(string QueryString)
        {
            SqlConnection connection = new SqlConnection(connectionString_);
            connection.Open();
            object result;
            try
            {
                SQLstring_ = QueryString;
                SqlCommand command = new SqlCommand(SQLstring_, connection);
                command.CommandTimeout = 1000; //Max timeout is about 15mins
                result = command.ExecuteScalar();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                if (ShowDetailError_ != null && ShowDetailError_ == "1")
                    throw new Exception(SQLstring_ + " - DatabaseHelper Error! - " + ex.Message);
                else
                    throw new Exception("DatabaseHelper Error! - " + ex.Message);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public static object SelectScalarObject(string Field, string TableName, string Criterias)
        {
            string criterias = (Criterias != null ? " WHERE " + Criterias : string.Empty);
            return SelectScalar(string.Format("SELECT {0} FROM [{1}] {2}", Field, TableName, criterias));
        }

        public static string SelectScalarString(string Field, string TableName, string Criterias)
        {
            object ob = SelectScalarObject(Field, TableName, Criterias);
            if (ob == null || ob == DBNull.Value)
                return string.Empty;
            return ob.ToString();
        }

        public static int SelectScalarInt(string Field, string TableName, string Criterias)
        {
            object ob = SelectScalarObject(Field, TableName, Criterias);
            if (ob == null || ob == DBNull.Value)
                return 0;
            return (int)ob;
        }

        public static bool SelectScalarBool(string Field, string TableName, string Criterias)
        {
            object ob = SelectScalarObject(Field, TableName, Criterias);
            if (ob == null || ob == DBNull.Value)
                return false;
            return (bool)ob;
        }

        public static string SelectScalarStringFromID(string Field, string TableName, int IdValue)
        {
            object o = SelectScalarObjectFromID(Field, TableName, IdValue);

            return (o != DBNull.Value ? (string)o : null);
        }

        public static int SelectScalarIntFromID(string Field, string TableName, int IdValue)
        {
            object o = SelectScalarObjectFromID(Field, TableName, IdValue);

            return (o != DBNull.Value && o != null ? (int)o : -1);
        }

        public static bool SelectScalarBoolFromID(string Field, string TableName, int IdValue)
        {
            object o = SelectScalarObjectFromID(Field, TableName, IdValue);

            return (o != DBNull.Value && o != null ? (bool)o : false);
        }

        public static DateTime SelectScalarDateTimeFromID(string Field, string TableName, int IdValue)
        {
            object o = SelectScalarObjectFromID(Field, TableName, IdValue);

            return (o != DBNull.Value && o != null ? (DateTime)o : DateTime.MinValue);
        }

        public static object SelectScalarObjectFromID(string Field, string TableName, int IdValue)
        {
            return SelectScalar(string.Format("SELECT [{0}] FROM [{1}] WHERE [{1}ID]={2}", Field, TableName, IdValue));
        }

        # endregion

        # region FormatSQLString Code

        private static string FormatSql_SELECT(string[] Fields, string TableName, string Criterias, string Priorities)
        {
            string fields = (Fields != null && Fields.Length != 0 ? string.Join(", ", Fields) : "*");
            string criterias = (Criterias != null ? " WHERE " + Criterias : string.Empty);
            string priorities = (Priorities != null ? " ORDER BY " + Priorities : string.Empty);

            return string.Format("SELECT {0} FROM {1}{2}{3}", fields, EscapeName(TableName), criterias, priorities);
        }

        private static string FormatSql_DELETE(string TableName, string Criterias)
        {
            string criterias = (Criterias != null ? " WHERE " + Criterias : string.Empty);
            return string.Format("DELETE {0} {1}", EscapeName(TableName), criterias);
        }

        private static string FormatSql_INSERT(string[] FieldNames, string[] FieldValues, string TableName)
        {
            FormatSQLString(ref FieldNames, ref FieldValues);
            string fieldNames = string.Join(", ", FieldNames);
            string fieldValues = string.Join(", ", FieldValues);

            return string.Format("INSERT INTO {2}({0}) VALUES ({1})", fieldNames, fieldValues, EscapeName(TableName));
        }

        public static string FormatSql_UPDATE(string[] FieldNames, string[] FieldValues, string TableName, string Criterias)
        {
            FormatSQLString(ref FieldNames, ref FieldValues);

            string[] setstr = new string[FieldNames.Length];
            for (int i = 0; i < FieldNames.Length; i++)
                setstr[i] = FieldNames[i] + "=" + FieldValues[i];

            string fieldnames = string.Join(" ,", setstr);

            string criterias = (Criterias != null ? " WHERE " + Criterias : string.Empty);

            return string.Format("UPDATE {1} SET {0}{2}", fieldnames, EscapeName(TableName), criterias);
        }

        private static void FormatSQLString(ref string[] Field, ref string[] Value)
        {
            if (Field != null && Value != null)
                for (int i = 0; i < Field.Length; i++)
                {
                    Field[i] = Field[i].ToLower().Trim();
                    Value[i] = EscapeQuote(Value[i]);
                    if (Field[i][0] != '[')
                        Field[i] = EscapeName(Field[i]);
                }
        }


        # endregion

        # region ShowError Code

        public static object ShowError(Exception ex)
        {
            if (ShowDetailError_ != null && ShowDetailError_ == "1")
                throw new Exception(SQLstring_ + " - DatabaseHelper Error! - " + ex.Message);
            else
                throw new Exception("DatabaseHelper Error! - " + ex.Message);
        }

        public static object ShowError(string str)
        {
            throw new Exception("DatabaseHelper Error! - " + str);
        }

        #endregion

        # region Export/Import Data To Excel Code

        public static void ExportDataExcel(string filename)
        {
            try
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();

                response.AddHeader("content-disposition", "filename=" + filename + ".xls");
                response.ContentType = "application/txt";
                response.Charset = "utf-8";
            }
            catch (Exception ex)
            {
                throw new Exception("Error ExportDataExcel! - " + ex.Message);
            }
        }

        public static DataTable ImportDataExcel(string Filename, string Sheet)
        {
            return ImportDataExcel(null, Filename, Sheet);
        }

        public static DataTable ImportDataExcel(string sqlSELECT, string Filename, string Sheet)
        {
            string SQLstring_;

            if (sqlSELECT == null || sqlSELECT == string.Empty)
                SQLstring_ = "SELECT * FROM " + "[" + Sheet + "$]";
            else
                SQLstring_ = "SELECT " + sqlSELECT + " FROM " + "[" + Sheet + "$]";
            try
            {
                string connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Filename +
                    ";Extended Properties=Excel 8.0;";
                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(SQLstring_, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                adapter.Dispose();
                connection.Close();
                connection.Dispose();
                return table;
            }
            catch (Exception ex)
            {
                throw new Exception(SQLstring_ + " - Error ImportDataExcel! - " + ex.Message);
            }
        }

        # endregion
    }

    //Lớp chuyển đổi kiểu chữ
    public class UnicodeUtility
    {

        public const string sUniChars =
            "àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴÂĂĐÔƠƯ'&";

        public const string sNoSignChars =
            "aaaaaaaaaaaaaaaaaeeeeeeeeeeediiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAAEEEEEEEEEEEDIIIOOOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYAADOOU -";

        public static string sTCVN3 =
            "¸µ¶·¹¨¾»¼½Æ©ÊÇÈÉËÐÌÎÏÑªÕÒÓÔÖÝ×ØÜÞãßáâä«èåæçé¬íêëìîóïñòô­øõö÷ùýúûüþ®¸µ¶·¹¡¾»¼½Æ¢ÊÇÈÉËÐÌÎÏÑ£ÕÒÓÔÖÝ×ØÜÞãßáâä¤èåæçé¥íêëìîóïñòô¦øõö÷ùýúûüþ§ -";

        public static string sUnicode =
            "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ -";

        public static string MD5Encrypt(string plainText)
        {
            byte[] data, output;
            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();

            data = encoder.GetBytes(plainText);
            output = hasher.ComputeHash(data);

            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }
        public static string SHA1Encrypt(string sPlainText)
        {
            string sHasher = FormsAuthentication.HashPasswordForStoringInConfigFile(sPlainText, "SHA1");
            return sHasher;
        }

        public static string GetEncodingByAccII(String sAccii)
        {
            String sEncoding = "";
            IDataParameter[] parameter = new IDataParameter[]
            {   
                new SqlParameter("@sCharacter",SqlDbType.NChar, 10, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Proposed, sAccii.Trim())
            };
            DataTable dt = DTWebSettings.DatabaseHelper.SelectDataTable(parameter, "[dbo].[Sys_dbo.sysEncoding_SelectEncodingByCharacter]");
            if (dt.Rows.Count > 0)
            {
                sEncoding = dt.Rows[0]["Encoding"].ToString().Trim();
            }
            return sEncoding;
        }

        public static string ReplaceEncoding(string sACII)
        {
            string sEncoding = "";
            for (int i = 0; i < sACII.Length; i++)
            {
                String sCheck = DTWebSettings.UnicodeUtility.GetEncodingByAccII(sACII[i].ToString());
                if (sCheck.Trim() != "")
                {
                    sEncoding += sCheck.ToString().Trim();
                }
                else
                {
                    sEncoding += sACII[i].ToString();
                }
            }
            return sEncoding;
        }

        //Convert String
        private static string ConvertString(string _strCon1, string _strCon2, string _ConvertStr)
        {
            string strTemp = string.Empty;
            int intPos;
            for (int i = 0; i < _ConvertStr.Length; i++)
            {
                intPos = _strCon1.IndexOf(_ConvertStr[i]);
                if (intPos >= 0)
                    strTemp += _strCon2[intPos];
                else
                    strTemp += _ConvertStr[i];
            }
            return strTemp;
        }

        //Chuyển TCVN3 sang Unicode
        public static string ConvertTcvn3ToUnicode(string sConvertString)
        {
            if (sConvertString.Length == 0)
                return sConvertString;
            return ConvertString(sTCVN3, sUnicode, sConvertString);
        }

        //Chuyển Unicode sang TCVN3
        public static string ConvertUnicodeToTcvn3(string sConvertString)
        {
            if (sConvertString.Length == 0)
                return sConvertString;
            return ConvertString(sUnicode, sTCVN3, sConvertString);
        }

        //Chuyển sang Unicode có dấu sang không dấu
        public static string ConvertUnicodeToNoSign(string sConvertString)
        {
            if (sConvertString.Length == 0)
                return sConvertString;
            return ConvertString(sUniChars, sNoSignChars, sConvertString);
        }
    }

    //Lớp chuyển đổi các định dạng
    public class ConvertUtility
    {
        public static Int32 ToInt32(object val, Int32 defValue)
        {
            Int32 ret = defValue;
            try
            {
                ret = Convert.ToInt32(val);
            }
            catch
            {
            }
            return ret;
        }

        public static Int32 ToInt32(object val)
        {
            return ToInt32(val, 0);
        }

        public static Decimal ToDecimal(object val, Decimal defValue)
        {
            Decimal ret = defValue;
            try
            {
                ret = Convert.ToInt32(val);
            }
            catch
            {
            }

            return ret;
        }

        public static Decimal ToDecimal(object val)
        {
            return ToDecimal(val, 0);
        }

        public static Single ToSingle(object val, Single defValue)
        {
            Single ret = defValue;
            try
            {
                ret = Convert.ToSingle(val);
            }
            catch
            {
            }

            return ret;
        }

        public static Single ToSingle(object val)
        {
            return ToSingle(val, 0);
        }

        public static DateTime ToDateTime(object val, DateTime defValue)
        {
            DateTime ret = defValue;
            try
            {
                ret = Convert.ToDateTime(val);
            }
            catch
            {
            }

            return ret;
        }

        public static DateTime ToDateTime(object val)
        {
            return ToDateTime(val, DateTime.Now);
        }

        public static bool ToBoolean(object o)
        {
            bool retVal;

            try
            {
                retVal = Convert.ToBoolean(o);
            }
            catch
            {
                retVal = false;
            }
            return retVal;
        }

        //Định dạng ngày tháng 
        public static string ToLongDateTimeString(DateTime dt)
        {
            return dt.ToString("\\'MM-dd-yyyy HH:mm:ss\\'");
        }

        public static string ToDateTimeFormat(DateTime dt, string sFormat)
        {
            return dt.ToString(sFormat);
        }

        //Chuyển chuỗi thành kiểu datetime ngắn MM-dd-yyyy
        public static string ToShortDateString(string strDateTime)
        {
            return DateTime.Parse(strDateTime).ToString("\\'MM-dd-yyyy\\'");
        }

    }
}