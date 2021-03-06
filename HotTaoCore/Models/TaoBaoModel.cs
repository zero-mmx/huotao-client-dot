﻿/*
 * 版权所有:杭州火图科技有限公司
 * 地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
 * (c) Copyright Hangzhou Hot Technology Co., Ltd.
 * Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
 * 2013-2017. All rights reserved.
 * author guomw
**/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotTaoCore.Models
{
    public class TaobaoCommonCampaignItemsModel
    {

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public List<TaobaoCommonItem> data { get; set; }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        public TaobaoComInfo info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>true if ok; otherwise, false.</value>
        public bool ok { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>true if [invalid key]; otherwise, false.</value>
        public string invalidKey { get; set; }

    }

    public class TaobaoCommonItem
    {
        /// <summary>
        /// 佣金率
        /// </summary>
        public decimal commissionRate { get; set; }
        /// <summary>
        /// 活动计划ID
        /// </summary>
        public string CampaignID { get; set; }
        /// <summary>
        /// 定向计划名称
        /// </summary>
        public string CampaignName { get; set; }
        /// <summary>
        /// 定向计划类型
        /// </summary>
        public string CampaignType { get; set; }
        /// <summary>
        /// 平均佣金率
        /// </summary>
        public string AvgCommission { get; set; }
        /// <summary>
        /// 是否已申请
        /// </summary>
        public bool Exist { get; set; }
        /// <summary>
        /// 是否人工审核 1 是 0自动
        /// </summary>
        public int manualAudit { get; set; }
        /// <summary>
        /// 店铺ID
        /// </summary>
        public string ShopKeeperID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Properties { get; set; }

    }


    public class TaobaoComInfo
    {
        public string message { get; set; }

        public bool ok { get; set; }
    }

}
