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

namespace SingPost.EzyParcels.Services
{
    public class Utility
    {
        public const string ORDER = "Order";
        public const string ORDER_CONSIGNMENT_HEADER = "ConsignmentHeader";
        public const string ORDER_ITEM = "Item";
        public const string ORDER_ITEM_CONSIGNMENT_DETAIL = "ConsignmentDetail";
        public const string ORDER_ITEM_REMARKS = "Remarks";
        public const string LWMS_USER = "LWMSUser";

        // Header
        public const string SERVICE_TYPE = "ServiceType";
        public const string SYSTEM_ID = "SystemID";
        public const string PICKUP_DATE = "PickupDate";
        public const string PICKUP_TIME = "PickupTime";
        public const string USER_ID = "UserID";

        // Consignment Detail        
        public const string FIRST_NAME = "FirstName";
        public const string MIDDLE_NAME = "MiddleName";
        public const string LAST_NAME = "LastName";
        public const string ADDRESS1 = "Address1";
        public const string ADDRESS2 = "Address2";
        public const string CITY = "City";
        public const string STATE = "State";
        public const string POST_CODE = "Postcode";
        public const string COUNTRY = "Country";
        public const string CONTACT = "Contact";
        public const string CUSTOMER_REFERENCE = "CustomerReference";
        public const string WEIGHT = "Weight";
        public const string DECLARED_CURRENCY = "DeclaredCurrency";
        public const string DECLARED_VALUE = "DeclaredValue";
        public const string ITEM_DESCRIPTION = "ItemDescription";
        public const string HSCODE = "HSCode";
        public const string INSURED = "Insured";
        public const string INSURED_CURRENCY = "InsuredCurrency";
        public const string INSURED_VALUE = "InsuredValue";
        public const string ACTION = "Action";
        public const string TYPE = "Type";

        //Remarks
        public const string RESPONSE = "Response";

        public const string LWMS_SOURCE = "LWMS";
        public const string LWMS_ACTION_PUSHORDERSTATUS = "Request";
        public const string LWMS_ACTION_VALIDATIONORDER_RESPONSE = "Validation Pending";
        public const string LWMS_ACTION_PROCESSORDER_RESPONSE = "Process Order Pending";
        public const string BATCH_ID = "BatchID";
        public const string TRACKING_NUMBER = "TrackingNumber";
        public const string TRANSACTION_ID = "TransactionId";
        public const string UPLOAD_REF_NO = "UploadReference";
        public const string NO_SYSTEM_ID = "System Id cannot be blank.";
        public const string NO_SERVICETYPE = "Service Type cannot be blank.";
        public const string INVALID_SERVICETYPE = "Invalid Service Type";
        public const string NO_USERID = "User Id cannot be blank.";
        public const string INVALID_USERID = "Invalid User Id";
        public const string NO_CUSTOMER_REF_NO = "Customer reference number cannot be blank.";
        public const string NO_NUMERIC_WEIGHT = "Weight per article should contain numeric value.";
        public const string INVALID_PICKUPDATE = "Invalid Pickup date.";
        public const string NO_NUMERIC_DECLAREDVALUE = "Declared value should contain numeric value.";
        public const string NO_NUMERIC_INSUREDVALUE = "Insured value should contain numeric value.";
        public const string PICKUP = "Before";
        public const string LWMS_ACTION_VALTIDATEFAILEDSTATUS = "Validation Failed";
        public const string LWMS_INVALID_XML = "Invalid XML Format";
        public const string LWMS_BATCHGEN_FAIL = "Batch ID could not be generated";
        public const string LWMS_SUCCESS = "Success";
    }
}