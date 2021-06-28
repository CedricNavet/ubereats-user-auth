using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ubereats_user_auth.Controllers
{

    //[Microsoft.AspNetCore.Cors.EnableCors(PolicyName = "AllowWebApp")]
    [Route("api/cpu")]
    [ApiController]
    public class CPUServer : ControllerBase
    {
        // POST: api/auth/login
        [HttpGet]
        public async Task<ActionResult<string>> CPUCharge()
        {
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            var cpu = proc.TotalProcessorTime;
            return (mem / 1024.0).ToString();

        }
    }
}
