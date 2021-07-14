//MathController.cs file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;

namespace WebAPI.Controllers
{
    public class MathController : ApiController
    {
        public string Compute(string oper)
        {
            DataTable dt = new DataTable();
            return dt.Compute(oper,"").ToString();
        }

        [HttpGet]
        public string Get()
        {
            return "default";
        }
    }
}