//MathController.cs file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using WebAPI.Models;
using System.Runtime.Caching;
using System.Net.Http.Headers;
using WebAPI.Handler;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 計算機的controller
    /// </summary>
    public class MathController : ApiController
    {
        /// <summary>
        /// 計算機的cache, 會有所有用家的caldata
        /// </summary>
        private MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// 每個行動前會確認server cache 有沒有用家資料, 沒有則新增
        /// </summary>
        /// <param name="cache">server memorycache</param>
        /// <param name="IdKey">cookie 內的ID, 由cookie handler 指派</param>
        /// <returns>回傳用家的caldata</returns>
        private static CalData GetorCreate(MemoryCache cache, string IdKey)
        {
            CalData caldata = new CalData();
            if (cache.Get(IdKey) != null)
            {
                caldata = (CalData)cache.Get(IdKey);
            }
            else
            {
                cache.Add(IdKey, caldata, null);
            }
            return caldata;
        }

        /// <summary>
        /// 數字鍵的Post request, 把按鍵值加到TempInputString最後
        /// </summary>
        /// <param name="btnnum">數字鍵本身的值</param>
        /// <returns>回傳新的caldata</returns>
        public CalData Numpad(string btnnum)
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            caldata.IsOperating = false;
            AddNum();
            caldata.StoretoDisplay();
            return caldata;

            // 把按鍵值加到TempInputString最後
            void AddNum()
            {
                try
                {
                    caldata.TempInputString += btnnum;
                    caldata.TempInputString = double.Parse(caldata.TempInputString).ToString();
                }
                catch (FormatException)
                {
                    caldata.TempInputString = btnnum;
                }
            }
        }
        
        /// <summary>
        /// Operator 的Post Request, 會把tempinput 及operator 放到stringofoperation 作儲存
        /// </summary>
        /// <param name="btnop">使用鍵本身的值</param>
        /// <returns>回傳新的caldata</returns>
        public CalData Operation(string btnop)
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            if (caldata.IsOperating)
            {
                caldata.StringOfOperation = caldata.StringOfOperation.Remove(caldata.StringOfOperation.Length - 1, 1) + btnop;
            }
            else
            {
                caldata.IsOperating = true;
                if (caldata.IsAfterBracket)
                {
                    caldata.Expressionlist.Add(btnop);
                    caldata.StringOfOperation += btnop;
                    caldata.IsAfterBracket = false;
                }
                else
                {
                    SaveValue();
                    ClearTemp();
                }
            }
            caldata.StoretoDisplay();
            return caldata;

            // 把TempInputString及目前的operator寫入StringOfOperation中
            void SaveValue()
            {
                caldata.StringOfOperation += double.Parse(caldata.TempInputString).ToString() + btnop;
                caldata.Expressionlist.Add(caldata.TempInputString);
                caldata.Expressionlist.Add(btnop);
            }

            // 寫入後把TempInputString清空作下一次儲存
            void ClearTemp()
            {
                caldata.TempInputString = "0";
            }
        }

        /// <summary>
        /// Execute的Post Request, 把TempInputString打包, 並透過運算StringOfOperation, 結果存在TempInputString
        /// </summary>
        /// <returns>回傳新的caldata</returns>
        public CalData Execute()
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            if (!caldata.IsAfterBracket)
            {
                SaveValue();
                caldata.IsAfterBracket = false;
            }
            Node ExpTree = Node.CreateTree(caldata.Expressionlist);
            caldata.Preordstring = "Pre-Order: \n";
            caldata.Inordstring = "In-Order: \n";
            caldata.Postordstring = "Post-Order: \n";
            GetPreorder(ExpTree);
            GetInorder(ExpTree);
            GetPostorder(ExpTree);
            caldata.TempInputString = GetResult(ExpTree).ToString();
            caldata.DisplayOperation = caldata.StringOfOperation;
            caldata.StringOfOperation = string.Empty;
            caldata.Expressionlist.Clear();
            return caldata;

            // 把目前輸入值加進運算式中
            void SaveValue()
            {
                caldata.StringOfOperation += caldata.TempInputString;
                caldata.Expressionlist.Add(caldata.TempInputString);
            }

            // 把運算式(string) 加到URL POST 給WebAPI 作運算並回傳結果(string)
            double GetResult(Node root)
            {
                if (root != null)
                {
                    if (root.Left == null && root.Right == null)
                    {
                        return double.Parse(root.Value);
                    }
                    double left_val = GetResult(root.Left);
                    double right_val = GetResult(root.Right);
                    Dictionary<string, double> OperationMap = new Dictionary<string, double>();
                    OperationMap.Add("+", left_val + right_val);
                    OperationMap.Add("-", left_val - right_val);
                    OperationMap.Add("*", left_val * right_val);
                    OperationMap.Add("/", left_val / right_val);
                    return OperationMap[root.Value];
                }
                return 0;
            }

            // 以前序編歷Tree, 並把value 加到Preordstring 以顯示
            void GetPreorder(Node root)
            {
                if (root != null)
                {
                    caldata.Preordstring += root.Value + " ";
                    GetPreorder(root.Left);
                    GetPreorder(root.Right);
                }
            }

            // 以中序編歷Tree, 並把value 加到Preordstring 以顯示
            void GetInorder(Node root)
            {
                if (root != null)
                {
                    GetInorder(root.Left);
                    caldata.Inordstring += root.Value + " ";
                    GetInorder(root.Right);
                }
            }

            // 以後序編歷Tree, 並把value 加到Preordstring 以顯示
            void GetPostorder(Node root)
            {
                if (root != null)
                {
                    GetPostorder(root.Left);
                    GetPostorder(root.Right);
                    caldata.Postordstring += root.Value + " ";
                }
            }
        }

        /// <summary>
        /// 開根號的post request, 把TempInputString作開根號
        /// </summary>
        /// <returns>回傳新的caldata</returns>
        public CalData Root()
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            double tempnum = double.Parse(caldata.TempInputString);
            caldata.TempInputString = Math.Sqrt(tempnum).ToString();
            return caldata;
        }

        /// <summary>
        /// ClearEntry 的Post Request, 清空TempInputString
        /// </summary>
        /// <returns>回傳新的caldata</returns>
        public CalData ClearEntry()
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            caldata.TempInputString = "0";
            return caldata;
        }

        /// <summary>
        /// ClearAll的Post Request, 清空tempinput及把之前的所有輸入移除
        /// </summary>
        /// <returns></returns>
        public CalData ClearAll()
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            caldata.TempInputString = "0";
            ClearDatas();
            caldata.StoretoDisplay();
            return caldata;

            void ClearDatas()
            {
                caldata.StringOfOperation = string.Empty;
                caldata.Expressionlist.Clear();
            }
        }

        /// <summary>
        /// Decimal 的Post Request, 會在Tempinputstring 加上.數, 不作decimal轉換
        /// </summary>
        /// <returns>回傳新的caldata</returns>
        public CalData Dec()
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            caldata.TempInputString += ".";
            return caldata;
        }

        /// <summary>
        /// 正負號的Post Request, 多加+/-在TempInputString前面
        /// </summary>
        /// <returns>回傳新的caldata</returns>
        public CalData PosNeg()
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            SwitchPosNeg();
            return caldata;

            void SwitchPosNeg()
            {
                string txtboxstr = caldata.TempInputString;
                decimal reversed = decimal.Parse(txtboxstr) * (-1);
                caldata.TempInputString = reversed.ToString();
            }
        }

        /// <summary>
        /// Backspace的Post Request, 將TempInputString 最後char 刪除
        /// </summary>
        /// <returns>回傳新的caldata</returns>
        public CalData Backspace()
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            try
            {
                RemoveLastDigit();
            }
            catch (ArgumentOutOfRangeException)
            {
                caldata.TempInputString = "0";
            }
            return caldata;

            void RemoveLastDigit()
            {
                string curtextbox = caldata.TempInputString;
                caldata.TempInputString = (curtextbox).Remove(Math.Max(1, curtextbox.Length - 1));
            }
        }

        /// <summary>
        /// 開括號的Post Request, 把"(" 存進Expressionlist 及Stringofoperation, 並更新exeoper
        /// </summary>
        /// <param name="bracket">使用按鍵本身括號值</param>
        /// <returns>回傳新的caldata</returns>
        public CalData BracketOp(string bracket)
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            caldata.Expressionlist.Add(bracket);
            caldata.StringOfOperation += bracket;
            caldata.StoretoDisplay();
            return caldata;
        }

        /// <summary>
        /// 關括號的Post request, 加入關括號, 把目前的輸入字串存起來, 並把IsAfterBracket 改為True 讓Operation 及 Execute 作判斷以免出錯
        /// </summary>
        /// <param name="bracket">使用按鍵本身括號值</param>
        /// <returns>回傳新的caldata</returns>
        public CalData BracketClose(string bracket)
        {
            string Idkey = Request.Properties[CookieHandler.Id].ToString();
            CalData caldata = GetorCreate(cache, Idkey);
            caldata.StringOfOperation += caldata.TempInputString;
            caldata.Expressionlist.Add(caldata.TempInputString);
            caldata.Expressionlist.Add(bracket);
            caldata.StringOfOperation += bracket;
            caldata.StoretoDisplay();
            caldata.TempInputString = "0";
            caldata.IsAfterBracket = true;
            return caldata;
        }
    }
}