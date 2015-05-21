﻿using ConsoleApplication1.Utility;
using ConsoleApplication1.UtilService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class AirportDetails
    {
        internal IDictionary<String, String> AllAirportsList()
        {
            ReferenceDataRetrieveReq retrieveReq = new ReferenceDataRetrieveReq();
            ReferenceDataRetrieveRsp retrieveRsp;
            IDictionary<String, String> airportsList = new Dictionary<String, String>();

            retrieveReq.BillingPointOfSaleInfo = new BillingPointOfSaleInfo()
            {
                OriginApplication = "UAPI"
            };

            retrieveReq.TargetBranch = CommonUtility.GetConfigValue(ProjectConstants.G_TARGET_BRANCH);
            retrieveReq.TypeCode = "CityAirport";

            retrieveReq.ReferenceDataSearchModifiers = new ReferenceDataSearchModifiers()
            {
                MaxResults = "20000",
                StartFromResult = "0",
                ProviderCode = "1G"
            };

            ReferenceDataRetrievePortTypeClient client = new ReferenceDataRetrievePortTypeClient("ReferenceDataRetrievePort", WsdlService.UTIL_ENDPOINT);

            client.ClientCredentials.UserName.UserName = Helper.RetrunUsername();
            client.ClientCredentials.UserName.Password = Helper.ReturnPassword();
            try
            {
                var httpHeaders = Helper.ReturnHttpHeader();
                client.Endpoint.EndpointBehaviors.Add(new HttpHeadersEndpointBehavior(httpHeaders));

                retrieveRsp = client.service(retrieveReq);

                if (retrieveRsp != null)
                {
                    IEnumerator dataItems = retrieveRsp.ReferenceDataItem.GetEnumerator();
                    int count = 0;
                    while (dataItems.MoveNext() && count < 50)//We have added 50 Airports in the List, You can add all if you want, Just remove the count
                    {
                        ReferenceDataItem item = (ReferenceDataItem)dataItems.Current;
                        airportsList.Add(item.Code, item.Name);
                        count++;
                    }
                    
                }

                return airportsList;

            }
            catch (Exception se)
            {
                Console.WriteLine("Error : " + se.Message);
                client.Abort();
                return null;
            }



        }
    }
}