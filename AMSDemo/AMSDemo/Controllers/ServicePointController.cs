﻿using AMSDemo.DatabaseOps;
using AMSDemo.Models;
using AMSDemo.Utility;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AMSDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServicePointController : ControllerBase
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        //MicroService 4
        public IActionResult ReadMQ2OPLDNCreateSPNPushTOMQ3(string strOPLD)
        {
            try
            {
                //Read from MQ
                //OPLD opldObject = CommonUtility<OPLD>.PullFromActiveMQ(2);

                OPLD opldObject = Newtonsoft.Json.JsonConvert.DeserializeObject<OPLD>(strOPLD);

                log.Info(DateTime.Now.ToString() + " AMS-POC: Service point genration process started.");

                ServicePoint servicePointObject = new ServicePoint();

                //CreateServicepoint          
                //Check if opld tracking number matches with dials matching number
                SakilaContext context = HttpContext.RequestServices.GetService(typeof(SakilaContext)) as SakilaContext;
                DIALS dialsObject = context.GetMatchingDialsID(opldObject.TrackingNumber);

                if (dialsObject != null)
                {
                    servicePointObject = ServicePointUtility.CreateServicePoint(opldObject, dialsObject.ConsigneeName, dialsObject.ClarifiedSignature, true);
                }
                else
                {
                    servicePointObject = ServicePointUtility.CreateServicePoint(opldObject, "", "", false);
                }

                //Push OPLD in to Active MQ2
                CommonUtility<ServicePoint>.PushToActiveMQ(servicePointObject, 3);

                log.Info(DateTime.Now.ToString() + " AMS-POC: Service point Created and Pushed to MQ3.");
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now.ToString() + " AMS-POC: " + Convert.ToString(ex.Message));
                return new JsonResult(new { Result = System.Net.HttpStatusCode.InternalServerError });
            }

            return Ok();
        }

        [HttpPost]
        //MicroService 5
        public IActionResult ReadMQ3ServicePointNWriteToDB(string strServicepoint)
        {
            try
            {
                //Read from MQ
                //ServicePoint servicePointObject = CommonUtility<ServicePoint>.PullFromActiveMQ(3);

                ServicePoint servicePointObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ServicePoint>(strServicepoint);

                //Write Servicepoint to DB
                SakilaContext context = HttpContext.RequestServices.GetService(typeof(SakilaContext)) as SakilaContext;
                context.AddNewServicePoint(servicePointObject);

                log.Info(DateTime.Now.ToString() + " AMS-POC: Service point inserted in to database.");
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now.ToString() + " AMS-POC: " + Convert.ToString(ex.Message));
                return new JsonResult(new { Result = System.Net.HttpStatusCode.InternalServerError });
            }

            return Ok();
        }
    }
}