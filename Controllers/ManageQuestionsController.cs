using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pkt.Data;
using Pkt.Models.Entites;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CsvHelper;


namespace Pkt.Controllers
{
    public class ManageQuestionsController : Controller
    {
        private readonly AppDbContext _context;

        public ManageQuestionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ManageQuestions
        public async Task<IActionResult> Index()
        {
            /*return View(await _context.manageQuestions.ToListAsync());*/

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
                TempData["error"] = "You are not authorised for this route !";
                return RedirectToAction("Index", "Home");
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int[] Ques, ManageQuestion manageQuestion, IFormFile file)
        {

            /* [Bind("Id,PKTName,UploadType,Question,Option1,Option2,Option3,Option4,Answer,CreatedBy,UpdatedBy,CreatedOn,UpdatedOn")]*/
            var employeeID = HttpContext.Session.GetString("empID");
            if (manageQuestion.UploadType == "Manual")
                 {
                    ManageQuestion manual = new ManageQuestion();
                    manual.PKTName = manageQuestion.PKTName;
                    manual.UploadType = manageQuestion.UploadType;
                    manual.Question = manageQuestion.Question;
                    manual.Option1 = manageQuestion.Option1;
                    manual.Option2 = manageQuestion.Option2;
                    manual.Option3 = manageQuestion.Option3;
                    manual.Option4 = manageQuestion.Option4;
                    manual.Answer = manageQuestion.Answer;
                    manual.CreatedBy = employeeID;

                    if (!_context.manageQuestions.Any(d => d.Question == manageQuestion.Question && d.PKTName == manageQuestion.PKTName))
                    {
                        _context.manageQuestions.Add(manual);
                        _context.SaveChanges();
                        TempData["Success"] = "Data Saved Successfully !";
                        return RedirectToAction(nameof(Index));
                    }
                    else {
                        TempData["Error"] = $"Give Question Already Exist In Current PKT  - {manageQuestion.PKTName}";
                        return RedirectToAction(nameof(Index));
                    }

                }
                 if (manageQuestion.UploadType == "Upload")
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

                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension == ".csv")
                    {
                        using (var reader = new StreamReader(filePath))
                        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HasHeaderRecord = true, // Assuming the first row is a header
                            IgnoreBlankLines = true
                        }))
                        {
                            var records = csv.GetRecords<dynamic>().ToList();
                            foreach (var record in records)
                            {
                                var recordDict = (IDictionary<string, object>)record;
                                 // Assuming the CSV columns are named accordingly
                                var question = recordDict.ContainsKey(" Question") ? recordDict[" Question"].ToString() : string.Empty;
                                var option1 = recordDict.ContainsKey(" Option1") ? recordDict[" Option1"].ToString() : string.Empty;
                                var option2 = recordDict.ContainsKey(" Option2") ? recordDict[" Option2"].ToString() : string.Empty;
                                var option3 = recordDict.ContainsKey(" Option3") ? recordDict[" Option3"].ToString() : string.Empty;
                                var option4 = recordDict.ContainsKey(" Option4") ? recordDict[" Option4"].ToString() : string.Empty;
                                var answer = recordDict.ContainsKey(" Answer") ? recordDict[" Answer"].ToString() : string.Empty;
                                var manage = new ManageQuestion
                                {
                                    PKTName = manageQuestion.PKTName,
                                    UploadType = manageQuestion.UploadType,
                                    Question = question,
                                    Option1 = option1,
                                    Option2 = option2,
                                    Option3 = option3,
                                    Option4 = option4,
                                    Answer = answer,
                                    CreatedBy = employeeID
                                };

                                if (!_context.manageQuestions.Any(d => d.Question == question && d.PKTName == manageQuestion.PKTName))
                                {
                                    _context.manageQuestions.Add(manage);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                    }
                        
                    
                    if (fileExtension == ".xlsx")
                    {
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
                                        ManageQuestion manage = new ManageQuestion();
                                        manage.PKTName = manageQuestion.PKTName;
                                        manage.UploadType = manageQuestion.UploadType;
                                        manage.Question = reader.GetValue(0).ToString();
                                        manage.Option1 = reader.GetValue(1).ToString();
                                        manage.Option2 = reader.GetValue(2).ToString();
                                        manage.Option3 = reader.GetValue(3).ToString();
                                        manage.Option4 = reader.GetValue(4).ToString();
                                        manage.Answer = reader.GetValue(5).ToString();
                                        manage.CreatedBy = employeeID;
                                        if (!_context.manageQuestions.Any(d => d.Question == reader.GetValue(0).ToString() && d.PKTName == manageQuestion.PKTName))
                                        {
                                            _context.manageQuestions.Add(manage);
                                            _context.SaveChanges();
                                        }
                                        else { continue; }
                                    }
                                } while (reader.NextResult());
                            }
                        }
                    }

                    
                  }
                        TempData["Success"] = "Excel Sheet Data Imported Successfully";
                        return RedirectToAction(nameof(Index));

                  }
                 if (manageQuestion.UploadType == "Question Bank")
                    {

                            foreach (var value in Ques)
                            {
                                var row = _context.commonquestions.FirstOrDefault(d=>d.Id == value);
                                ManageQuestion manage = new ManageQuestion();
                                manage.PKTName = manageQuestion.PKTName;
                                manage.UploadType = manageQuestion.UploadType;
                                manage.Question = row.Question;
                                manage.Option1 = row.Option1;
                                manage.Option2 = row.Option2;
                                manage.Option3 = row.Option3;
                                manage.Option4 = row.Option4;
                                manage.Answer = row.Answer;
                                manage.CreatedBy = employeeID;
                                if (!_context.manageQuestions.Any(d => d.Question == row.Question && d.PKTName == manageQuestion.PKTName))
                                {
                                    _context.manageQuestions.Add(manage);
                                    _context.SaveChanges();
                                }
                                else { continue; }
                             }
                        TempData["Success"] = $"Question Bank Assined Successfully to {manageQuestion.PKTName}";
                        return RedirectToAction(nameof(Index));

                 }
                TempData["Error"] = "Something Went Wrong !";
                return RedirectToAction(nameof(Index));


        }

        // POST: ManageQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manageQuestion = await _context.manageQuestions.FindAsync(id);
            if (manageQuestion != null)
            {
                _context.manageQuestions.Remove(manageQuestion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManageQuestionExists(int id)
        {


            return _context.manageQuestions.Any(e => e.Id == id);
        }
    }
}
