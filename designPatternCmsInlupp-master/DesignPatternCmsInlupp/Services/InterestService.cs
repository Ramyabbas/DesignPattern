using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Services
{
    public class InterestService
    {
        public static decimal GetRiksbankensBaseRate()
        {
            //Fake slow call
            int sleppWhenFail = 3000;
            int reteies = 5;
            while (reteies > 0)
            {

                try
                {
                    using (var c = new SweaWebService.SweaWebServicePortTypeClient())
                    {
                        //var groups = c.getInterestAndExchangeGroupNames(SweaWebService.LanguageType.sv).ToList();

                        //var n = c.getInterestAndExchangeNames(5, SweaWebService.LanguageType.sv).ToList();

                        var r = c.getLatestInterestAndExchangeRates(SweaWebService.LanguageType.sv, new[] { "SEDP3MSTIBORDELAYC" });

                        return Convert.ToDecimal(r.groups[0].series[0].resultrows[0].value);
                    }
                }

                catch
                {
                    System.Threading.Thread.Sleep(sleppWhenFail);
                    reteies--;
                }
            }
            return 0m;
            
        }
    }
}