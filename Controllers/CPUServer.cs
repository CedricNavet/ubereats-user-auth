using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ubereats_user_auth.Model;
using Microsoft.EntityFrameworkCore;

namespace ubereats_user_auth.Controllers
{

    //[Microsoft.AspNetCore.Cors.EnableCors(PolicyName = "AllowWebApp")]
    [Route("api/cpu")]
    [ApiController]
    public class CPUServer : ControllerBase
    {

        private readonly UberEatsAuthDBContext _context;

        public CPUServer(UberEatsAuthDBContext context)
        {
            _context = context;
        }
        // POST: api/auth/login
        [HttpGet]
        public async Task<ActionResult<string>> CPUCharge()
        {
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            var cpu = proc.TotalProcessorTime;
            return (mem / 1024.0).ToString();

        }

        [HttpGet("Charge")]
        public async Task<ActionResult<Stats>> Charge()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var stopWatch = new Stopwatch();
            // Start watching CPU
            stopWatch.Start();

            // Meansure something else, such as .Net Core Middleware
            var users = await _context.Users.ToListAsync();

            // Stop watching to meansure
            stopWatch.Stop();
            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            var cpuUsagePercentage = cpuUsageTotal * 100;

            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            var cpu = proc.TotalProcessorTime;
            Stats stats = new Stats()
            {
                CPU_Usage = cpuUsagePercentage,
                Mem_Process = (mem / 1024.0),
                Process_Time = cpuUsedMs,
            };
            return stats;

        }
    }
}
