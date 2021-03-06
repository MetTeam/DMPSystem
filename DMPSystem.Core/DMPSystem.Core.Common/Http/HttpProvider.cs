﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DMPSystem.Core.Common.ServicesException;

namespace DMPSystem.Core.Common.Http
{
   public class HttpProvider
    {
        public static int Timeout = 10 * 1000;

        public static Encoding Encoding = Encoding.UTF8;

        public HttpProvider(string url)
        {
            Url = url;
        }

        public string Url { get; set; }


        /// <summary>
        ///     发送Get请求
        /// </summary>
        /// <param name="formData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(FormData formData)
        {
            return Deserialize<T>(GetString(formData));
        }


        public T Get<T, Q>(Q queryModel)
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            return Deserialize<T>(GetString(s));
        }

        /// <summary>
        ///     请GET方式请求，将响应序列化为<paramref name="anonymous" />的结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formData"></param>
        /// <param name="anonymous"></param>
        /// <returns></returns>
        public T GetAnonymous<T>(FormData formData, T anonymous)
        {
            return Get<T>(formData);
        }

        public T GetAnonymous<T, Q>(Q queryModel, T anonymous)
        {
            return Get<T, Q>(queryModel);
        }

        /// <summary>
        ///     以GET方式请求，返回响应的字符中
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public string GetString(FormData formData)
        {
            return GetString(Url, formData);
        }

        public string GetString(string formData)
        {
            return GetString(Url, formData);
        }


        /// <summary>
        ///     以POST方式提交数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public T Post<T>(string body, FormData formData = null)
        {
            var url = String.Format("{0}{1}", Url, formData == null ? "" : string.Format("?{0}", formData.Format()));
            return Post<T>(url, body);
        }

