﻿
using HotCoreUtils.Helper;
using HotTaoCore.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotTaoCore.Logic
{
    public class LogicGoods
    {

        private static LogicGoods _instance = new LogicGoods();

        public static LogicGoods Instance
        {
            get
            {
                return _instance;
            }
        }
        /// <summary>
        /// 获取已选商品列表
        /// </summary>
        /// <param name="loginToken">The login token.</param>
        /// <returns>List&lt;GoodsModel&gt;.</returns>
        public List<GoodsModel> getGoodsList(string loginToken)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["token"] = loginToken;
            return BaseRequestService.Post<List<GoodsModel>>(ApiConst.getGoodsList, data);
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteGoods(string loginToken, int id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["token"] = loginToken;
            data["goodsid"] = id.ToString();
            return BaseRequestService.Post(ApiConst.delGoods, data);
        }

        /// <summary>
        /// 根据链接获取商品信息
        /// </summary>
        /// <param name="loginToken"></param>
        /// <param name="urls">json数组[{url:"",url2:""}]</param>
        /// <returns></returns>
        public List<GoodsSelectedModel> getGoodsByLink(string loginToken, string urls)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["token"] = loginToken;
            data["urls"] = urls;
            return BaseRequestService.Post<List<GoodsSelectedModel>>(ApiConst.findGoodsByLink, data);

        }








        /// <summary>
        /// 商品入库
        /// </summary>
        /// <param name="item"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int SaveGoods(GoodsSelectedModel item, int userid, out bool isUpdate)
        {
            isUpdate = false;
            if (item.couponPrice <= 0 || item.goodsPrice - item.couponPrice <= 0) return 0;
            GoodsModel goods = new GoodsModel()
            {
                userid = userid,
                goodsId = item.goodsId.Replace("=", ""),
                goodsName = item.goodsName,
                goodsIntro = item.goodsIntro,
                goodsMainImgUrl = item.goodsImageUrl,
                goodsDetailUrl = item.goodsDetailUrl,
                goodsSupplier = string.IsNullOrEmpty(item.goodsPlatform) ? "淘宝" : item.goodsPlatform,
                goodsComsRate = item.goodsComsRate,
                goodsPrice = item.goodsPrice,
                goodsSalesAmount = item.goodsSalesAmount,
                endTime = Convert.ToDateTime(item.endTime),
                couponUrl = item.couponUrl,
                couponId = item.couponId.Replace("activityId=", "").Replace("activity_id=", "").Replace("activity_Id=", ""),
                couponPrice = item.couponPrice
            };
            if (string.IsNullOrEmpty(goods.couponId))
            {
                System.Text.RegularExpressions.Regex regs = new System.Text.RegularExpressions.Regex("&activityId=[^&]*", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string couponUrl = item.couponUrl.Replace("activity_id=", "activityId=").Replace("activity_Id=", "activityId=");
                System.Text.RegularExpressions.Match m = regs.Match(couponUrl);
                if (m.Success)
                {
                    goods.couponId = m.Value.Replace("&activityId=", "");
                }
            }

            string fileName = Environment.CurrentDirectory + "\\temp\\img\\" + userid;
            if (!Directory.Exists(fileName))
                Directory.CreateDirectory(fileName);
            fileName += "\\" + EncryptHelper.MD5(goods.goodsId) + ".jpg";
            //判断文件是否存在
            if (!File.Exists(fileName))
            {
                downloadGoodsImage(fileName, goods.goodsMainImgUrl, goods.goodsId, userid);
            }
            goods.goodslocatImgPath = fileName;
            return LogicHotTao.Instance(userid).AddUserGoods(goods, out isUpdate);
        }


        /// <summary>
        /// 下载商品图片
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="goodsImageUrl">The goods image URL.</param>
        private void downloadGoodsImage(string fileName, string goodsImageUrl, string goodsid, int userid)
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    string _goodsid = goodsid;
                    byte[] data = BaseRequestService.GetNetWorkImageData(goodsImageUrl);
                    if (data == null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        data = BaseRequestService.GetNetWorkImageData(goodsImageUrl);
                    }
                    if (data != null)
                    {
                        MemoryStream ms = new MemoryStream(data);
                        Bitmap img = new Bitmap(ms);
                        img.Save(fileName, ImageFormat.Jpeg);
                        ms.Dispose();
                        ms = null;
                        img.Dispose();
                        img = null;
                    }
                    else
                    {
                        LogicHotTao.Instance(userid).DeleteGoodsByGoodsid(goodsid);
                    }

                }
                catch (Exception ex)
                {

                }
            })
            { IsBackground = true }.Start();
        }


    }
}
