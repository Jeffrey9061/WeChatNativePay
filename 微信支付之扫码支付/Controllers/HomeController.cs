﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;

namespace WxPay.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }




        /// <summary>
        /// 模式一
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetQRCode1()
        {
            object objResult = "";
            string strProductID = Request.Form["productId"];
            string strQRCodeStr = GetPrePayUrl(strProductID);
            if (!string.IsNullOrWhiteSpace(strProductID))
            {
                objResult = new { result = true, str = strQRCodeStr };
            }
            else
            {
                objResult = new { result = false };
            }
            return Json(objResult);
        }

        /// <summary>
        /// 模式二
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetQRCode2()
        {
            object objResult = "";
            string strProductID = Request.Form["productId"];
            string strQRCodeStr = GetPayUrl(strProductID);
            if (!string.IsNullOrWhiteSpace(strProductID))
            {
                objResult = new { result = true, str = strQRCodeStr };
            }
            else
            {
                objResult = new { result = false };
            }
            return Json(objResult);
        }

        /**
        * 生成扫描支付模式一URL
        * @param productId 商品ID
        * @return 模式一URL
        */
        public string GetPrePayUrl(string productId)
        {
            WxPayData data = new WxPayData();
            data.SetValue("appid", WxPayConfig.APPID);//公众帐号id
            data.SetValue("mch_id", WxPayConfig.MCHID);//商户号
            data.SetValue("time_stamp", WxPayApi.GenerateTimeStamp());//时间戳
            data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            data.SetValue("product_id", productId);//商品ID
            data.SetValue("sign", data.MakeSign());//签名
            string str = ToUrlParams(data.GetValues());//转换为URL串
            string url = "weixin://wxpay/bizpayurl?" + str;

            return url;
        }

        /**
       * 参数数组转换为url格式
       * @param map 参数名与参数值的映射表
       * @return URL字符串
       */
        private string ToUrlParams(SortedDictionary<string, object> map)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in map)
            {
                buff += pair.Key + "=" + pair.Value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }


        /**
       * 生成直接支付url，支付url有效期为2小时,模式二
       * @param productId 商品ID
       * @return 模式二URL
       */
        public string GetPayUrl(string productId)
        {

            WxPayData data = new WxPayData();
            data.SetValue("body", "广东雅达电子股份有限公司");//商品描述
            data.SetValue("attach", "附加信息,用于后台或者存入数据库,做自己的判断");//附加数据
            data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//随机字符串
            data.SetValue("total_fee", 1);//总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", "商品的备忘,可以自定义");//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", productId);//商品ID

            WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接
            
            return url;
        }
    }
}