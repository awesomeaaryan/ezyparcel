
/*********************************************************************
 * Screen / Process Name : User Registration,User Profile,SubAccount Edit,Creation.
 * Called From :
 * Parameter: 
 * 
* Date		    Version	    Author			Comments
*---------------------------------------------------------------------------------
* 17-Nov-2013	 1.0.0		------   	    initial version
* 2-Dec-2013     1.0.1      Sourabh         Added Logic for SimpleMembershipInitializer(Web Security)
*---------------------------------------------------------------------------------
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SingPost.EzyParcels.Models.CommonModels;
using SingPost.EzyParcels.ApplicationCode;
using WebMatrix.WebData;
using WebMatrix.Data;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Optimization;

namespace SingPost.EzyParcels
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication, IDisposable
    {
        protected void Application_Start()
        {
            try
            {
                AreaRegistration.RegisterAllAreas();
                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);               
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// redirect to https
        /// </summary>
        protected void Application_BeginRequest()
        {
            //if (!Context.Request.IsSecureConnection)
            //    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
            //Code Added for Click Jacking Attacks.
            HttpContext.Current.Response.AddHeader("x-frame-options", "SAMEORIGIN");
            /* Added by Minati on 04-June-2014 for remove http header information (Bug Id : 2962)*/
            MvcHandler.DisableMvcResponseHeader = true;
        }

        /// <summary>
        /// Method to Validate based on principal and identity classes and maintain session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
           
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            /* Added by Minati on 04-June-2014 for remove http header information (Bug Id : 2962)*/
            string iisMode = System.Configuration.ConfigurationManager.AppSettings["IISMODE"];
            if (!string.IsNullOrEmpty(iisMode) && iisMode.Equals("1"))
                Response.Headers.Remove("Server");
        }
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;
        /// <summary>
        /// Added For WebSecurity Implementation
        /// </summary>
        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                try
                {
                    WebSecurity.InitializeDatabaseConnection("EZYPARCELSConnectionString", "tbl_customer", "CustomerID", "UserID", autoCreateTables: false);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized", ex);
                }
            }
        }
    }
}