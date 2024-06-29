using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pkt.Data;
using Pkt.Models.Entites;

namespace Pkt.Controllers
{
    public class CommonQuestionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CommonQuestionsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: CommonQuestions
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
                return View(await _context.commonquestions.ToListAsync());
            }
            else
            {
                TempData["success"] = "You are not authorised for this route !";
                return RedirectToAction("Index","Home");
            }

        }

        
        // GET: CommonQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var commonQuestion = await _context.commonquestions.FindAsync(id);
            if (commonQuestion != null)
            {
                _context.commonquestions.Remove(commonQuestion);
            }
             await _context.SaveChangesAsync();
            return Json(new { message = "Question Deleted Successfully !", success = true, StatusCode = 200 });
        }

       

        private bool CommonQuestionExists(int id)
        {
            return _context.commonquestions.Any(e => e.Id == id);
        }


        // POST: CommonQuestions/UploadExcel
         [HttpPost]
         
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
           
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (file != null && file.Length > 0)
            {
                var uploadFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";
                if(!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                
                var  filePath = Path.Combine(uploadFolder, file.FileName);
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
                                CommonQuestion commonQuestion = new CommonQuestion();
                                commonQuestion.Question = reader.GetValue(0).ToString();
                                commonQuestion.Option1 = reader.GetValue(1).ToString();
                                commonQuestion.Option2 = reader.GetValue(2).ToString();
                                commonQuestion.Option3 = reader.GetValue(3).ToString();
                                commonQuestion.Option4 = reader.GetValue(4).ToString();
                                commonQuestion.Answer = reader.GetValue(5).ToString();
                                commonQuestion.CreatedBY = HttpContext.Session.GetString("empID");
                                if(!_context.commonquestions.Any(d=> d.Question == reader.GetValue(0).ToString()))
                                {
                                    _context.commonquestions.Add(commonQuestion);
                                    _context.SaveChanges();
                                }
                                else { continue; }
                                
                             
                            }
                        } while (reader.NextResult());

                    }
                }


            }

            TempData["Success"] = "Excel Sheet Data Imported Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
