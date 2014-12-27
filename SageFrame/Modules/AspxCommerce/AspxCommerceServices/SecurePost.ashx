<%@ WebHandler Language="C#" Class="SecurePost" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using AspxCommerce.Core;
using System.Collections.Generic;
using ServiceInvoker;

public class SecurePost : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
       
            JavaScriptSerializer jss = new JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            //string json = new System.IO.StreamReader(context.Request.InputStream).ReadToEnd();

            string calltype = context.Request.QueryString["call"];
            object status = null;
            switch (calltype)
            {//GetSESSION
                case "GSA":
                    string addkey = context.Request.Form["k"].ToString();


                    //context.Session.Add(key, value);
                    status = CheckOutSessions.Get<double>(addkey, 0);
                    break;


                //set session
                case "SS":
                    string key = context.Request.Form["k"].ToString();
                    string value = context.Request.Form["v"];

                    //context.Session.Add(key, value);
                    CheckOutSessions.Add(key, value);
                    break;
                //remove session coupon
                case "RSC":
                    CheckOutSessions.Delete("CouponSession");
                    //if (context.Session["CouponSession"] != null)
                    //{
                    //    context.Session.Remove("CouponSession");
                    //}

                    break;
                //get session coupon

                case "GSC":
                    List<CouponSession> cs = new List<CouponSession>();

                    //if (context.Session["CouponSession"] != null)
                    //{
                    //    cs = (List<CouponSession>)context.Session["CouponSession"];
                    //}
                    cs = CheckOutSessions.Get<List<CouponSession>>("CouponSession");

                    status = jss.Serialize(cs);
                    break;
                //set session coupon
                case "SSC":
                    string cKey = context.Request.Form["k"].ToString();
                    string cValue = context.Request.Form["v"];

                    List<CouponSession> couponInfo = jss.Deserialize<List<CouponSession>>(cValue);
                    CheckOutSessions.Add("CouponSession", couponInfo);
                    //context.Session.Add("CouponSession", couponInfo);

                    break;
                //SEssion Get
                case "SG":
                    string skey = context.Request.Form["k"].ToString();
                    if (context.Session[skey] != null)
                    {

                    }
                    break;
                #region CHECKOUT SESSIONS
                //get kgiftcard used
                case "GGC":



                    List<GiftCardUsage> list = CheckOutSessions.Get<List<GiftCardUsage>>("UsedGiftCard");
                    if (list != null)
                    {
                        status = jss.Serialize(list);
                    }
                    else
                    {
                        status = false;
                    }
                    break;
                //remove coupon session
                case "RGC":
                    CheckOutSessions.Delete("UsedGiftCard");
                    //if (context.Session["UsedGiftCard"] != null)
                    //{
                    //    context.Session.Remove("UsedGiftCard");
                    //}
                    break;
                case "SGC":
                    string gcList = context.Request.Form["v"];

                    List<GiftCardUsage> gcUsage = jss.Deserialize<List<GiftCardUsage>>(gcList);
                    CheckOutSessions.Add("UsedGiftCard", gcUsage);
                    //context.Session.Add("UsedGiftCard", gcUsage);

                    break;
                //get checkoutvars final
                case "GCVs":
                    double tax = 0, gateway = 1, gt = 0, discount = 0, spcost = 0;
                    ////if (context.Session["TaxAll"] != null)
                    ////{
                    ////    tax = double.Parse(context.Session["TaxAll"].ToString());
                    ////}
                    tax = CheckOutSessions.Get<Double>("TaxAll", 0);
                    //if (context.Session["Gateway"] != null)
                    //{
                    //    gateway = double.Parse(context.Session["Gateway"].ToString());

                    //}
                    gateway = CheckOutSessions.Get<int>("GateWay", 0);
                    gt = CheckOutSessions.Get<Double>("GrandTotalAll", 0);
                    discount = CheckOutSessions.Get<Double>("DiscountAmount", 0);
                    spcost = CheckOutSessions.Get<Double>("ShippingCostAll", 0);
                    //if (context.Session["TaxAll"] != null)
                    //{
                    //    gt = double.Parse(context.Session["GrandTotalAll"].ToString());

                    //}
                    //if (context.Session["DiscountAmount"] != null)
                    //{
                    //    discount = double.Parse(context.Session["DiscountAmount"].ToString());

                    //}
                    //if (context.Session["ShippingCostAll"] != null)
                    //{
                    //    spcost = double.Parse(context.Session["ShippingCostAll"].ToString());

                    //}
                    var obj = new
                    {
                        TaxAll = tax,
                        GrandTotal = gt,
                        ShippingCost = spcost,
                        DiscountAll = discount,
                        Gateway = gateway
                    };
                    status = jss.Serialize(obj);

                    break;
                //set session taxall
                case "STx":
                    string txAll = context.Request.Form["v"].ToString(); ;
                    // context.Session["TaxAll"] = txAll;
                    CheckOutSessions.Add("TaxAll", txAll);
                    break;
                //set grandtotal
                case "SGT":
                    string _gt = context.Request.Form["v"].ToString(); ;
                    // context.Session["GrandTotalAll"] = _gt;
                    CheckOutSessions.Add("GrandTotalAll", _gt);

                    break;
                //set gateway
                case "SGtw":
                    string gtw = context.Request.Form["v"].ToString();
                    CheckOutSessions.Add("GateWay", gtw);

                    break;
                //set discount
                case "SDsct":
                    string dsc = context.Request.Form["v"].ToString(); ;
                    CheckOutSessions.Add("DiscountAmount", dsc);
                    break;
                //set shippingCost
                case "SSpC":
                    string _sp = context.Request.Form["v"].ToString(); ;
                    CheckOutSessions.Add("ShippingCostAll", _sp);
                    break;
                case "SSpN":
                    string _spM = context.Request.Form["v"].ToString(); ;
                    CheckOutSessions.Add("ShippingMethodName", _spM);
                    break;

                case "SSA":
                    //set session addtional type
                    string saKey = context.Request.Form["k"].ToString();
                    string saValue = context.Request.Form["v"];
                    CheckOutSessions.Add(saKey, saValue, true);
                    break;
                #endregion
            }




            var wrapper = new { d = status };

            context.Response.Write(jss.Serialize(wrapper));

            //context.Response.Write("Hello World");
      
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}