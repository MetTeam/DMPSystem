﻿/*----------------------------------------------------
 * 作者:范  亮
 * 创建时间：2015-11-28
 * ------------------修改记录-------------------
 * 修改人      修改日期        修改目的
 * 范  亮      2015-11-28      创建
 ----------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMPSystem.Core.Common.Extensions;
using DMPSystem.Core.Logger;

namespace DMPSystem.Core.Common.ServicesException
{
    /// <summary>
    /// 订单支付中心执行过程中发生的错误
    /// </summary>
    public sealed class ServiceException : Exception
    {
        /// <summary>
        /// 初始化 System.Exception 类的新实例。
        /// </summary>
        public ServiceException()
        {
        }

        /// <summary>
        /// 使用指定的错误信息初始化 System.Exception 类的新实例。
        /// </summary>
        /// <param name="message">错误信息</param>
        public ServiceException(string message)
            : base(message)
        {
            Message = message;
        }



        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 System.Exception 类的新实例。
        /// </summary>
        /// <param name="message"> 解释异常原因的错误信息。</param>
        /// <param name="e">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用</param>
        public ServiceException(string message, Exception e)
            : base(message, e)
        {
            Message = string.IsNullOrEmpty(message) ? e.Message : message;
            this.Source = e.Source;
        }

        /// <summary>
        /// 错误号
        /// </summary>
        public int Code
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public new string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 使用指定的枚举初始化 System.Exception 类的新实例
        /// </summary>
        /// <param name="sysCode">错误号</param>
        public ServiceException(Enum sysCode)
            : base(sysCode.GetDisplay())
        {
            this.Code = (int)Enum.Parse(sysCode.GetType(), sysCode.ToString());
            this.Message = sysCode.GetDisplay();
            Log.Write(
                  string.Format("\r\n微信回调请求： {0} + ！\r\n错误信息：{1}\r\n错误号:{2}", Source, this.Message,this.Code)
                                , MessageType.Error, this.GetType(), this);
        }

        public ServiceException(Enum sysCode,string message)
        {
            this.Code = (int)Enum.Parse(sysCode.GetType(), sysCode.ToString());
            this.Message =string.Format(message, sysCode.GetDisplay());
            Log.Write(
                 string.Format("\r\n微信回调请求： {0} + ！\r\n错误信息：{1}\r\n错误号:{2}", Source, this.Message, this.Code)
                               , MessageType.Error, this.GetType(), this);
        }

       /// <summary>
        /// 通过自定义错误枚举对象获取ServiceException
       /// </summary>
       /// <typeparam name="T">自定义错误枚举</typeparam>
        /// <returns>返回ServiceException</returns>
        public ServiceException GetServiceException<T>()
        {
           var code= Message.Substring(Message.LastIndexOf("错误号", System.StringComparison.Ordinal)+3);
           var sysCode = Enum.Parse(typeof(T), code);
           this.Code = (int)Enum.Parse(sysCode.GetType(), sysCode.ToString());
           Log.Write(
                string.Format("\r\n微信回调请求： {0} + ！\r\n错误信息：{1}\r\n错误号:{2}", Source, this.Message, this.Code)
                              , MessageType.Error, this.GetType(), this);
           return this;
        }
    }
}
