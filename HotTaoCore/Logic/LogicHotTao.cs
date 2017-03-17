﻿/*
 * 版权所有:杭州火图科技有限公司
 * 地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
 * (c) Copyright Hangzhou Hot Technology Co., Ltd.
 * Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
 * 2013-2017. All rights reserved.
 * author guomw
**/


using HotTaoCore.DAL;
using HotTaoCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HotTaoCore.Models.SQLiteEntitysModel;

namespace HotTaoCore.Logic
{
    public class LogicHotTao
    {
        private static LogicHotTao _instance = new LogicHotTao();
        private static HotTaoDAL dal = new HotTaoDAL();
        public static LogicHotTao Instance
        {
            get
            {
                return _instance;
            }
        }


        #region 微信群操作
        /// <summary>
        /// 添加用户微信群
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddUserWeChatGroup(weChatGroupModel model)
        {
            return dal.AddUserWeChatGroup(model);
        }

        /// <summary>
        /// 修改微信群
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool UpdateUserWeChatGroup(weChatGroupModel model)
        {
            return dal.UpdateUserWeChatGroup(model);
        }

        public bool UpdateUserWeChatTitle(int userid, int groupid, string title, string pid)
        {
            weChatGroupModel model = new weChatGroupModel()
            {
                id = groupid,
                pid = pid,
                title = title,
                userid = userid
            };
            if (groupid > 0)
                return UpdateUserWeChatGroup(model);
            else
                return AddUserWeChatGroup(model) > 0;
        }

        /// <summary>
        /// 删除微信群
        /// </summary>
        /// <param name="groupid">The groupid.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool DeleteUserWeChatGroup(List<int> groupid)
        {
            return dal.DeleteUserWeChatGroup(groupid);
        }

        /// <summary>
        /// 获取微信群信息
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>weChatGroupModel.</returns>
        public weChatGroupModel FindByUserWeChatGroup(string title, int userid)
        {
            return dal.FindByUserWeChatGroup(title, userid);
        }

        /// <summary>
        /// 根据用户id，获取微信群
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>List&lt;weChatGroupModel&gt;.</returns>
        public List<UserWechatListModel> GetUserWeChatGroupListByUserId(int userid)
        {
            List<UserWechatListModel> data = new List<UserWechatListModel>();
            var items = dal.GetUserWeChatGroupListByUserId(userid);
            if (items != null)
            {
                items.ForEach(item =>
                {

                    data.Add(new UserWechatListModel()
                    {
                        id = Convert.ToInt32(item.id),
                        pid = item.pid,
                        wechattitle = item.title,
                        userid = userid
                    });
                });
            }
            return data;
        }

        /// <summary>
        /// 根据群id，获取群数据
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="ids">The ids.</param>
        /// <returns>weChatGroupModel.</returns>
        public List<weChatGroupModel> FindByUserWeChatGroup(int userid, List<int> ids)
        {
            return dal.FindByUserWeChatGroup(userid, ids);
        }



        #endregion





        #region 商品操作

