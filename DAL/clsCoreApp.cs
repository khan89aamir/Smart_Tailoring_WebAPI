using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for clsCoreApp
/// </summary>
public  class clsCoreApp 
{
  
   
    public string strErrorText = "";
    private int Counter = 0;
    private string strColumns;
    private string strValues;
    DataTable dtOutputParm = new DataTable();
    // List for storing sql parameter.
    private char[] c1 = new char[2];
   
    List<SqlParameter> lstSQLParameter = new List<SqlParameter>();
    public clsCoreApp()
    {
        c1[0] = '[';
        c1[1] = ']';
    }

    public enum ParamType
    {
        Input,
        Output
    }

    public bool ResetData()
    {
        try
        {
            // IMP Note : Never make transaction object Null over here.
            Counter = 0;
            strColumns = string.Empty;
            strValues = string.Empty;
            lstSQLParameter.Clear();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    int MyValue = 0;
    public void SetValue(int value)
    {
        MyValue = value;

    }
    public int GetValue()
    {
        return MyValue;

    }

    public bool SetStoreProcedureData(string strParamterName, SqlDbType DataType, object Value, ParamType parameterType = ParamType.Input)
    {
        try
        {
            if (Counter == 0)
            {
                strColumns = strParamterName;
                strParamterName = strParamterName.Trim(c1).Replace(" ", string.Empty);
                strValues = "@" + strParamterName;
            }
            else
            {
                strColumns += "," + strParamterName;
                strParamterName = strParamterName.Trim(c1).Replace(" ", string.Empty);
                strValues += ",@" + strParamterName;
            }
            SqlParameter p = new SqlParameter("@" + strParamterName.Trim(c1).Replace(" ", string.Empty), DataType);
            p.Value = Value;
            if (parameterType == ParamType.Input)
            {
                p.Direction = ParameterDirection.Input;
            }
            else if (parameterType == ParamType.Output)
            {
                p.Direction = ParameterDirection.Output;
                
                // set the Max size by default for below parm
                if (p.SqlDbType == SqlDbType.NVarChar || p.SqlDbType == SqlDbType.Text || p.SqlDbType == SqlDbType.VarChar || p.SqlDbType == SqlDbType.VarBinary)
                {
                    p.Size = -1;
                }
                else if (p.SqlDbType == SqlDbType.Decimal)
                {
                    p.Precision = 18;
                    p.Scale = 3;
                }
            }

            lstSQLParameter.Add(p);
            Counter++;
            return true;
        }
        catch (Exception ex)
        {
            strErrorText = ex.ToString();
            ResetData();
            return false;
        }
    }
    public DataTable ExecuteSelectStatement(string query)
    {
        DataTable dtTable = new DataTable();
        dtTable.TableName = "MyTable";
        try
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        con.Open();
                        sda.Fill(dtTable);
                    }
                }
            }
        }
        catch (Exception  ex)
        {
            strErrorText = ex.ToString();
           
        }
        return dtTable;
    }
    public int ExecuteNonQuery(string query)
    {
        int result = -1;
        try
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    result= cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        catch (Exception ex)
        {
            strErrorText = ex.ToString();
        }

        return result;
    }
    public object ExecuteScalarQuery(string query)
    {
        object result  = 0;
        try
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    result = cmd.ExecuteScalar();


                    con.Close();
                }
            }
        }
        catch (Exception ex)
        {
            strErrorText = ex.ToString();
        }

        return result;
    }

    public int ExecuteScalarInt(string query)
    {
        int result = 0;
        try
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    if (cmd.ExecuteScalar()!=null)
                    {
                        result = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        result = 0;
                    }


                    con.Close();
                }
            }
        }
        catch (Exception ex)
        {
            strErrorText = ex.ToString();
        }

        return result;
    }
    private void InitOutputTable()
    {
        if (dtOutputParm.Columns.Count == 0)
        {
            dtOutputParm.Columns.Add("ParmName");
            dtOutputParm.Columns.Add("Value", typeof(object));
        }
    }
    private void AddRowToOutputParm(string name, object value)
    {
        DataRow dataRow = dtOutputParm.NewRow();
        dataRow["ParmName"] = name.Replace("@", "");
        dataRow["Value"] = value;

        dtOutputParm.Rows.Add(dataRow);
    }
    public DataTable GetOutputParmData()
    {
        return dtOutputParm;
    }
    /// <summary>
    /// Execute the store Procedure for DML operation and  and returns true or false
    /// </summary>
    /// <param name="strStoreProcedureName">Name of the procedure</param>
    /// <returns>Operation success result</returns>
    public bool ExecuteStoreProcedure_DML(string strStoreProcedureName)
    {
        bool result = false;
        if (dtOutputParm != null && dtOutputParm.Rows.Count > 0)
        {
            dtOutputParm.Clear();
        }       
        try
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con=new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strStoreProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;

                // if sp is called with parameters.
                if (lstSQLParameter.Count > 0)
                {
                    SqlParameter[] p = lstSQLParameter.ToArray();
                    cmd.Parameters.AddRange(p);
                }
                con.Open();
                cmd.ExecuteNonQuery();

                // check if there is any output parameter.
                for (int i = 0; i < cmd.Parameters.Count; i++)
                {
                    if (cmd.Parameters[i].Direction == ParameterDirection.Output)
                    {
                        InitOutputTable();
                        AddRowToOutputParm(cmd.Parameters[i].ParameterName, cmd.Parameters[i].Value);
                    }
                }
                result = true;
                con.Close();
            }
        }
        catch (Exception ex)
        {
            strErrorText = ex.ToString();
            ResetData();
            result = false;
        }
        ResetData();
        return result;
    }
    public bool UpdateColumnData(string strColumnnName, SqlDbType DataType, object Value)
    {
        try
        {
            if (Counter == 0)
            {
                strColumns = strColumnnName + "=@" + strColumnnName.Trim(c1).Replace(" ", string.Empty);
                strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                strValues = "@" + strColumnnName;
            }
            else
            {
                strColumns += "," + strColumnnName + "=@" + strColumnnName.Trim(c1).Replace(" ", string.Empty);
                strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                strValues += ",@" + strColumnnName;
            }
            SqlParameter p = new SqlParameter("@" + strColumnnName, DataType);
            p.Value = Value;
            lstSQLParameter.Add(p);
            Counter++;
            return true;
        }
        catch (Exception ex)
        {
         
            ResetData();
            return false;
        }
    }

    public int InsertData(string strTableName, bool ReturnIdentity)
    {
      
        int result = 0;
        SqlCommand cmd = new SqlCommand();
        try
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con=new SqlConnection(constr))
            {
                SqlParameter[] p = lstSQLParameter.ToArray();

                if (ReturnIdentity)
                {
                    cmd.CommandText = "INSERT INTO " + strTableName + "(" + strColumns + ") VALUES(" + strValues + "); SELECT SCOPE_IDENTITY()";
                }
                else
                {
                    cmd.CommandText = "INSERT INTO " + strTableName + "(" + strColumns + ") VALUES(" + strValues + ")";
                }

                cmd.Parameters.AddRange(p);
                cmd.Connection = con;
                con.Open();
                // if user want identity value then get the identity value.
                if (ReturnIdentity)
                {
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    result = cmd.ExecuteNonQuery();
                }
                con.Close();

                ResetData();
            }

         
          
        }
        catch (Exception ex)
        {
         
            ResetData();
            return -1;
        }
        ResetData();
        return result;
    }

    public int UpdateData(string strTableName, string strCondition)
    {
       
        int result = 0;
        SqlCommand cmd = new SqlCommand();
        try
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con=new SqlConnection(constr))
            {
                SqlParameter[] p = lstSQLParameter.ToArray();
                con.Open();
                strCondition = strCondition.Replace("where", " ");
                cmd.CommandText = "UPDATE " + strTableName + " SET " + strColumns + " WHERE " + strCondition;

                cmd.Parameters.AddRange(p);
                cmd.Connection = con;
                result = cmd.ExecuteNonQuery();
                con.Close();
                ResetData();
            }
           
         
        }
        catch (Exception ex)
        {
            ResetData();
            return -1;
        }
        ResetData();
        return result;
    }

    /// <summary>
    /// Execute the store Procedure and returns the data table.
    /// </summary>
    /// <param name="strStoreProcedureName">Name of the procedure.</param>
    /// <returns>Data Set</returns>
    public DataSet ExecuteStoreProcedure_Get(string strStoreProcedureName)
    {
        // clear the output parm table for fresh data.
        if (dtOutputParm != null && dtOutputParm.Rows.Count > 0)
        {
            dtOutputParm.Clear();
        }
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        DataSet ds = new DataSet();
        using (SqlConnection con=new SqlConnection(constr))
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
               SqlDataAdapter ObjDA = new SqlDataAdapter();
                cmd.CommandText = strStoreProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;

                // if sp is called with parameters.
                if (lstSQLParameter.Count > 0)
                {
                    SqlParameter[] p = lstSQLParameter.ToArray();
                    cmd.Parameters.AddRange(p);
                }
                con.Open();
                ObjDA.SelectCommand = cmd;
                ObjDA.Fill(ds);

                // check if there is any output parameter.
                for (int i = 0; i < cmd.Parameters.Count; i++)
                {
                    if (cmd.Parameters[i].Direction == ParameterDirection.Output)
                    {
                        InitOutputTable();
                        AddRowToOutputParm(cmd.Parameters[i].ParameterName, cmd.Parameters[i].Value);
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                strErrorText = ex.ToString();
                ResetData();
                return null;
            }
            ResetData();
        }
        return ds;
    }

    public bool SetColumnData(string strColumnnName, SqlDbType DataType, object Value)
    {
        try
        {
            if (Counter == 0)
            {
                strColumns = strColumnnName;
                strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                strValues = "@" + strColumnnName;
            }
            else
            {
                strColumns += "," + strColumnnName;
                strColumnnName = strColumnnName.Trim(c1).Replace(" ", string.Empty);
                strValues += ",@" + strColumnnName;
            }
            SqlParameter p = new SqlParameter("@" + strColumnnName.Trim(c1).Replace(" ", string.Empty), DataType);
            p.Value = Value;
            lstSQLParameter.Add(p);
            Counter++;
            return true;
        }
        catch (Exception ex)
        {
            ResetData();
            return false;
        }
    }

    /// <summary>
    /// Encrypt a string using dual encryption method. Return a encrypted  Text
    /// </summary>
    /// <param name="toEncrypt">string to be encrypted</param>
    /// <param name="useHashing">use hashing? send to for extra security</param>
    /// <returns></returns>
    public string Encrypt(string toEncrypt, bool useHashing)
    {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

        string key = "abdulmateen1989";

        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
        }
        else
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        tdes.Key = keyArray;
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tdes.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    /// <summary>
    /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
    /// If you had encrypted a string by passing true to UseHashing parameter then you must pass true 
    /// while decrypting the string.
    /// </summary>
    /// <param name="cipherString">encrypted string</param>
    /// <param name="useHashing">Did you use hashing to encrypt this data? pass true if yes</param>
    /// <returns></returns>
    public string Decrypt(string cipherString, bool useHashing)
    {
        string strDecryptString = "";
        try
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            //Get your key from config file to open the lock!
            string key = "abdulmateen1989";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            strDecryptString = UTF8Encoding.UTF8.GetString(resultArray);
            return strDecryptString;
        }
        catch (FormatException)
        {
            strDecryptString = null;
        }
        return strDecryptString;
    }
}