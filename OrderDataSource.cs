/*********************************************************************
 * Screen / Process Name : Change Password
 * Called From :
 * Parameter: 
 * 
 * 
* Date		    Version	    Author			Comments
*---------------------------------------------------------------------------------
* 24-Dec-2013	 1.0.0		Naresh.A	    initial version
 *                                        
*---------------------------------------------------------------------------------
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SingPost.EzyParcelsEF;
using SingPost.EzyParcels.ExceptionHandling;
using System.Data;
using System.Data.Entity.Core.Objects;
using SingPost.EzyParcels.ApplicationCode;
using System.Configuration;
using SingPost.EzyParcels.BatchProcessCoreLogic;
using SingPost.EzyParcels.RoutingMatrixCoreLogic;
using SingPost.EzyParcels.TrackingNumberGenerationCoreLogic;
using SingPost.EzyParcels.ReportGenerationCoreLogic;

namespace SingPost.EzyParcels.Services
{
    public class OrderDataSource
    {
        #region GlobalDeclarations
        EZYPARCELSEntities _ObjContext = new EZYPARCELSEntities();
        public string _StrUploadRefNO { get; set; }
        public int _IntOrderId { get; set; }
        public List<string> _LstErrorNameReferencePair = new List<string>();
        #endregion

        public string GetUploadRefNO()
        {
            string StrRefNo = DateTime.Now.Year + "_EZYUPD_01"; 
            try
            {

                StrRefNo = _ObjContext.stp_GetBulkUploadRefNo("ORDERENTRY").ElementAt(0);
                if (string.IsNullOrEmpty(StrRefNo))
                {
                    StrRefNo = DateTime.Now.Year + "_EZYUPD_01";
                }            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjRefNoError = new ClsApplicationExceptionHandling();
                ObjRefNoError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return StrRefNo;
        }

        public string GetProcessStatus(string uploadrefno)
        {
            string strReturn = string.Empty;
            try
            {
             
                 tbl_ConsignmentInterfaceHeader ObjConsignmentInterfaceHeader = new tbl_ConsignmentInterfaceHeader();
                 Itbl_CustomerRepository ObjCustomerRepository = new tbl_CustomerRepository();
                 Itbl_ConsignmentInterfaceHeaderRepository ObjHeaderRepository = new tbl_ConsignmentInterfaceHeaderRepository();

                var  varStatus = from processstatus in ObjHeaderRepository.All
                             where processstatus.UploadRefNumber == uploadrefno
                             select processstatus.ProcessStatus;
                strReturn = varStatus.ToString();

                return strReturn;
            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return strReturn;
        }

        public void ExecuteInterfaceBulkCopy(DataTable dtOrder, string serviceType)
        {
            bool BolReturnStatus = false;
                try
                {
                     Itbl_ConsignmentInterfaceDetailsRepository ObjRepository = new tbl_ConsignmentInterfaceDetailsRepository();
                    DataColumn DColAdd = new DataColumn();
                    DColAdd = new DataColumn();
                    DColAdd.ColumnName = "OrderID";
                    DColAdd.DataType = typeof(int);
                    DColAdd.DefaultValue = _IntOrderId;
                    //DColAdd.DefaultValue = IntOrderId;
                    dtOrder.Columns.Add(DColAdd);

                    DColAdd = new DataColumn();
                    DColAdd.ColumnName = "ImportedFrom";
                    DColAdd.DataType = typeof(string);
                    DColAdd.DefaultValue = "LWMS";
                    dtOrder.Columns.Add(DColAdd);

                    //Bulk Copy data
                    BolReturnStatus = ObjRepository.LWMSInsert(dtOrder);

                }
                catch (Exception ex)
                {
                    ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                    ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
                }
                
            }
        public int IsUserExists(string userid)
        {
            int userDetail= 0;
            try
            {

                tbl_CustomerRepository tbl_customerRepository = new tbl_CustomerRepository();
                Itbl_CustomerRepository custRepository = new tbl_CustomerRepository();

                userDetail = (from customer in custRepository.All
                                  where customer.UserID.Equals(userid)
                                  select customer.CustomerID).FirstOrDefault();
            }
            catch (Exception ex) {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return userDetail;
        }
        public void GetUserDetails(string userid, ref int uId, ref string accountid, ref string userName, ref string pickupCountry, ref string pkupAddr1, ref string pkupAddr2, ref string pkupContNo, ref string pkupCity, ref string pkupPostalCode, ref string shipCountry, ref string shipCity)
        {
            try
            {
               
                tbl_CustomerRepository tbl_customerRepository = new tbl_CustomerRepository();
                tbl_CountryMasterRepository tbl_countryRepository = new tbl_CountryMasterRepository();
                tbl_AddressRepository tbl_addressRepository = new tbl_AddressRepository();
                
                IQueryable<tbl_Customer> custRepository = tbl_customerRepository.All;
                
                tbl_Address ObjAddress = new tbl_Address();
                 var custdetail = (from customer in custRepository
                                  where customer.UserID.Equals(userid)
                                  select customer).ToList().FirstOrDefault();



                 var AddressDetail = (from address in tbl_addressRepository.All
                                      where address.CustomerID == custdetail.CustomerID && address.AddressType == "Sender"
                                      select address).FirstOrDefault();
                       

                        uId = custdetail.CustomerID;
                        accountid = custdetail.AccountNumber;
                        userName = custdetail.FirstName;
                        pickupCountry = AddressDetail.CountryID;
                        pkupAddr1 = AddressDetail.Address1;
                        pkupAddr2 = AddressDetail.Address2;
                        pkupContNo = AddressDetail.ContactNumber;
                        pkupCity = AddressDetail.City;
                        pkupPostalCode = AddressDetail.PostalCode;
                                  
            }
            catch (Exception ex) {

                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            
            
            }
            
        }

        public void SaveStagingHdr(string uploadrefno, string userid, int numoffiles, string CUSTOMERACCOUNTID, DateTime PickupDate, string SenderName, string Pickupcountry, int usermasterid, string pickupTime, string pkupName, string pkupAddr1, string pkupAddr2, string pkupContNo, string pkupCity, string pkupPostalCode)
        {
            
            try
            {
               
                tbl_ConsignmentInterfaceHeader ObjConsignmentInterfaceHeader = new tbl_ConsignmentInterfaceHeader();
                Itbl_CustomerRepository ObjCustomerRepository = new tbl_CustomerRepository();
                Itbl_ConsignmentInterfaceHeaderRepository ObjHeaderRepository = new tbl_ConsignmentInterfaceHeaderRepository();
               
                var LstCustomerAccType = (from customer in ObjCustomerRepository.All
                                          where customer.UserID == userid
                                          select customer).ToList()[0];
                ObjConsignmentInterfaceHeader.UploadRefNumber = uploadrefno;
                ObjConsignmentInterfaceHeader.PickUpAddress1 = pkupAddr1;
                ObjConsignmentInterfaceHeader.PickUpFirstName = pkupName;
                ObjConsignmentInterfaceHeader.PickUpLastName = pkupName;
                ObjConsignmentInterfaceHeader.PickUpAddress2 = pkupAddr2;
                ObjConsignmentInterfaceHeader.PickUpCity = pkupCity;
                ObjConsignmentInterfaceHeader.PickUpPostalCode = pkupPostalCode;
                ObjConsignmentInterfaceHeader.PickUpCountryID = Pickupcountry;
                ObjConsignmentInterfaceHeader.PickUpContactNumber = pkupContNo;

                ObjConsignmentInterfaceHeader.PickUpDate = PickupDate;
                ObjConsignmentInterfaceHeader.PickUp = pickupTime;
                ObjConsignmentInterfaceHeader.PickUpTime1 = pickupTime;
                ObjConsignmentInterfaceHeader.PickUpTime2 = pickupTime;
                ObjConsignmentInterfaceHeader.CustomerAccountType = "C";
                ObjConsignmentInterfaceHeader.CreditLimitFlag = true;
                ObjConsignmentInterfaceHeader.CustomerType = "R";
                ObjConsignmentInterfaceHeader.TransactionType = "Bulk Upload";
                ObjConsignmentInterfaceHeader.BatchStatus = "Save";
                ObjConsignmentInterfaceHeader.CreatedBy = LstCustomerAccType.CustomerID;
                ObjConsignmentInterfaceHeader.CreatedForUserID = LstCustomerAccType.CustomerID;
                ObjConsignmentInterfaceHeader.CreatedDate = DateTime.Now;
                ObjHeaderRepository.InsertOrUpdate(ObjConsignmentInterfaceHeader);
                ObjHeaderRepository.Save();
                ObjHeaderRepository.Dispose();
                _IntOrderId = ObjConsignmentInterfaceHeader.OrderID;
                
            }
            catch (Exception ex) {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
           
        }

        public Boolean ValidateUplodedData(string uploadrefno, string PickupCountry)
        {
            bool BolReturnStatus = false;
            try
            {
                _ObjContext.stp_UpdateIDForBulkUpload(_IntOrderId);
                ObjectResult<stp_validateBulkUploadData_Result> errorFileRefNo = _ObjContext.stp_validateBulkUploadData(_IntOrderId,PickupCountry, uploadrefno, false);
                if (errorFileRefNo.Count() > 0)
                {

                    BolReturnStatus = false;
                }
                else
                {
                    Itbl_ConsignmentInterfaceHeaderRepository ObjHeader = new tbl_ConsignmentInterfaceHeaderRepository();
                    tbl_ConsignmentInterfaceHeader ObjConsignmentInterfaceHeader = new tbl_ConsignmentInterfaceHeader();
                    ObjConsignmentInterfaceHeader = (from Obj in ObjHeader.All
                                                     where Obj.UploadRefNumber == uploadrefno
                                                     select Obj).FirstOrDefault();
                    
                   // var batchstatus = ObjHeader.Find(_IntOrderId);
                    ObjConsignmentInterfaceHeader.ProcessStatus = "Submitted";
                    ObjConsignmentInterfaceHeader.BatchStatus = "Submit";
                    ObjHeader.Update(ObjConsignmentInterfaceHeader);
                    ObjHeader.Save();
                    ObjHeader.Dispose();

                    BolReturnStatus = true;
                }

            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return BolReturnStatus;
        }


        public string CallBatchProcess(string UploadRefno)
        {
            string StrReturnValue = null;
            string StrReturnStatus = string.Empty;
            try
            {
                ClsBatchProcess ObjBatchProcess = new ClsBatchProcess();
                StrReturnStatus = ObjBatchProcess.CallBatchProcess(UploadRefno);
                if (StrReturnStatus.ToUpper().Equals("SUCCESS"))
                {
                    tbl_ConsignmentInterfaceHeaderRepository ObjHeaderRepository = new tbl_ConsignmentInterfaceHeaderRepository();
                    StrReturnValue = (from headerObj in ObjHeaderRepository.All
                                      where headerObj.UploadRefNumber == UploadRefno
                                      select headerObj.BatchID).FirstOrDefault();

                }
                return StrReturnValue;
            }
                 catch (Exception ex) {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            
            }
            return StrReturnValue;
        }
        public string CallRoutingMatrix(string StrBatchId)
        {
            string StrReturnValue = string.Empty;
            RoutingMatrixController ObjRoutingMatrix = new RoutingMatrixController();
            bool BolRM = ObjRoutingMatrix.callRoutingMatrix(StrBatchId);

            if (BolRM)
            {
                TrackingNoGeneration ObjTrackingNoGeneration = new TrackingNoGeneration(StrBatchId);
                string StrTrackingStatus = ObjTrackingNoGeneration.GenerateTrackingNumber();

                if (StrTrackingStatus.ToLower() == "success")
                {
                    
                    ClsReportGeneration objGenerate = new ClsReportGeneration();
                    objGenerate._IsReportGenerationSendMail = false;
                    if (objGenerate.GenerateReport(StrBatchId) != "success")
                    {
                        StrReturnValue = ConfigurationManager.AppSettings["REPORTGENERATIONFAIL_EN"];
                        return StrReturnValue;
                    }
                }
                else
                {
                    StrReturnValue = ConfigurationManager.AppSettings["TRACKINGNUMBERFAILURE_EN"];
                }
            }
            else
            {
                StrReturnValue = ConfigurationManager.AppSettings["RMFAILURE_EN"];
            }
           

            return StrReturnValue;

        }


        public List<tbl_ConsignmentDetail> GetConsignmentDetails(string batchId)
        {
            List<tbl_ConsignmentDetail> ObjConsignmentDetail = new List<tbl_ConsignmentDetail>();
            try
            {

                tbl_ConsignmentHeader ObjConsignmentHeader = new tbl_ConsignmentHeader();
                Itbl_ConsignmentHeaderRepository ObjConsignmentHeaderRepository = new tbl_ConsignmentHeaderRepository();

                ObjConsignmentHeader = (from ObjHeaderResult in ObjConsignmentHeaderRepository.All
                                        where ObjHeaderResult.BatchID == batchId
                                        select ObjHeaderResult).FirstOrDefault();
                _IntOrderId = ObjConsignmentHeader.OrderID;
                Itbl_ConsignmentDetailRepository ObjConsignmentDetailRepository = new tbl_ConsignmentDetailRepository();
                ObjConsignmentDetail = (from ObjResult in ObjConsignmentDetailRepository.All
                                        where ObjResult.OrderID == ObjConsignmentHeader.OrderID
                                        select ObjResult).ToList();
                return ObjConsignmentDetail;
            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);

            }
            return ObjConsignmentDetail;
        }

        public List<tbl_ConsignmentInterfaceDetail> GetOrderValidationDetails(string UploadRefno)
        {
            List<tbl_ConsignmentInterfaceDetail> ObjConsignmentInterfaceDetail = new List<tbl_ConsignmentInterfaceDetail>();
            try
            {
                
                tbl_ConsignmentInterfaceHeader ObjConsignmentInterfaceHeader = new tbl_ConsignmentInterfaceHeader();
                Itbl_ConsignmentInterfaceHeaderRepository ObjConsignmentInterfaceHeaderRepository = new tbl_ConsignmentInterfaceHeaderRepository();

                ObjConsignmentInterfaceHeader = (from ObjHeaderResult in ObjConsignmentInterfaceHeaderRepository.All
                                                 where ObjHeaderResult.UploadRefNumber == UploadRefno
                                                select ObjHeaderResult).FirstOrDefault();

                Itbl_ConsignmentInterfaceDetailsRepository ObjConsignmentInterfaceDetailRepository = new tbl_ConsignmentInterfaceDetailsRepository();
                ObjConsignmentInterfaceDetail = (from ObjResult in ObjConsignmentInterfaceDetailRepository.All
                                                where ObjResult.OrderID == ObjConsignmentInterfaceHeader.OrderID
                                                select ObjResult).ToList();
                return ObjConsignmentInterfaceDetail;
            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);

            }
            return ObjConsignmentInterfaceDetail;
        }

        public void UpdateCustomerAccTypeHDR(string UploadRefno, string CUSTOMERACCOUNTTYPE, string creditLimit)
        {
          
            try
            {
                tbl_ConsignmentInterfaceHeader ObjConsignmentInterfaceHeader;
                Itbl_ConsignmentInterfaceHeaderRepository ObjHeaderRepository = new tbl_ConsignmentInterfaceHeaderRepository();
                ObjConsignmentInterfaceHeader = (from ObjHeaderResult in ObjHeaderRepository.All
                                                 where ObjHeaderResult.UploadRefNumber == UploadRefno
                                                 select ObjHeaderResult).FirstOrDefault();

                
                ObjConsignmentInterfaceHeader.CustomerAccountType = CUSTOMERACCOUNTTYPE;
                ObjConsignmentInterfaceHeader.CreditLimitFlag = creditLimit == "True" ? true : false;
                ObjConsignmentInterfaceHeader.UploadRefNumber = UploadRefno;
                //consignmentinterfacehdr
                ObjHeaderRepository.Update(ObjConsignmentInterfaceHeader);
                ObjHeaderRepository.Save();
               
                
            }
            catch (Exception ex) {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy); 
            
            }
            
        }

        public int InsertUploadAction(string uploadRefNo, string xmlContent, string strReturnValue, bool success, string status)
        {
            int IntReturnId = 0;
            try
            {
                tbl_LWMSUploadAction ObjLWMSUpload = new tbl_LWMSUploadAction();
                Itbl_LWMSUploadActionRepository ObjUpload = new tbl_LWMSUploadActionRepository();

                ObjLWMSUpload.OrderXML = xmlContent;
                ObjLWMSUpload.UploadRefNumber = uploadRefNo;
                ObjLWMSUpload.IsSuccess = success;
                ObjLWMSUpload.StatusMessage = status;
                ObjLWMSUpload.CreatedDate = DateTime.Now;
                ObjLWMSUpload.AcknowledgeXML = strReturnValue;
                ObjUpload.InsertOrUpdate(ObjLWMSUpload);
                ObjUpload.Save();
                IntReturnId = ObjLWMSUpload.LWMSUploadId;
                ObjUpload.Dispose();
            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy); 
            }
            return IntReturnId;
        }
        public void UpdateOrderAction(string statusMsg, string strAcknowledge, bool status, int IntInsertedId,string userid,string batchId)
        { 
            try
            {
                Itbl_ConsignmentHeaderRepository ConsignmentHeaderRepository = new tbl_ConsignmentHeaderRepository();
                tbl_LWMSUploadAction ObjLWMSUpload = new tbl_LWMSUploadAction();
                Itbl_LWMSUploadActionRepository ObjUpload = new tbl_LWMSUploadActionRepository();

                tbl_CustomerRepository ObjCustomerRepository = new tbl_CustomerRepository ();
                tbl_Customer ObjCustomer = (from ObjCust in ObjCustomerRepository.All
                                           where ObjCust.UserID == userid
                                           select ObjCust).FirstOrDefault();


                ObjLWMSUpload = ObjUpload.Find(IntInsertedId);

                ObjLWMSUpload.AcknowledgeXML = strAcknowledge;
                if (IntInsertedId > 0)
                {
                    ObjLWMSUpload.LWMSUploadId = IntInsertedId;
                }
                ObjLWMSUpload.IsSuccess = status;

                if (!string.IsNullOrEmpty(batchId))
                {
                    int _IntOrderId = (from ConHeader in ConsignmentHeaderRepository.All
                                       where ConHeader.BatchID == batchId
                                       select ConHeader.OrderID).FirstOrDefault();
                    if (_IntOrderId != 0)
                    {
                        ObjLWMSUpload.OrderId = _IntOrderId;
                    }
                }
                ObjLWMSUpload.StatusMessage = statusMsg;
                if (ObjCustomer != null)
                {
                    ObjLWMSUpload.CreatedBy = ObjCustomer.CustomerID;
                }
                ObjUpload.InsertOrUpdate(ObjLWMSUpload);
                ObjUpload.Save();
                ObjUpload.Dispose();
                
            }
            catch (Exception ex) {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
              }
           
        }

        public IQueryable<string> GetServiceTypes(string batchId)
        {

            IQueryable<string> StrRMserviceTypes = null;
            try
            {
                tbl_ConsignmentDetail ObjConsignmentDetail = new tbl_ConsignmentDetail();
                Itbl_ConsignmentDetailRepository DetailRepository = new tbl_ConsignmentDetailRepository();
                tbl_ConsignmentHeaderRepository HeaderRepository = new tbl_ConsignmentHeaderRepository();

                int orderId = (from header in HeaderRepository.All
                              where header.BatchID == batchId
                              select header.OrderID).ToList().FirstOrDefault();

                StrRMserviceTypes = (from detail in DetailRepository.All
                                     where detail.OrderID == orderId
                                      select detail.RMServiceType).Distinct();


                return StrRMserviceTypes;
            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return StrRMserviceTypes;
        }

        public bool IsLoadReportfromPath(string batchId)
        {
            tbl_ConsignmentDetail ObjConsignmentDetail = new tbl_ConsignmentDetail();
            Itbl_ConsignmentDetailRepository DetailRepository = new tbl_ConsignmentDetailRepository();
            tbl_ConsignmentHeaderRepository HeaderRepository = new tbl_ConsignmentHeaderRepository();
            try
            {
            int orderId = (from header in HeaderRepository.All
                           where header.BatchID == batchId
                           select header.OrderID).ToList().FirstOrDefault();
           

                bool? Loadreport = (from detail in DetailRepository.All
                                 where detail.OrderID == orderId
                                 select detail.LoadReportFromPath).ToList().FirstOrDefault();
                return Loadreport.HasValue ? (bool)Loadreport : false;
            }
            catch (Exception excep)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(excep, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return false;
        }

        public bool IsReportGenerated(string batchId)
        {

            tbl_ConsignmentDetail ObjConsignmentDetail = new tbl_ConsignmentDetail();
            Itbl_ConsignmentDetailRepository DetailRepository = new tbl_ConsignmentDetailRepository();
            tbl_ConsignmentHeaderRepository HeaderRepository = new tbl_ConsignmentHeaderRepository();
            try
            {
                int orderId = (from header in HeaderRepository.All
                               where header.BatchID == batchId
                               select header.OrderID).ToList().FirstOrDefault();


                bool? ReportGenerated = (from detail in DetailRepository.All
                                    where detail.OrderID == orderId
                                    select detail.IsReportGenerated).ToList().FirstOrDefault();
                return ReportGenerated.HasValue ? (bool)ReportGenerated : false;
            }
            catch (Exception excep)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(excep, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return false;
        }

        public string GetDeclaredCurrencyDescription(string decCurrency)
        {

            try
            {
                Itbl_CurrencyRepository CurrDesc = new tbl_CurrencyRepository();
                tbl_Currency ObjCurrency = new tbl_Currency();

                string CurrencyDesc = (from curr in CurrDesc.All
                                   where curr.CurrencyID == decCurrency
                                   select curr.CurrencyDescription).ToList().FirstOrDefault();

                string strReturn = CurrencyDesc;
                return decCurrency + "  " + strReturn;
            }
            catch (Exception ex)
            {
                ClsApplicationExceptionHandling ObjStatusError = new ClsApplicationExceptionHandling();
                ObjStatusError.HandleException(ex, ApplicationExceptionPolicies.EnmExceptionPolicies.BussinessPolicy);
            }
            return decCurrency;
        }

        }
    }
