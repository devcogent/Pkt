using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Pkt.Data;
using Pkt.Models;
using Pkt.Models.Entites;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Pkt.Controllers
{
    public class 
        
        
        ManagePktsController : Controller
    {
        private readonly AppDbContext _context;

        public ManagePktsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult QuestionList(ManageQuestion manage)
        {
            if (manage.PKTName == null)
            {
                return NotFound();
            }
            // Assuming you have a context named YourDbContext
            var query = from question in _context.manageQuestions
                        where question.PKTName == Convert.ToString(manage.PKTName)
                        orderby question.Id descending
                        select new
                        {
                            question.Id,
                            question.PKTName,
                            question.UploadType,
                            question.Question,
                            question.Option1,
                            question.Option2,
                            question.Option3,
                            question.Option4,
                            question.Answer,
                            question.CreatedOn
                        };

            var result = query.ToList();



            if (result == null)
            {
                return NotFound();
            }

            return new JsonResult(result);
        }
        //GET : ManagePkts/GetUniquePKT
        [HttpGet]
        public IActionResult GetUniquePKT()
        {
            var empid = HttpContext.Session.GetString("empID");
            string currentDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var result = _context.managepkt
                .Where(m => m.StartDate > DateTime.Now || m.CreatedBy == empid || m.StartDate == null)
                .Select(m => new { PKTName = m.Name, ID = m.Id })
                .Distinct()
                .ToList();
             return new JsonResult(result);  
        }

        [HttpGet]
        public IActionResult DeleteQuestion(int? id)
        {
            try
            {
                var manageQuestion = _context.manageQuestions.FirstOrDefault(d => d.Id == id);
                if(_context.attemptPkts.Any(d=>d.PKTName == manageQuestion.PKTName))
                {
                    return Json(new { message = "pkt already attempted question has been locked can not be delete .", error = true, StatusCode = 302 });
                }
                _context.manageQuestions.Remove(manageQuestion);
                _context.SaveChangesAsync();
                return Json(new { message = "Pkt Question Deleted Successfully !", success = true, StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, error = true, StatusCode = 302 });
            }
        }

        [HttpGet]
        public IActionResult EditQuestion(int? id)
        {
            try
            {
                var manageQuestion = _context.manageQuestions.FirstOrDefault(d => d.Id == id);
                if (_context.attemptPkts.Any(d => d.PKTName == manageQuestion.PKTName))
                {
                    return Json(new { message = "pkt already attempted question has been locked can not be edited .", error = true, StatusCode = 302 });
                }
               
                return new JsonResult(manageQuestion);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, error = true, StatusCode = 302 });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuestion(ManageQuestion manage)
        {
         
            var row = await _context.manageQuestions.FirstOrDefaultAsync(e => e.Id == manage.Id);
            
             if (row != null)
            {
                row.PKTName = manage.PKTName;
                row.Answer = manage.Answer;
                row.Question =  manage.Question;
                row.Option1 = manage.Option1;
                row.Option2 = manage.Option2;
                row.Option3 = manage.Option3;
                row.Option4 = manage.Option4;
                row.UploadType =manage.UploadType;
                row.UpdatedBy = HttpContext.Session.GetString("empID");
                row.UpdatedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new {message = "PKT Question Updated Successfully !", success = true, StatusCode = 200 });
            }


            return Json(new { message = "PKT Question Updated Failed !", error = true, StatusCode = 302 });
         }

        public IActionResult GetPKTRow(int? id)
        {
            var row = _context.managepkt.FindAsync(id);
            return new JsonResult(row);
        }


        [HttpGet]
        public IActionResult GetProcess()
        {
            /*using (var client = new HttpClient())
            {
                var employeeID =  HttpContext.Session.GetString("empID");
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("EmployeeID", employeeID));
                var content = new FormUrlEncodedContent(values);
                var response =  client.PostAsync("https://demo.cogentlab.com/erpm/Services/GetCmIdByEmpId.php", content);
                var responseString =  response.Result.Content.ReadAsStringAsync();
                return new JsonResult(responseString);
            }*/

            var responseString = _context.clientMasters.ToList();

            return new JsonResult(responseString);


        }


        [HttpGet]
        public IActionResult GetCountQuestion(string? PKTName)
        {

            var result = _context.manageQuestions
             .Where(q => q.PKTName == Convert.ToString(PKTName)) 
             .GroupBy(q => q.PKTName)
             .Select(g => new {
                 NoQuestion = g.Count(),
                 PKTName = g.Key
             }).FirstOrDefault(); 
            int count = result?.NoQuestion ?? 0; 
            string pktName = result?.PKTName ?? "";
           
            return new JsonResult(count);
        }

        public IActionResult GetPktList()
        {
            var employeeID = HttpContext.Session.GetString("empID");
            var query = from pkt in _context.managepkt
                        where (pkt.CreatedBy == employeeID && pkt.StartDate < DateTime.Now) ||
                              ((pkt.StartDate == null || pkt.StartDate == null && pkt.CreatedBy == employeeID))
                        orderby pkt.Id descending select new { pkt.Name, pkt.Id };
            var result = query.ToList();
            return new JsonResult(result);
        }
        // GET: ManagePkts
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.Session.GetString("empName");
            var userId = HttpContext.Session.GetString("empID");
            var OH = HttpContext.Session.GetString("OH");
            var AH = HttpContext.Session.GetString("AH");
            var QH = HttpContext.Session.GetString("QH");
            var TH = HttpContext.Session.GetString("TH");

            if (userId == OH || userId == QH || userId == AH || userId == TH)
            {
                var empID = HttpContext.Session.GetString("empID");
                return View(await _context.managepkt.Where(p => p.CreatedBy == empID).OrderByDescending(p => p.Id).ToListAsync());
            }
            else
            {
                  TempData["error"] = "You are not authorised for this route !";
                  return RedirectToAction("Index", "Home");
            }
        }

        // POST: ManagePkts/Manage/
        [HttpPost]
        
        public async Task<IActionResult> EditManagePkt(Models.Entites.ManagePkt managePkt)
        {

            
               if (managePkt != null)
              {

                if(managePkt.NoQuestion == 0)
                {
                    return Json(new { message = $"Please First Assign Question To This {managePkt.Name}!", error = true, StatusCode = 302 });
                }

                var manage = _context.managepkt.FirstOrDefault(e => e.Id == managePkt.Id);
                var employeeID = HttpContext.Session.GetString("empID");
                if (manage != null)
                {
                  
                    DateTime endDate = DateTime.Parse(Convert.ToString(managePkt.EndDate));
                    DateTime outputDateTime = endDate.AddHours(12);
                    DateTime endoutputendDate = Convert.ToDateTime(outputDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

					DateTime startDate = DateTime.Parse(Convert.ToString(managePkt.StartDate));
					DateTime statrDateTime = startDate.AddHours(12);
					DateTime startDateTime = Convert.ToDateTime(statrDateTime.ToString("yyyy-MM-dd HH:mm:ss"));

					manage.Name = managePkt.Name;
                    manage.Process = managePkt.Process;
                    manage.StartDate = startDateTime;
                    manage.EndDate = endoutputendDate;
                    manage.PKTForText = managePkt.PKTForText;
                    manage.TestDuration = managePkt.TestDuration;
                    manage.NoQuestion = managePkt.NoQuestion;
                    manage.UpdatesBy = employeeID;
                    manage.UpdatesOn = DateTime.Now;
                    _context.SaveChanges();
                    return Json(new { message = "PKT Manage Updated Successfully !", success = true, StatusCode = 200 });
				}

  
                }

            return Json(new { message = "PKT Manage Updated Failed !", error = true, StatusCode = 302 });
        }


        

        // GET: ManagePkts/Create
        public async Task<IActionResult>  Create()
        {
            var userName = HttpContext.Session.GetString("empName");
            var userId = HttpContext.Session.GetString("empID");
            var OH = HttpContext.Session.GetString("OH");
            var AH = HttpContext.Session.GetString("AH");
            var QH = HttpContext.Session.GetString("QH");
            var TH = HttpContext.Session.GetString("TH");

            if (userId == OH || userId == QH || userId == AH || userId == TH)
            {
                var empID = HttpContext.Session.GetString("empID");
                return View(await _context.managepkt.Where(p => p.CreatedBy == empID).OrderByDescending(p => p.Id).ToListAsync());
            }
            else
            {
                TempData["error"] = "You are not authorised for this route !";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ManagePkts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       /*[ValidateAntiForgeryToken]*/
        public async Task<IActionResult> Create(Models.ManagePkt managePkt)
        {
            if (_context.managepkt.Any(d => d.Name == managePkt.Name))
            {
                return Json(new { message = "Pkt Name Already Exist !",error = true, StatusCode =403});
            }
            else
            {
               
                string formattedDdateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var designation = "All";
                Models.Entites.ManagePkt manage = new Models.Entites.ManagePkt();
                manage.Cm_Id = Convert.ToInt32(managePkt.Cm_Id);
                manage.Process = managePkt.Process;
                manage.Name = managePkt.Name;
                manage.StartDate = null;
                manage.EndDate = null;
                manage.TestDuration = Convert.ToInt32(managePkt.TestDuration);
                manage.PKTForText = managePkt.PKTForText;
                manage.CreatedBy = HttpContext.Session.GetString("empID");
                manage.Designation = designation;
                manage.CreatedOn = DateTime.Now;
                 _context.Add(manage);
                await _context.SaveChangesAsync();
            
                return Json(new { message = "Pkt Details Saved Successfully !", success = true, StatusCode = 200 });
            }
            return Json(new { message = "Pkt Details Saved Failed !", error = true, StatusCode = 302 });
        }


        [HttpGet]
        public async Task<IActionResult> Assign()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Assign(PktFor pkt, IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (file != null && file.Length > 0)
            {
                var uploadFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        do
                        {
                            bool isHeaderRowSkipped = false;

                            while (reader.Read())
                            {
                                if (!isHeaderRowSkipped)
                                {
                                    isHeaderRowSkipped = true;
                                    continue;
                                }
                                PktFor manage = new PktFor();
                                manage.PktName = pkt.PktName;
                                manage.UserId = reader.GetValue(0).ToString();
                                manage.CreatedBy = HttpContext.Session.GetString("empID");
                                if (!_context.pktFors.Any(d => d.UserId == reader.GetValue(0).ToString() && d.PktName == pkt.PktName))
                                {
                                    _context.pktFors.Add(manage);
                                    _context.SaveChanges();
                                }
                                else { continue; }
                            }
                        } while (reader.NextResult());
                    }
                }
                TempData["Success"] = "PKT Assigned To User Successfully";
                return View();
            }
            TempData["Success"] = "PKT Assigned To User Failed";
            return View();
        }


        // GET: ManagePkts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var managePkt = await _context.managepkt
                .FirstOrDefaultAsync(m => m.Id == id);
                    _context.managepkt.Remove(managePkt);
                await _context.SaveChangesAsync();
                return Json(new { message = "Pkt Details Deleted Successfully !", success = true, StatusCode = 200 });
            }
            catch(Exception ex)
            {
                return Json(new { message = ex.Message, error = true, StatusCode = 302 });
            }

            
        }

        
        private bool ManagePktExists(int id)
        {
            return _context.managepkt.Any(e => e.Id == id);
        }
    }
}