        public T Post<T, Q>(string body, Q queryModel = default(Q))
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            var url = String.Format("{0}{1}", Url, s ?? "");
            return Post<T>(url, body);
        }

        /// <summary>
        ///     以post方式提交，将响应编码为字串。
        /// </summary>
        /// <param name="body"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public string PostString(string body, FormData formData = null)
        {
            var url = String.Format("{0}{1}", Url, formData == null ? "" : string.Format("?{0}", formData.Format()));
            return PostString(url, body);
        }

        public string PostString<Q>(string body, Q queryModel = default(Q))
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            var url = String.Format("{0}{1}", Url, s ?? "");
            return PostString(url, body);
        }


        /// <summary>
        ///     上传文件。formData参数附加到url
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string Upload(FormData formData, string filePath)
        {
            var url = String.Format("{0}?{1}", Url, formData == null ? "" : formData.Format());
            var data = new WebClient().UploadFile(url, "POST", filePath);
            return Encoding.GetString(data);
        }

        public string Upload<Q>(string filePath, Q queryModel = default(Q))
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            var url = String.Format("{0}?{1}", Url, s ?? "");
            var data = new WebClient().UploadFile(url, "POST", filePath);
            return Encoding.GetString(data);
        }

        /// <summary>
        ///     GET 方式获取响应流
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="queryString">请求参数</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应流</returns>
        public static Stream Get(string url, string queryString = null, CookieContainer cc = null, string refer = null)
        {
            url = String.Format("{0}?{1}", url, queryString);
            var r = CreateHttpWebRequest(url);
            r.Referer = refer;
            r.CookieContainer = cc;
            r.Method = "GET";
            return r.GetResponse().GetResponseStream();
        }

       /// <summary>
        ///  GET 方式获取响应流
       /// </summary>
       /// <typeparam name="Q">请求参数模型</typeparam>
       /// <param name="url">url地址</param>
        /// <param name="queryModel">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应流</returns>
        public static Stream Get<Q>(string url, Q queryModel = default(Q),
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            return Get(url, s, cc, refer);
        }

       /// <summary>
        /// GET 方式获取响应流
       /// </summary>
       /// <typeparam name="T">返回实体对象</typeparam>
       /// <typeparam name="Q">请求的参数实体对象类型</typeparam>
       /// <param name="url">url地址</param>
       /// <param name="queryModel">请求的参数实体对象</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
       /// <returns>返回响应流</returns>
        public static T Get<T, Q>(string url, Q queryModel = default(Q),
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            var json = new StreamReader(Get(url, s, cc, refer), Encoding).ReadToEnd();
            return Deserialize<T>(json);
        }

       /// <summary>
       /// Get 方式获取响应流
       /// </summary>
       /// <param name="url">url地址</param>
       /// <param name="data">请求的参数实体对象类型</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
       /// <returns>返回响应流</returns>
        public static Stream Get(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            return Get(url, FormatData(data), cc, refer);
        }
       
       /// <summary>
       /// Get 方式获取响应流
       /// </summary>
       /// <typeparam name="T">请求返回实体模型</typeparam>
       /// <param name="url">url地址</param>
        /// <param name="data">请求的参数键值对</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应流</returns>
        public static T Get<T>(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            var json = new StreamReader(Get(url, s, cc, refer), Encoding).ReadToEnd();
            return Deserialize<T>(json);
        }

       /// <summary>
        ///   以GET方式请求，返回响应的字符中
       /// </summary>
       /// <param name="url">url地址</param>
       /// <param name="queryString">请求参数</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应流</returns>
        public static string GetString(string url, string queryString = null, CookieContainer cc = null,
            string refer = null)
        {
            return new StreamReader(Get(url, queryString, cc, refer)).ReadToEnd();
        }

        /// <summary>
        ///     以GET方式请求，返回响应的字符中
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="data">请求的参数键值</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应字符串</returns>
        public static string GetString(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            return new StreamReader(Get(url, FormatData(data), cc, refer)).ReadToEnd();
        }

       /// <summary>
        /// 以GET方式请求，返回响应的字符串
       /// </summary>
       /// <typeparam name="Q">请求参数模型类型</typeparam>
       /// <param name="url">url地址</param>
       /// <param name="queryModel">请求参数模型</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应字符串</returns>
        public static string GetString<Q>(string url, Q queryModel = default(Q),
          CookieContainer cc = null, string refer = null)
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            return new StreamReader(Get(url, s, cc, refer)).ReadToEnd();
        }


        /// <summary>
        ///     POST 方式获取响应流
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="data">写入正文部分的数据</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应字符串</returns>
        public static Stream Post(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            var r = CreateHttpWebRequest(url);
            r.Method = "POST";
            r.Referer = refer;
            r.CookieContainer = cc;
            r.ContentType = "application/x-www-form-urlencoded";
            if (data != null)
            {
                var bs = Encoding.GetBytes(data);
                r.ContentLength = bs.Length;
                var stream = r.GetRequestStream();
                stream.Write(bs, 0, bs.Length);
                stream.Flush();
                stream.Close();
            }
            var rep = r.GetResponse();
            return rep.GetResponseStream();
        }

       /// <summary>
        /// POST 方式获取响应流
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="url">url地址</param>
        /// <param name="data">写入正文请求参数数据</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应字符串</returns>
        public static T Post<T>(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            var str = new StreamReader(Post(url, data, cc, refer), Encoding).ReadToEnd();
            return Deserialize<T>(str);
        }

    

       /// <summary>
       /// POST方式获取响应流
       /// </summary>
       /// <typeparam name="T">返回实体模型类型</typeparam>
       /// <param name="url">url地址</param>
        /// <param name="data">写入正文请求参数数据</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回响应实体模型</returns>
        public static T Post<T>(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            return Post<T>(url, s, cc, refer);
        }

       /// <summary>
        ///  POST方式获取实体模型
       /// </summary>
       /// <typeparam name="T">返回实体模型类型</typeparam>
       /// <typeparam name="Q">请求参数实体模型类型</typeparam>
       /// <param name="url">url地址</param>
       /// <param name="queryModel">请求参数模型</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
       /// <returns>返回实体模型</returns>
        public static T Post<T, Q>(string url, Q queryModel = default(Q),
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatQueryParaProvider.FormatQueryParaMap(queryModel, true, true);
            return Post<T>(url, s, cc, refer);
        }

       /// <summary>
       /// Post方式返回字符串
       /// </summary>
       /// <param name="url">url地址</param>
       /// <param name="data">请求参数</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回字符串</returns>
        public static string PostString(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            var str = new StreamReader(Post(url, data, cc, refer), Encoding).ReadToEnd();
            return str;
        }

        /// <summary>
        /// 获取响应的状态
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="data">请求参数</param>
        /// <param name="cc">为System.Net.CookieCollection对象的集合提供容器</param>
        /// <param name="refer">获取和设置http标头值</param>
        /// <returns>返回状态</returns>
        public static HttpStatusCode HeadHttpCode(string url, string data = null,
            CookieContainer cc = null, string refer = null)
        {
            try
            {
                var r = CreateHttpWebRequest(url);
                r.Method = "HEAD";
                r.Referer = refer;
                r.CookieContainer = cc;
                return (r.GetResponse() as HttpWebResponse).StatusCode;
            }
            catch
            {
                return HttpStatusCode.ExpectationFailed;
            }
        }

        /// <summary>
        ///     创建一个请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebRequest CreateRequest(string url)
        {
            var r = WebRequest.Create(url) as HttpWebRequest;
            return r;
        }

        /// <summary>
        ///     创建一个请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpWebRequest(string url)
        {
            var r = CreateRequest(url);
            r.Timeout = Timeout;
            //r.Headers["X-Requested-With"] = "XMLHttpRequest";
            var s =
                "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";
            r.UserAgent = s;
            return r;
        }

        /// <summary>
        ///     将json反序列化为对象。
        ///     如果参数json不为null（或空串）但不能序列化为类型T，将抛出DesializeException异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private static T Deserialize<T>(string json)
        {
            var ret = JsonConvert.DeserializeObject<T>(json);

            //抛出异常
            if (!string.IsNullOrEmpty(json) && ret == null)
                throw new ServiceException(string.Format("不能反序列化字符串为类型 ‘{0}’,字符串：{1}", typeof(T), json));

            return ret;
        }

        private static string FormatData(IEnumerable<KeyValuePair<string, object>> data)
        {
            return new FormData(data).Format();
        }
    }



    public class FormData : Dictionary<string, object>
    {
        public FormData()
        {
        }

        public FormData(IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (var keyValuePair in data)
            {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        ///     转换为http form格式字符串
        /// </summary>
        /// <returns></returns>
        public virtual string Format()
        {
            var lst = this.Select(e => String.Format("{0}={1}", e.Key, Uri.EscapeDataString(Convert.ToString(e.Value))));
            return String.Join("&", lst);
        }
    }
}