        /// <summary>
        /// 添加本地商品
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddUserGoods(GoodsModel model)
        {
            return dal.AddUserGoods(model);
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="gid">The gid.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool DeleteGoods(int gid)
        {
            return dal.DeleteGoods(gid);
        }
        /// <summary>
        /// 根据商品自增ID，获取商品信息
        /// </summary>
        /// <param name="gid">The gid.</param>
        /// <returns>GoodsModel.</returns>
        public GoodsModel FindByUserGoodsInfo(int gid)
        {
            return dal.FindByUserGoodsInfo(gid);
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>List&lt;GoodsModel&gt;.</returns>
        public List<GoodsModel> FindByUserGoodsList(int userid)
        {
            return dal.FindByUserGoodsList(userid);
        }


        /// <summary>
        /// 根据商品id获取商品信息
        /// </summary>
        /// <param name="goodsId">The goods identifier.</param>
        /// <returns>GoodsModel.</returns>
        public GoodsModel FindByUserGoodsInfo(string goodsId, int userid)
        {
            return dal.FindByUserGoodsInfo(goodsId, userid);
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="ids">The ids.</param>
        /// <returns>List&lt;GoodsModel&gt;.</returns>
        public List<GoodsModel> FindByUserGoodsList(int userid, List<int> ids)
        {
            return dal.FindByUserGoodsList(userid, ids);
        }


        #endregion



        #region 任务计划相关操作
        /// <summary>
        /// 获取任务信息
        /// </summary>
        /// <param name="gid">The gid.</param>
        /// <returns>TaskPlanModel.</returns>
        public TaskPlanModel FindByUserTaskPlanInfo(int userid, int taskid)
        {
            return dal.FindByUserTaskPlanInfo(userid, taskid);
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>List&lt;GoodsModel&gt;.</returns>
        public List<TaskPlanModel> FindByUserTaskPlanList(int userid)
        {
            return dal.FindByUserTaskPlanList(userid);
        }
        /// <summary>
        /// 删除微信群
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool DeleteUserTaskPlan(int taskid)
        {
            return dal.DeleteUserTaskPlan(taskid);
        }
        /// <summary>
        /// 修改用户计划任务转链状态,转链成功后调用
        /// </summary>
        /// <param name="taskid">The taskid.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool UpdateUserTaskPlanIsTpwd(int taskid)
        {
            return dal.UpdateUserTaskPlanIsTpwd(taskid);
        }

        /// <summary>
        /// 修改用户计划任务执行状态 0 待执行  1进行中， 2已完成 3已过期
        /// </summary>
        /// <param name="taskid">The taskid.</param>
        /// <param name="status">The status.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool UpdateUserTaskPlanExecStatus(int taskid, int status)
        {
            return dal.UpdateUserTaskPlanExecStatus(taskid, status);
        }
        /// <summary>
        /// 添加用户计划任务
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public TaskPlanModel AddUserTaskPlan(TaskPlanModel model)
        {
            if (model.id == 0)
            {
                int taskid = dal.AddUserTaskPlan(model);
                if (taskid > 0)
                {
                    return FindByUserTaskPlanInfo(model.userid, taskid);
                }
            }
            else
            {
                if (dal.UpdateUserTaskPlan(model) > 0)
                    return FindByUserTaskPlanInfo(model.userid, Convert.ToInt32(model.id));
            }
            return null;
        }

        /// <summary>
        /// 添加微信分享数据
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddUserWechatSharetext(weChatShareTextModel model)
        {
            return dal.AddUserWechatSharetext(model);
        }


        /// <summary>
        /// 修改发送状态
        /// </summary>
        /// <param name="shareid">The shareid.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool UpdateUserShareTextStatus(long shareid)
        {
            return dal.UpdateUserShareTextStatus(shareid);
        }

        /// <summary>
        /// 添加微信发送失败数据
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddUserWeChatError(weChatUserWechatErrorModel model)
        {
            return dal.AddUserWeChatError(model);
        }



        /// <summary>
        /// 获取发送内容列表
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="taskid">The taskid.</param>
        /// <returns>List&lt;GoodsModel&gt;.</returns>
        public List<weChatShareTextModel> FindByUserWechatShareTextList(int userid, int taskid)
        {
            return dal.FindByUserWechatShareTextList(userid, taskid);
        }
        /// <summary>
        /// 获取发送内容列表
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="taskid">The taskid.</param>
        /// <param name="goodsid">The goodsid.</param>
        /// <returns>List&lt;weChatShareTextModel&gt;.</returns>
        public List<weChatShareTextModel> FindByUserWechatShareTextList(int userid, int taskid, int goodsid)
        {
            return dal.FindByUserWechatShareTextList(userid, taskid, goodsid);
        }

        /// <summary>
        /// 获取发送内容列表
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>List&lt;weChatShareTextModel&gt;.</returns>
        public List<weChatShareTextModel> FindByUserWechatShareTextList(int userid)
        {
            return dal.FindByUserWechatShareTextList(userid);
        }


        /// <summary>
        /// 生成淘口令
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="taskid">The taskid.</param>
        /// <param name="templateText">文案</param>
        /// <param name="callback">生成结果返回</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        public bool BuildTaskTpwd(int userid, int taskid, string templateText, Action<weChatShareTextModel> result = null)
        {
            var taskData = FindByUserTaskPlanInfo(userid, taskid);
            if (taskData == null || taskData.ExecStatus != 0) return false;

            if (string.IsNullOrEmpty(taskData.pidsText) || string.IsNullOrEmpty(taskData.goodsText)) return false;

            List<UserPidTaskModel> lst = JsonConvert.DeserializeObject<List<UserPidTaskModel>>(taskData.pidsText);
            List<UserPidTaskModel> lst2 = JsonConvert.DeserializeObject<List<UserPidTaskModel>>(taskData.goodsText);
            List<int> ids = new List<int>();
            //如果群数据和商品数据都为空时
            if (lst == null || lst2 == null) return false;

            lst.ForEach(item =>
            {
                if (!ids.Contains(item.id))
                    ids.Add(item.id);
            });
            //获取微信群数据
            var wechatlist = FindByUserWeChatGroup(userid, ids);
            ids.Clear();

            lst2.ForEach(item =>
            {
                if (!ids.Contains(item.id))
                    ids.Add(item.id);
            });
            //获取商品数据
            var goodslist = FindByUserGoodsList(userid, ids);

            //删除现有数据
            dal.DeleteUserWechatShareText(userid, taskid);
            foreach (var group in wechatlist)
            {
                //生成商品分享文本
                BuildShareText(userid, taskid, templateText, goodslist, group, result);
            }
            UpdateUserTaskPlanIsTpwd(taskid);
            return true;
        }


        /// <summary>
        ///生成商品文案
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="taskid">The taskid.</param>
        /// <param name="templateText">The template text.</param>
        /// <param name="data">The data.</param>
        /// <param name="group">The group.</param>
        /// <returns>true if XXXX, false otherwise.</returns>
        private bool BuildShareText(int userid, int taskid, string templateText, List<GoodsModel> data, weChatGroupModel group, Action<weChatShareTextModel> result = null)
        {
            if (data == null) return false;
            weChatShareTextModel share = new weChatShareTextModel()
            {
                userid = userid,
                taskid = taskid,
                status = 0,
                title = group.title
            };
            foreach (var item in data)
            {
                if (item.goodsPrice - item.couponPrice <= 0) continue;
                string url = GlobalConfig.couponUrl;
                url += "?src=ht_hot&activityId=" + item.couponId;
                url += "&itemId=" + item.goodsId;
                url += "&pid=" + (string.IsNullOrEmpty(group.pid) ? "mm_33648229_22032774_73500078" : group.pid);
                item.shareLink = url;
                string shortUrl = HotTaoApiService.Instance.taobao_tbk_spread_get(item.shareLink);
                string tpwd = HotTaoApiService.Instance.taobao_wireless_share_tpwd_create(item.goodsMainImgUrl, item.shareLink, item.goodsName);
                string text = templateText;
                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(tpwd))
                {
                    while (text.Contains("[商品标题]"))
                        text = text.Replace("[商品标题]", item.goodsName);
                    while (text.Contains("[商品价格]"))
                        text = text.Replace("[商品价格]", item.goodsPrice.ToString());
                    while (text.Contains("[券后价格]"))
                        text = text.Replace("[券后价格]", (item.goodsPrice - item.couponPrice).ToString());
                    while (text.Contains("[二合一淘口令]"))
                        text = text.Replace("[二合一淘口令]", tpwd);
                    while (text.Contains("[短链接]"))
                        text = text.Replace("[短链接]", shortUrl);
                    while (text.Contains("[来源]"))
                        text = text.Replace("[来源]", item.goodsSupplier);
                    while (text.Contains("[销量]"))
                        text = text.Replace("[销量]", item.goodsSalesAmount.ToString());
                    while (text.Contains("[优惠券价格]"))
                        text = text.Replace("[优惠券价格]", item.couponPrice.ToString());
                    while (text.Contains("[分隔符]"))
                        text = text.Replace("[分隔符]", "-----------------");

                }
                else
                    share.status = 2;
                share.goodsid = Convert.ToInt32(item.id);
                share.text = text;
                share.tpwd = tpwd;
                AddUserWechatSharetext(share);
                result?.Invoke(share);
            }
            return true;
        }


        #endregion
    }
}