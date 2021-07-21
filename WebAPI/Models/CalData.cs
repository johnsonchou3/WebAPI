using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace WebAPI.Models
{
    /// <summary>
    /// 計算機所有資料, 作為回傳給client 的object
    /// </summary>
    public class CalData
    {
        /// <summary>
        /// 判斷目前是否在AfterBracket, 以免operation或execute 出現格式錯誤
        /// </summary>
        public bool IsAfterBracket { get; set; } = false;

        /// <summary>
        /// 判斷目前是否在operation, 在operation當中再按operation 會修改operator
        /// </summary>
        public bool IsOperating { get; set; } = false;

        /// <summary>
        /// 目前的運算式, 每按operation/bracket/execute 都會使其更新, 在execute/ClearAll後會清空
        /// </summary>
        public string StringOfOperation { get; set; }

        /// <summary>
        /// 讓form1 讀取顯示, 透過storedisplay存取stringofoperation, 唯execute後不清空以便用家看到式子
        /// </summary>
        public string DisplayOperation { get; set; }

        /// <summary>
        /// 讓form1 讀取顯示, 在execute後會顯示expressiontree 的前序
        /// </summary>
        public string Preordstring { get; set; }

        /// <summary>
        /// 讓form1 讀取顯示, 在execute後會顯示expressiontree 的中序
        /// </summary>
        public string Inordstring { get; set; }

        /// <summary>
        /// 讓form1 讀取顯示, 在execute後會顯示expressiontree 的後序
        /// </summary>
        public string Postordstring { get; set; }

        /// <summary>
        /// 目前輸入的String, 會顯示在form1textbox 讓用家知道目前輸入
        /// </summary>
        public string TempInputString { get; set; } = "0";

        /// <summary>
        /// 存入每個operand 及operator 的List, 以便在execute 創建tree
        /// </summary>
        public List<string> Expressionlist { get; set; } = new List<string>();

        /// <summary>
        /// 內建的儲存displayoperation method 方便使用
        /// </summary>
        public void StoretoDisplay()
        {
            DisplayOperation = StringOfOperation;
        }
    }
}