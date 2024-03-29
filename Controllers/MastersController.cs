﻿using Smart_Tailoring_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Configuration;

namespace Smart_Tailoring_WebAPI.Controllers
{
    public class MastersController : ApiController
    {
        // Instantiate random number generator.  
        private readonly Random _random = new Random();
        // Generates a random number within a range.  

        string strDBName = ConfigurationManager.AppSettings["DBName"];

        clsCoreApp ObjDAL = new clsCoreApp();

        private string[] ExceptionLog(Exception ex, bool IsAutoLog = true)
        {
            string[] strError = new string[3];

            string strUrl = ControllerContext.Request.RequestUri.AbsoluteUri;

            string strControllername = ControllerContext.Request.RequestUri.Segments[2].ToString();
            string strMethod = ((System.Web.Http.ApiController)ControllerContext.Controller).Url.Request.RequestUri.Segments[3];

            if (IsAutoLog)
                ObjDAL.WriteBackupLog(strControllername.Replace("/", "Controller"), strMethod, strUrl, ex.ToString());
            else
            {
                strError[0] = strControllername;
                strError[1] = strMethod;
                strError[2] = strUrl;
            }
            return strError;
        }

        public IEnumerable<Customer> GetCustomerDetails(int lastChange)
        {
            List<Customer> lstcustomers = new List<Customer>();
            try
            {
                ObjDAL.SetStoreProcedureData("LastChange", System.Data.SqlDbType.BigInt, lastChange);
                DataSet dsCustomer = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".[dbo].[SPR_Sync_Customer]");
                if (dsCustomer != null && dsCustomer.Tables.Count > 0)
                {
                    DataTable dtCustomer = dsCustomer.Tables[0];

                    lstcustomers = (from DataRow dr in dtCustomer.Rows
                                    select new Customer()
                                    {
                                        CustomerID = Convert.ToInt32(dr["CustomerID"]),
                                        Name = dr["Name"].ToString(),
                                        Address = dr["Address"].ToString(),
                                        MobileNo = dr["MobileNo"].ToString(),
                                        EmailID = dr["EmailID"].ToString(),
                                        LastChange = dr["LastChange"].ToString()
                                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstcustomers;
        }

        public IEnumerable<UserManagement> GetUserManagementDetails(int lastChange)
        {
            List<UserManagement> lstusers = new List<UserManagement>();
            try
            {
                ObjDAL.SetStoreProcedureData("LastChange", System.Data.SqlDbType.BigInt, lastChange);
                DataSet dsUser = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".[dbo].[SPR_Sync_UserManagement]");
                if (dsUser != null && dsUser.Tables.Count > 0)
                {
                    DataTable dtUser = dsUser.Tables[0];

                    lstusers = (from DataRow dr in dtUser.Rows
                                select new UserManagement()
                                {
                                    UserID = Convert.ToInt32(dr["UserID"]),
                                    EmployeeID = Convert.ToInt32(dr["EmployeeID"]),
                                    ActiveStatus = Convert.ToInt32(dr["ActiveStatus"]),
                                    UserName = dr["UserName"].ToString(),
                                    Password = dr["Password"].ToString(),
                                    EmailID = dr["EmailID"].ToString(),
                                    LastChange = Convert.ToInt32(dr["LastChange"])
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstusers;
        }

        public List<Customer> Sync_CustomerData(List<Customer> lstCustomerList)
        {
            List<Customer> response = new List<Customer>();
            try
            {
                for (int i = 0; i < lstCustomerList.Count; i++)
                {
                    // check if customer ID exists then update 
                    int IsExist = ObjDAL.ExecuteScalarInt("SELECT COUNT(1) FROM " + strDBName + ".dbo.CustomerMaster WITH(NOLOCK) WHERE CustomerID=" + lstCustomerList[i].CustomerID);
                    if (IsExist > 0)
                    {
                        ObjDAL.UpdateColumnData("Name", SqlDbType.NVarChar, lstCustomerList[i].Name);
                        ObjDAL.UpdateColumnData("Address", SqlDbType.NVarChar, lstCustomerList[i].Address);
                        ObjDAL.UpdateColumnData("MobileNo", SqlDbType.VarChar, lstCustomerList[i].MobileNo);
                        ObjDAL.UpdateColumnData("EmailID", SqlDbType.VarChar, lstCustomerList[i].EmailID);
                        ObjDAL.UpdateColumnData("UpdatedBy", SqlDbType.Int, 0);
                        ObjDAL.UpdateColumnData("UpdatedOn", SqlDbType.DateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        ObjDAL.UpdateData(strDBName + ".dbo.CustomerMaster", "CustomerID=" + lstCustomerList[i].CustomerID);

                        int LastChangeID = ObjDAL.ExecuteScalarInt("SELECT CONVERT(INT,LASTCHANGE) FROM " + strDBName + ".dbo.CustomerMaster WITH(NOLOCK) WHERE CustomerID=" + lstCustomerList[i].CustomerID);

                        // Make a new customer so that data can be updated back to mobile device.
                        response.Add(new Customer
                        {
                            CustomerID = lstCustomerList[i].CustomerID,
                            Name = lstCustomerList[i].Name,
                            Address = lstCustomerList[i].Address,
                            EmailID = lstCustomerList[i].EmailID,
                            LastChange = LastChangeID.ToString(),
                            MB_CustomerID = lstCustomerList[i].MB_CustomerID,
                            MobileNo = lstCustomerList[i].MobileNo
                        });
                    }
                    else
                    {
                        // check if customer ID is new then add 

                        ObjDAL.SetColumnData("Name", SqlDbType.NVarChar, lstCustomerList[i].Name);
                        ObjDAL.SetColumnData("Address", SqlDbType.NVarChar, lstCustomerList[i].Address);
                        ObjDAL.SetColumnData("MobileNo", SqlDbType.VarChar, lstCustomerList[i].MobileNo);
                        ObjDAL.SetColumnData("EmailID", SqlDbType.VarChar, lstCustomerList[i].EmailID);
                        ObjDAL.SetColumnData("CreatedBy", SqlDbType.Int, 0);

                        int CustID = ObjDAL.InsertData(strDBName + ".dbo.CustomerMaster", true);

                        int LastChangeID = ObjDAL.ExecuteScalarInt("SELECT CONVERT(INT,LASTCHANGE) FROM " + strDBName + ".dbo.CustomerMaster WITH(NOLOCK) WHERE CustomerID=" + CustID);

                        // Make a new customer so that data can be updated back to mobile device.
                        response.Add(new Customer
                        {
                            CustomerID = CustID,
                            Name = lstCustomerList[i].Name,
                            Address = lstCustomerList[i].Address,
                            EmailID = lstCustomerList[i].EmailID,
                            //LastChange = LastChangeID.ToString(),
                            //MB_CustomerID = lstCustomerList[i].MB_CustomerID,
                            MobileNo = lstCustomerList[i].MobileNo
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strError = ExceptionLog(ex, false);
                ObjDAL.WriteBackupLog(strError[0], strError[1], strError[2], "CustomerID : " + lstCustomerList[0].CustomerID + " " + ex.ToString());
            }
            return response;
        }

        public List<UserManagement> Sync_UserManagementData(List<UserManagement> lstUserList)
        {
            List<UserManagement> response = new List<UserManagement>();
            try
            {
                for (int i = 0; i < lstUserList.Count; i++)
                {
                    // check if customer ID exists then update 
                    int IsExist = ObjDAL.ExecuteScalarInt("SELECT COUNT(1) FROM " + strDBName + ".dbo.UserManagement WITH(NOLOCK) WHERE UserID=" + lstUserList[i].UserID);
                    if (IsExist > 0)
                    {
                        ObjDAL.UpdateColumnData("UserName", SqlDbType.NVarChar, lstUserList[i].UserName);
                        ObjDAL.UpdateColumnData("Password", SqlDbType.NVarChar, lstUserList[i].Password);
                        ObjDAL.UpdateColumnData("EmailID", SqlDbType.VarChar, lstUserList[i].EmailID);
                        ObjDAL.UpdateColumnData("ActiveStatus", SqlDbType.VarChar, lstUserList[i].ActiveStatus);
                        ObjDAL.UpdateColumnData("UpdatedBy", SqlDbType.Int, 0);
                        ObjDAL.UpdateColumnData("UpdatedOn", SqlDbType.DateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        ObjDAL.UpdateData(strDBName + ".dbo.UserManagement", "UserID=" + lstUserList[i].UserID);

                        int LastChangeID = ObjDAL.ExecuteScalarInt("SELECT CONVERT(INT,LASTCHANGE) FROM " + strDBName + ".dbo.UserManagement WITH(NOLOCK) WHERE UserID=" + lstUserList[i].UserID);

                        // Make a new customer so that data can be updated back to mobile device.
                        response.Add(new UserManagement
                        {
                            UserID = lstUserList[i].UserID,
                            UserName = lstUserList[i].UserName,
                            EmailID = lstUserList[i].EmailID,
                            ActiveStatus = lstUserList[i].ActiveStatus,
                            LastChange = LastChangeID,
                            MB_UserID = lstUserList[i].MB_UserID
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strError = ExceptionLog(ex, false);

                ObjDAL.WriteBackupLog(strError[0], strError[1], strError[2], "UserID : " + lstUserList[0].UserID + " " + ex.ToString());
            }
            return response;
        }

        // for checking connectivity
        public Response GetStatus()
        {
            return new Response { Result = true, Message = "Connection OK", Value = 1 };
        }

        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public Response ValidateActivation(ActivationDetails activationDetails)
        {
            int count = ObjDAL.ExecuteScalarInt("SELECT COUNT(1) FROM " + strDBName + ".[dbo].[tblMobileActivation] WITH(NOLOCK) WHERE ActivationCode='" + activationDetails.ActivationCode + "' AND SerialNumber='" + activationDetails.DeviceSerialNumber + "'");
            if (count > 0)
            {
                return new Response { Result = true, Message = "Application has been activated!", Value = 1 };
            }
            else
            {
                return new Response { Result = false, Message = "Invalid Activation Code", Value = 0 };
            }
        }

        public Response ProcessActivationRequest(ActivationDetails activationDetails)
        {
            // delete activation if alerady exsit
            ObjDAL.ExecuteNonQuery("DELETE FROM " + strDBName + ".[dbo].[tblMobileActivation] WHERE SerialNumber='" + activationDetails.DeviceSerialNumber + "'");

            int isCodeExist = 1;
            int ActivationCode = 0;
            // keep generating the activation code if it is already exist in the system
            while (isCodeExist > 0) // Need to Generate code from SQL
            {
                ActivationCode = RandomNumber(1000, 9999);
                isCodeExist = ObjDAL.ExecuteScalarInt("SELECT COUNT(1) FROM " + strDBName + ".[dbo].[tblMobileActivation] WHERE ActivationCode='" + ActivationCode + "'");
            }

            ObjDAL.SetStoreProcedureData("SerialNumber", System.Data.SqlDbType.NVarChar, activationDetails.DeviceSerialNumber);
            ObjDAL.SetStoreProcedureData("ActivationCode", System.Data.SqlDbType.NVarChar, ActivationCode);
            ObjDAL.ExecuteStoreProcedure_DML(strDBName + ".dbo.SPR_Insert_MobileActivation");

            return new Response { Result = true, Message = "ActivationSuccess", Value = ActivationCode };
        }

        public Response ValidateLogin(UserManagement UserDetails)
        {
            int UserID = 0;
            UserID = ObjDAL.ExecuteScalarInt("SELECT EmployeeID FROM " + strDBName + ".[dbo].[UserManagement] WITH(NOLOCK) WHERE UserName='" + UserDetails.UserName + "' AND Password='" + ObjDAL.Encrypt(UserDetails.Password, true) + "' AND ISNULL(ActiveStatus,1)=1");
            if (UserID > 0)
            {
                UserDetails.UserID = UserID;
                return new Response { Result = true, Message = "Validate Sucessfully!", Value = UserID };
            }
            else
            {
                return new Response { Result = false, Message = "Invalid Login Details", Value = 0 };
            }
        }

        public IEnumerable<Employee> GetEmployeeDetails(int EmpID)
        {
            List<Employee> lstEmployees = new List<Employee>();
            try
            {
                ObjDAL.SetStoreProcedureData("EmpID", System.Data.SqlDbType.Int, EmpID);
                DataSet Employee = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".[dbo].[SPR_Get_Employee]");
                if (Employee != null && Employee.Tables.Count > 0)
                {
                    DataTable dtEmployee = Employee.Tables[0];

                    lstEmployees = (from DataRow dr in dtEmployee.Rows
                                    select new Employee()
                                    {
                                        EmpID = Convert.ToInt32(dr["EmpID"]),
                                        Name = dr["Name"].ToString(),
                                        Address = dr["Address"].ToString(),
                                        MobileNo = dr["MobileNo"].ToString(),
                                        Gender = dr["Gender"].ToString(),
                                        EmployeeCode = dr["EmployeeCode"].ToString(),
                                        EmployeeType = dr["EmployeeType"].ToString(),
                                        ActiveStatus = dr["ActiveStatus"].ToString(),
                                        LastChange = Convert.ToInt32(dr["LastChange"])
                                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstEmployees;
        }

    }
}