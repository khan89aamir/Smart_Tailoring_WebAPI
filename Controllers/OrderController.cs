﻿using Smart_Tailoring_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Smart_Tailoring_WebAPI.Models.OrderModel;
using System.Collections;

namespace Smart_Tailoring_WebAPI.Controllers
{
    public class OrderController : ApiController
    {
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

        public IEnumerable<clsMeasurment> GetGarmentMasterMeasurement(int GarmentID)
        {
            List<clsMeasurment> lstMeasurment = new List<clsMeasurment>();
            try
            {
                DataTable dtMeasurement = new DataTable();
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, GarmentID);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_GarmentMeasurement");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtMeasurement = ds.Tables[0];

                    lstMeasurment = (from DataRow dr in dtMeasurement.Rows
                                     select new clsMeasurment()
                                     {
                                         MeasurmentID = Convert.ToInt32(dr["MeasurementID"]),
                                         MeasurmentName = dr["MeasurementName"].ToString(),
                                         IsMandatory = Convert.ToBoolean(dr["IsMandatory"]),
                                         //value = "0"
                                     }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstMeasurment;
        }

        public IEnumerable<clsStyle> GetGarmentStyle(int GarmentID)
        {
            List<clsStyle> lstStyle = new List<clsStyle>();
            try
            {
                DataTable dtStyle = new DataTable();
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, GarmentID);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_GarmentStyle");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtStyle = ds.Tables[0];

                    lstStyle = (from DataRow dr in dtStyle.Rows
                                select new clsStyle()
                                {
                                    StyleID = Convert.ToInt32(dr["StyleID"]),
                                    StyleName = dr["StyleName"].ToString(),
                                    IsMandatory = Convert.ToBoolean(dr["IsMandatory"]),
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstStyle;
        }

        public IEnumerable<clsBodyPosture> GetBodyPosture(int GarmentID)
        {
            List<clsBodyPosture> lstBodyPosture = new List<clsBodyPosture>();
            try
            {
                DataTable dtBodyPosture = new DataTable();
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, GarmentID);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_BodyPosture_ByGarmentID");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtBodyPosture = ds.Tables[0];

                    lstBodyPosture = (from DataRow dr in dtBodyPosture.Rows
                                      select new clsBodyPosture()
                                      {
                                          BodyPostureID = Convert.ToInt32(dr["BodyPostureID"]),
                                          BodyPostureType = dr["BodyPostureType"].ToString(),
                                          lstImage = GetBodyPostureImagesByBodyPostureID(GarmentID, Convert.ToInt32(dr["BodyPostureID"]))
                                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstBodyPosture;
        }

        private List<clsImageList> GetBodyPostureImagesByBodyPostureID(int pGarmentID, int BodyPostureID)
        {
            List<clsImageList> lstBodyPosture = new List<clsImageList>();
            try
            {
                DataTable dtBodyPostureImages = new DataTable();
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, pGarmentID, clsCoreApp.ParamType.Input);
                ObjDAL.SetStoreProcedureData("BodyPostureID", SqlDbType.Int, BodyPostureID, clsCoreApp.ParamType.Input);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_BodyPosture_Images_ByGarmentID");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtBodyPostureImages = ds.Tables[0];

                    lstBodyPosture = (from DataRow dr in dtBodyPostureImages.Rows
                                      select new clsImageList()
                                      {
                                          GarmentID = pGarmentID,
                                          BodyPostureID = Convert.ToInt32(dr["BodyPostureID"]),
                                          ImageURL = "/Images/" + dr["Photo1"].ToString()
                                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstBodyPosture;
        }

        public IEnumerable<StitchType> GetStitchType()
        {
            List<StitchType> lstStichType = new List<StitchType>();
            try
            {
                DataTable dtStichType = new DataTable();
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_StichType");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtStichType = ds.Tables[0];

                    lstStichType = (from DataRow dr in dtStichType.Rows
                                    select new StitchType()
                                    {
                                        StichTypeID = Convert.ToInt32(dr["StichTypeID"]),
                                        StichTypeName = dr["StichTypeName"].ToString(),
                                        LastChange = Convert.ToInt32(dr["LastChange"])
                                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstStichType;
        }

        public IEnumerable<FitType> GetFitType()
        {
            List<FitType> lstFitType = new List<FitType>();
            try
            {
                DataTable dtFitType = new DataTable();
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_FitType");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtFitType = ds.Tables[0];

                    lstFitType = (from DataRow dr in dtFitType.Rows
                                  select new FitType()
                                  {
                                      FitTypeID = Convert.ToInt32(dr["FitTypeID"]),
                                      FitTypeName = dr["FitTypeName"].ToString(),
                                      LastChange = Convert.ToInt32(dr["LastChange"])
                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstFitType;
        }

        [HttpPost]
        public IEnumerable<SelectedGarments> GetGarmentRateListBySelectedGarment(ArrayList paramList)
        {
            List<SelectedGarments> lstSelected = new List<SelectedGarments>();
            try
            {
                if (paramList.Count > 0)
                {
                    lstSelected = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SelectedGarments>>(paramList[0].ToString());

                    DataTable dt = ObjDAL.ToDataTable(lstSelected);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        lstSelected = (from DataRow dr in dt.Rows
                                       select new SelectedGarments()
                                       {
                                           GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                           Name = dr["Name"].ToString(),
                                           Photo = dr["Photo"].ToString(),
                                           GarmentCode = dr["GarmentCode"].ToString(),
                                           QTY = Convert.ToInt32(dr["QTY"]),
                                           OrderType = dr["OrderType"].ToString(),
                                           OrderTypeID = Convert.ToInt32(dr["OrderTypeID"]),
                                           Rate = Convert.ToDouble(dr["Rate"]),
                                           TrimAmount = Convert.ToDouble(dr["TrimAmount"]),
                                           IsTrail = Convert.ToBoolean(dr["IsTrail"]),
                                           DeliveryDate = Convert.ToDateTime(dr["DeliveryDate"]),
                                           TrailDate = Convert.ToDateTime(dr["TrailDate"]),
                                           lstGarmentRate = GetGarmentRate(Convert.ToInt32(dr["GarmentID"]), 2)
                                       }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstSelected;
        }

        public List<clsGarmentRate> GetGarmentRate(int GarmentID, int ServiceType = 0)
        {
            //GarmentID = 0
            //ServiceType = 2 and it will return all garments rate data
            List<clsGarmentRate> lstGarmentRate = new List<clsGarmentRate>();
            try
            {
                DataTable dtGarmentRate = new DataTable();
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, GarmentID, clsCoreApp.ParamType.Input);
                ObjDAL.SetStoreProcedureData("OrderType", SqlDbType.Int, ServiceType, clsCoreApp.ParamType.Input);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_Product_Rate");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtGarmentRate = ds.Tables[0];

                    lstGarmentRate = (from DataRow dr in dtGarmentRate.Rows
                                      select new clsGarmentRate()
                                      {
                                          GarmentRateID = Convert.ToInt32(dr["GarmentRateID"]),
                                          GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                          GarmentCode = dr["GarmentCode"].ToString(),
                                          GarmentName = dr["GarmentName"].ToString(),
                                          GarmentCodeName = dr["GarmentCodeName"].ToString(),
                                          GarmentType = dr["GarmentType"].ToString(),
                                          OrderType = dr["OrderType"].ToString(),
                                          OrderTypeID = Convert.ToInt32(dr["OrderTypeID"]),
                                          Rate = Convert.ToDouble(dr["Rate"]),
                                          LastChange = Convert.ToInt32(dr["LastChange"])
                                      }).ToList();

                    lstGarmentRate = lstGarmentRate.OrderBy(lst => lst.OrderTypeID).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstGarmentRate;
        }

        public IEnumerable<GarmentList> GetGarmentList(int GarmentID = 0)
        {
            //GarmentID = 0 return all garments
            List<GarmentList> lstGarmentList = new List<GarmentList>();
            try
            {
                DataTable dtGarmentList = new DataTable();
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, GarmentID, clsCoreApp.ParamType.Input);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_Product");
                if (ds != null && ds.Tables.Count > 0)
                {
                    dtGarmentList = ds.Tables[0];

                    lstGarmentList = (from DataRow dr in dtGarmentList.Rows
                                      select new GarmentList()
                                      {
                                          GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                          Name = dr["GarmentName"].ToString(),
                                          GarmentType = dr["GarmentType"].ToString(),
                                          ImageURL = "/Images/Generic/" + dr["Photo1"].ToString(),
                                      }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return lstGarmentList;
        }

        public ArrayList CopyGarmentDetails_LastOrder(int CustomerID, int GarmentID, int MasterGarmentID)
        {
            ArrayList array = new ArrayList();

            //List<CustomerStichFitType> stichFitType = new List<CustomerStichFitType>();

            CustomerStichFitType stichFitType = new CustomerStichFitType();
            List<CustomerMeasurement> measure = new List<CustomerMeasurement>();
            List<CustomerStyle> style = new List<CustomerStyle>();
            List<CustomerBodyPosture> body = new List<CustomerBodyPosture>();
            try
            {
                ObjDAL.SetStoreProcedureData("CustomerID", SqlDbType.Int, CustomerID, clsCoreApp.ParamType.Input);
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, GarmentID, clsCoreApp.ParamType.Input);
                ObjDAL.SetStoreProcedureData("MasterGarmentID", SqlDbType.Int, MasterGarmentID, clsCoreApp.ParamType.Input);
                DataSet ds = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_GarmentMeasurementStyle_CopyOrder");
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["SalesOrderID"]) != 0)
                        {
                            stichFitType = (from DataRow dr in dt.Rows
                                            select new CustomerStichFitType()
                                            {
                                                MasterGarmentID = Convert.ToInt32(dr["MasterGarmentID"]),
                                                GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                                StichTypeID = Convert.ToInt32(dr["StichTypeID"]),
                                                FitTypeID = Convert.ToInt32(dr["FitTypeID"]),
                                                OrderDate = dr["OrderDate"].ToString(),
                                            }).Single();

                            DataTable dtmeasure = ds.Tables[1];
                            measure = (from DataRow dr in dtmeasure.Rows
                                       select new CustomerMeasurement()
                                       {
                                           MasterGarmentID = Convert.ToInt32(dr["MasterGarmentID"]),
                                           GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                           MeasurementID = Convert.ToInt32(dr["MeasurementID"]),
                                           MeasurementValue = Convert.ToDecimal(dr["MeasurementValue"])
                                       }).ToList();

                            DataTable dtstyle = ds.Tables[2];
                            style = (from DataRow dr in dtstyle.Rows
                                     select new CustomerStyle()
                                     {
                                         MasterGarmentID = Convert.ToInt32(dr["MasterGarmentID"]),
                                         GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                         QTY = Convert.ToInt32(dr["QTY"]),
                                         StyleID = Convert.ToInt32(dr["StyleID"]),
                                         StyleImageID = Convert.ToInt32(dr["StyleImageID"])
                                     }).ToList();

                            DataTable dtbody = ds.Tables[3];
                            body = (from DataRow dr in dtbody.Rows
                                    select new CustomerBodyPosture()
                                    {
                                        MasterGarmentID = Convert.ToInt32(dr["MasterGarmentID"]),
                                        GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                        BodyPostureID = Convert.ToInt32(dr["BodyPostureID"]),
                                        BodyPostureMappingID = Convert.ToInt32(dr["BodyPostureMappingID"])
                                    }).ToList();

                            array.Add(stichFitType);
                            array.Add(measure);
                            array.Add(style);
                            array.Add(body);

                            //if (ds.Tables.Count > 1)
                            //    CopyMeasurement_LastOrder(ds.Tables[1]);

                            //if (ds.Tables.Count > 2)
                            //    CopyStyle_LastOrder(ds.Tables[2]);

                            //if (ds.Tables.Count > 3)
                            //    CopyBodyPosture_LastOrder(ds.Tables[3]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return array;
        }

        public IEnumerable<CustomerMeasurement> CopyCommonMeasurement(int GarmentID)
        {
            List<CustomerMeasurement> customerMeasurements = new List<CustomerMeasurement>();
            try
            {
                ObjDAL.SetStoreProcedureData("GarmentID", SqlDbType.Int, GarmentID, clsCoreApp.ParamType.Input);
                DataSet dscommon = ObjDAL.ExecuteStoreProcedure_Get(strDBName + ".dbo.SPR_Get_CommonMeasurement");
                if (dscommon != null && dscommon.Tables.Count > 0)
                {
                    DataTable dtcommon = dscommon.Tables[0];
                    customerMeasurements = (from DataRow dr in dtcommon.Rows
                                            select new CustomerMeasurement()
                                            {
                                                GarmentID = Convert.ToInt32(dr["GarmentID"]),
                                                MeasurementID = Convert.ToInt32(dr["MeasurementID"]),
                                                MeasurementValue = 0
                                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
            }
            return customerMeasurements;
        }

        [HttpPost]
        public Response SavedSalesOrder(System.Collections.ArrayList paramList)
        {
            if (paramList.Count > 0)
            {
                SalesOrder salesorder = Newtonsoft.Json.JsonConvert.DeserializeObject<SalesOrder>(paramList[0].ToString());

                return new Response { Result = true, Message = "Temporary Saved!", Value = 1 };
            }
            else
            {
                return new Response { Result = false, Message = "Failed", Value = 0 };
            }
        }

        [HttpPost]
        public Response SavedSalesOrderDetails(System.Collections.ArrayList paramList)
        {
            if (paramList.Count > 0)
            {
                CustomerMeasurement Measurment = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomerMeasurement>(paramList[0].ToString());

                CustomerStyle Style = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomerStyle>(paramList[1].ToString());

                CustomerBodyPosture Posture = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomerBodyPosture>(paramList[2].ToString());

                SalesOrderDetails OrderDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<SalesOrderDetails>(paramList[3].ToString());

                return new Response { Result = true, Message = "Temporary Saved !", Value = 1 };
            }
            else
            {
                return new Response { Result = false, Message = "Failed", Value = 0 };
            }
        }
    }
}