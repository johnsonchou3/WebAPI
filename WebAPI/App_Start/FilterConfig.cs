using System.Web;
using System.Web.Mvc;

namespace WebAPI
{
    /// <summary>
    /// 模版自動產生文件
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 模版自動產生文件
        /// </summary>
        /// <param name="filters">使用filters</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
