﻿using System.Web;
using System.Web.Mvc;

namespace 微信支付之扫码支付
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
