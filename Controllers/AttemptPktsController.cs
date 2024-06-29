using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using Pkt.Data;
using Pkt.Models;
using Pkt.Models.Entites;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pkt.Controllers
{

    public class ManagePktDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public class AttemptPktsController : Controller
    {
        private readonly AppDbContext _context;

        // Global variables

        public AttemptPktsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AttemptPkts
        public async Task<IActionResult> Index()
        {
             var userName = HttpContext.Session.GetString("empName");
             var userId = HttpContext.Session.GetString("empID");
             var OH = HttpContext.Session.GetString("OH");
             var AH = HttpContext.Session.GetString("AH");
             var QH = HttpContext.Session.GetString("QH");
             var TH = HttpContext.Session.GetString("TH");
           
            if(userId == OH || userId == QH || userId == AH || userId == TH)
            {
                var cmid = HttpContext.Session.GetString("CMID");
                var query = _context.attemptPkts
                    .Where(a => a.Cm_Id == Convert.ToInt32(cmid) && a.Status == 1)
                    .AsEnumerable() 
                    .Where(a => DateTime.Now < a.EndDate)
                    .ToList(); 
               return View(query);
            }
            else
            {
                TempData["error"] = "You are not authorised for this route";
                return RedirectToAction("Index", "Home");
            }

            
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cm_Id,Status,PKTName,EmployeeId,EmployeeName,StartDate,EndDate,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,Attempts")] AttemptPkt attemptPkt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attemptPkt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attemptPkt);
        }

        
       
        
        private bool AttemptPktExists(int id)
        {
            return _context.attemptPkts.Any(e => e.Id == id);
        }
        
        //GTE: AttemptPkts/Start
        [HttpGet]
        public IActionResult Start()
        {
            var userName = HttpContext.Session.GetString("empName");
            var userId = HttpContext.Session.GetString("empID");
            var OH = HttpContext.Session.GetString("OH");
            var AH = HttpContext.Session.GetString("AH");
            var QH = HttpContext.Session.GetString("QH");
            var TH = HttpContext.Session.GetString("TH");

            if (userId != OH && userId != QH && userId != AH && userId != TH)
            {
                return View();
            }
            else
            {
                TempData["error"] = "You are not authorised for this route";
                return RedirectToAction("Index", "Home");
            }
        }

		//GET: get PKT Name 
		[HttpGet]
        public IActionResult GetPktName()
        {
            var CMID = HttpContext.Session.GetString("CMID");
            var EmployeeID = HttpContext.Session.GetString("empID");
         	var query = _context.managepkt
                .Where(p => p.Cm_Id == Convert.ToInt32(CMID) && _context.pktFors.Any(a => a.UserId == EmployeeID && a.PktName == p.Name) &&
                     !_context.attemptPkts.Any(a => a.EmployeeId == EmployeeID && a.Status == 1 && a.PKTName == p.Name))
                .AsEnumerable().Where(d => d.StartDate < DateTime.Now && d.EndDate > DateTime.Now);
              var results = query.ToList();
            return new JsonResult(results);
        }

        public IActionResult GetCountDuration(int? id)
        {
            var countDuration = _context.managepkt
                   .Where(x => x.Id == id)
                    .Select(x => x.TestDuration)
                    .FirstOrDefault();
            return new JsonResult(countDuration);
         }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewQuestion(int ? PKTName, int ? duration)
        {

             var pktRow = _context.managepkt.FirstOrDefault(x => x.Id == PKTName);
             var attempts = _context.attemptPkts
            .Where(a => a.PKTName == pktRow.Name && a.EmployeeId == HttpContext.Session.GetString("empID"))
            .Select(a => a.Attempts)
            .FirstOrDefault();

            if (attempts != null && attempts != 0)
            {
                var attempt = Convert.ToInt32(attempts);
                attempt++;
                var updateAttempt = _context.attemptPkts
                    .Where(a => a.PKTName == pktRow.Name && a.EmployeeId == HttpContext.Session.GetString("empID"))
                    .FirstOrDefault();
                if (updateAttempt != null)
                {
                    updateAttempt.Status = 1;
                    updateAttempt.Attempts = attempt;
                }
            }
            else
            {
                    AttemptPkt attemptPkt = new AttemptPkt();
                    attemptPkt.Cm_Id = Convert.ToInt32(HttpContext.Session.GetString("CMID"));
                    attemptPkt.Status = 1;
                    attemptPkt.PKTName = pktRow.Name;
                    attemptPkt.EmployeeId = HttpContext.Session.GetString("empID");
                    attemptPkt.EmployeeName = HttpContext.Session.GetString("empName");
                    attemptPkt.CreatedBy = HttpContext.Session.GetString("empID");
                    attemptPkt.StartDate = Convert.ToDateTime(pktRow.StartDate);
                    attemptPkt.EndDate = Convert.ToDateTime(pktRow.EndDate);
                    attemptPkt.Attempts = 1;
                    attemptPkt.CreatedOn = DateTime.Now;
                   _context.Add(attemptPkt);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Start));
            }

            HttpContext.Session.SetString("PKTName", pktRow.Name);
            HttpContext.Session.SetString("Duration", Convert.ToString(pktRow.TestDuration));
            HttpContext.Session.SetString("NoQuestion", Convert.ToString(pktRow.NoQuestion));
           
            var model = _context.manageQuestions
                    .Where(q => q.PKTName == pktRow.Name)
                    .OrderBy(q => Guid.NewGuid()) 
                    .Take(Convert.ToInt32(pktRow.NoQuestion))
                    .ToList();
           
            return View(model);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Manage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetRowAttemptPkts(int? id)
        {
            var row = _context.attemptPkts.FirstOrDefault(d=>d.Id == id);
            return new JsonResult(row);
        }

        [HttpPost]
        public IActionResult UpdateRowAttemptPkts(AttemptPkt attempt)
        {

            var  attemptPkt = _context.attemptPkts.FirstOrDefault(a=> a.Id == attempt.Id);
             if (attemptPkt != null)
                {
                   attemptPkt.Status = attempt.Attempts;
                }
             var deleteRow = _context.manageAnswer.Where(ma => ma.EmployeeId == attemptPkt.EmployeeId && ma.PKTName == attemptPkt.PKTName);
             if (deleteRow != null)
              {
                _context.manageAnswer.RemoveRange(deleteRow);
                _context.SaveChanges();
              }
            _context.SaveChanges();
            return Json(new { message = "Pkt Details Updated Successfully !", success = true, StatusCode = 200 });
        }

      
        [HttpPost]
        public IActionResult  ManageAnswer(List<ManageAnswer> manage)
        {
            //string dateString = DateTimeOffset.Now.ToString("dd-MM-yyyy HH:mm:ss");
            //DateTime dateTimes = DateTime.ParseExact(dateString, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            string formattedDdateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            ManageAnswer[] manageArray = JsonConvert.DeserializeObject<ManageAnswer[]>(JsonConvert.SerializeObject(manage));
            foreach (var item in manageArray)
            {
                ManageAnswer QuizRow = new ManageAnswer();
                QuizRow.Cm_Id = item.Cm_Id;
                QuizRow.EmployeeId = item.EmployeeId;
                QuizRow.EmployeeName = item.EmployeeName;
                QuizRow.PKTName = item.PKTName;
                QuizRow.AnsOption = item.AnsOption;
                QuizRow.CreatedOn = item.CreatedOn;
                QuizRow.QuestionId = item.QuestionId;
                QuizRow.CreatedOn = DateTime.Now;
                _context.manageAnswer.Add(QuizRow);
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(ViewReport));
         }


        [HttpGet]
        public IActionResult ViewReport()
        {
            var empID = HttpContext.Session.GetString("empID");
            var pktName = HttpContext.Session.GetString("PKTName");

            var query = from a in _context.manageAnswer
                        join q in _context.manageQuestions on Convert.ToInt32(a.QuestionId) equals q.Id
                        where a.EmployeeId == empID && a.PKTName == pktName
                        group new { q, a } by new { a.EmployeeName, a.EmployeeId } into grouped
                        orderby grouped.FirstOrDefault().a.Id descending
                        select new 
                        {
                            EmployeeName = grouped.Key.EmployeeName,
                            EmployeeID = grouped.Key.EmployeeId,
                            TotalQuestion = grouped.Count(),
                            Correct_Ans = grouped.Sum(x => x.q.Answer == x.a.AnsOption ? 1 : 0),
                            Not_Attempt = grouped.Sum(x => string.IsNullOrEmpty(x.a.AnsOption) ? 1 : 0),
                            Wrong_Ans = grouped.Sum(x => x.q.Answer != x.a.AnsOption && !string.IsNullOrEmpty(x.a.AnsOption) ? 1 : 0)
                        };
                         var result = query.FirstOrDefault();
                            if (result == null) 
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["Not_Attempt"] = result.Not_Attempt;
                                TempData["Correct_Ans"] = result.Correct_Ans;
                                TempData["EmployeeID"] = result.EmployeeID;
                                TempData["EmployeeName"] = result.EmployeeName;
                                TempData["TotalQuestion"] = result.TotalQuestion;
                                TempData["Wrong_Ans"] = result.Wrong_Ans;

                                return View();
                            }
                        
                 }

        [HttpGet]
        public IActionResult Summary(string? process, string? fromDate, string? endDates)
        {
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(endDates))
            {
                var employeeID = HttpContext.Session.GetString("empID");
                string start = fromDate;
                string end = endDates;
                DateTime startDate = DateTime.ParseExact(start, "dd-MM-yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(end, "dd-MM-yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);
               if (string.IsNullOrEmpty(process))
                {
                    var query = (from a in _context.manageAnswer
                                 join q in _context.manageQuestions on Convert.ToInt32(a.QuestionId) equals q.Id
                                 join pkt in _context.managepkt on a.PKTName equals pkt.Name
                                 group new { a, q, pkt } by new { a.PKTName, a.EmployeeId } into g
                                 select new QueryResultViewModel
                                 {
                                     EmployeeName = g.Select(x => x.a.EmployeeName).FirstOrDefault(),
                                     EmployeeId = g.Key.EmployeeId,
                                     CreatedOn = g.Select(x => x.a.CreatedOn).FirstOrDefault(), // Keep as string
                                     PKTDate = g.Select(x => x.pkt.CreatedOn).FirstOrDefault(),
                                     Attemped = Convert.ToString(g.Select(x => x.a.CreatedOn == x.pkt.CreatedOn ? "Yes" : "No").FirstOrDefault()),
                                     PKTName = g.Key.PKTName,
                                     TotalQuestion = Convert.ToString(g.Count()),
                                     CorrectAns = Convert.ToString(g.Sum(x => x.q.Answer == x.a.AnsOption ? 1 : 0)),
                                     NotAttempt = Convert.ToString(g.Sum(x => x.a.AnsOption == "" ? 1 : 0)),
                                     Wrong_Ans = Convert.ToString(g.Sum(x => x.q.Answer != x.a.AnsOption && x.a.AnsOption != "" ? 1 : 0)),
                                     Percentage = Convert.ToString(Math.Round(g.Sum(x => x.q.Answer == x.a.AnsOption ? 1 : 0) * 100.0 / g.Count()))
                                  }).AsEnumerable().Where(d =>
                                     d.CreatedOn.Date >= Convert.ToDateTime(start).Date && // Compare with Date
                                     d.CreatedOn.Date <= Convert.ToDateTime(endDates).Date && // Compare with Date
                                     d.EmployeeId == employeeID);
                                 return View(query);
                }
                else if (process == "All")
                {
                     var query = (from a in _context.manageAnswer
                                join q in _context.manageQuestions on Convert.ToInt32(a.QuestionId) equals q.Id
                                join pkt in _context.managepkt on a.PKTName equals pkt.Name
                                 group new { a, q, pkt } by new { a.PKTName, a.EmployeeId } into g
                                select new QueryResultViewModel
                                {
                                    EmployeeName = g.Select(x => x.a.EmployeeName).FirstOrDefault(),
                                    EmployeeId = g.Key.EmployeeId,
                                    CreatedOn = g.Select(x => x.a.CreatedOn).FirstOrDefault(),
                                    PKTDate = g.Select(x => x.pkt.CreatedOn).FirstOrDefault(),
                                    Attemped = Convert.ToString(g.Select(x => x.a.CreatedOn == x.pkt.CreatedOn ? "Yes" : "No").FirstOrDefault()),
                                    PKTName = g.Key.PKTName,
                                    TotalQuestion = Convert.ToString(g.Count()),
                                    CorrectAns = Convert.ToString(g.Sum(x => x.q.Answer == x.a.AnsOption ? 1 : 0)),
                                    NotAttempt = Convert.ToString(g.Sum(x => x.a.AnsOption == "" ? 1 : 0)),
                                    Wrong_Ans = Convert.ToString(g.Sum(x => x.q.Answer != x.a.AnsOption && x.a.AnsOption != "" ? 1 : 0)),
                                    Percentage = Convert.ToString(Math.Round(g.Sum(x => x.q.Answer == x.a.AnsOption ? 1 : 0) * 100.0 / g.Count()))
                                }).AsEnumerable().Where(d =>
                                     d.CreatedOn.Date >= Convert.ToDateTime(start).Date && // Compare with Date
                                     d.CreatedOn.Date <= Convert.ToDateTime(endDates).Date);
                   return View(query);
                }
                else
                {
                     var query = (from a in _context.manageAnswer
                                join q in _context.manageQuestions on Convert.ToInt32(a.QuestionId) equals q.Id
                                join pkt in _context.managepkt on a.PKTName equals pkt.Name
                                group new { a, q, pkt } by new { a.PKTName, a.EmployeeId } into g
                                select new QueryResultViewModel
                                {   Cm_Id =Convert.ToInt32(g.Select(x=>x.pkt.Cm_Id).FirstOrDefault()),
                                    EmployeeName = g.Select(x => x.a.EmployeeName).FirstOrDefault(),
                                    EmployeeId = g.Key.EmployeeId,
                                    CreatedOn = g.Select(x => x.a.CreatedOn).FirstOrDefault(),
                                    PKTDate = g.Select(x => x.pkt.CreatedOn).FirstOrDefault(),
                                    Attemped = Convert.ToString(g.Select(x => x.a.CreatedOn == x.pkt.CreatedOn ? "Yes" : "No").FirstOrDefault()),
                                    PKTName = g.Key.PKTName,
                                    TotalQuestion = Convert.ToString(g.Count()),
                                    CorrectAns = Convert.ToString(g.Sum(x => x.q.Answer == x.a.AnsOption ? 1 : 0)),
                                    NotAttempt = Convert.ToString(g.Sum(x => x.a.AnsOption == "" ? 1 : 0)),
                                    Wrong_Ans = Convert.ToString(g.Sum(x => x.q.Answer != x.a.AnsOption && x.a.AnsOption != "" ? 1 : 0)),
                                    Percentage = Convert.ToString(Math.Round(g.Sum(x => x.q.Answer == x.a.AnsOption ? 1 : 0) * 100.0 / g.Count()))
                                }).AsEnumerable().Where(d =>
                                     d.CreatedOn.Date >= Convert.ToDateTime(start).Date && // Compare with Date
                                     d.CreatedOn.Date <= Convert.ToDateTime(endDates).Date && d.Cm_Id == Convert.ToInt32(process));
                    return View(query);
                }
            }
            return View();
        }


        [HttpGet]
        public IActionResult Reports(string? process, string? fromDate, string? endDates)
        {
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(endDates))
            {
                var employeeID = HttpContext.Session.GetString("empID");
                if (string.IsNullOrEmpty(process))
                {
                    var queryResult = (from a in _context.manageAnswer
                                       join q in _context.manageQuestions on Convert.ToInt32(a.QuestionId) equals q.Id
                                       join pkt in _context.managepkt on a.PKTName equals pkt.Name
                                       select new QueryResultViewModel
                                       {  EmployeeName = a.EmployeeName ?? "N/A",
                                           Process = pkt.Process ?? "N/A",
                                           PKTName = q.PKTName ?? "N/A",
                                           IsCorrect = q.Answer == a.AnsOption ? "Correct" : "Incorrect",
                                           Question = q.Question ?? "N/A",
                                           CorrectAns = q.Answer ?? "N/A",
                                           GivenAns = a.AnsOption ?? "N/A",
                                           EmployeeId = a.EmployeeId ?? "N/A",
                                           CreatedOn = a.CreatedOn,
                                       }).AsEnumerable()
                                      .Where(d => d.CreatedOn.Date <= Convert.ToDateTime(endDates).Date && d.CreatedOn.Date >= Convert.ToDateTime(fromDate).Date && d.EmployeeId == employeeID).ToList();
                    return View(queryResult);
                }
                else if (process == "All")
                {
                   
                    var queryResult = (from a in _context.manageAnswer
                                       join q in _context.manageQuestions on Convert.ToInt32(a.QuestionId) equals q.Id
                                       join pkt in _context.managepkt on a.PKTName equals pkt.Name
                                       select new QueryResultViewModel
                                       {
                                           EmployeeName = a.EmployeeName ?? "N/A",
                                           Process = pkt.Process ?? "N/A",
                                           PKTName = q.PKTName ?? "N/A",
                                           IsCorrect = q.Answer == a.AnsOption ? "Correct" : "Incorrect",
                                           Question = q.Question ?? "N/A",
                                           CorrectAns = q.Answer ?? "N/A",
                                           GivenAns = a.AnsOption ?? "N/A",
                                           EmployeeId = a.EmployeeId ?? "N/A",
                                           CreatedOn = a.CreatedOn
                                         }).AsEnumerable()
                                      .Where(d => d.CreatedOn.Date <= Convert.ToDateTime(endDates).Date && d.CreatedOn.Date >= Convert.ToDateTime(fromDate).Date).ToList();
                    return View(queryResult);
                }
                else
                {
                    var queryResult = (from a in _context.manageAnswer
                                       join q in _context.manageQuestions on Convert.ToInt32(a.QuestionId) equals q.Id
                                       join pkt in _context.managepkt on a.PKTName equals pkt.Name
                                       select new QueryResultViewModel
                                       {
                                           Cm_Id = Convert.ToInt32(pkt.Cm_Id),
                                           EmployeeName = a.EmployeeName ?? "N/A",
                                           Process = pkt.Process ?? "N/A",
                                           PKTName = q.PKTName ?? "N/A",
                                           IsCorrect = q.Answer == a.AnsOption ? "Correct" : "Incorrect",
                                           Question = q.Question ?? "N/A",
                                           CorrectAns = q.Answer ?? "N/A",
                                           GivenAns = a.AnsOption ?? "N/A",
                                           EmployeeId = a.EmployeeId ?? "N/A",
                                           CreatedOn = a.CreatedOn,
                                       }).AsEnumerable()
                                      .Where(d => 
                                       d.CreatedOn.Date <= Convert.ToDateTime(endDates).Date && d.CreatedOn.Date >= Convert.ToDateTime(endDates).Date && d.Cm_Id == Convert.ToInt32(process)).ToList();
                             return View(queryResult);
                }
             }
            return View();
         }

 
    }
}
