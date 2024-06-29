using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pkt.Models;
using System.Diagnostics;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Http;
using Pkt.Data;
using Microsoft.EntityFrameworkCore;
using Pkt.Models.Entites;
using Pkt.Helper;
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;

namespace Pkt.Controllers
{
   public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {

            var NoQuestion = _context.commonquestions.Where(d => d.CreatedBY == HttpContext.Session.GetString("empID")).Count();
            var NoQuiz = _context.managepkt.Where(d => d.CreatedBy == HttpContext.Session.GetString("empID")).Count();
            var Percentage = _context.attemptPkts.Where(d => d.Cm_Id == Convert.ToInt32(HttpContext.Session.GetString("CMID")) && d.Status == 1).Count();
            var CompleteQuiz = _context.attemptPkts.Where(d => d.EmployeeId == HttpContext.Session.GetString("empID") && d.Status == 1).Count();
            
            ViewData["NoQuestion"] = NoQuestion;
            ViewData["NoQuiz"] = NoQuiz;
            ViewData["Percentage"] = Percentage;
            ViewData["CompleteQuiz"] = CompleteQuiz;
           // ViewData["average"] = employees;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Login()
        {
            var empName = HttpContext.Session.GetString("empName");
            var empID = HttpContext.Session.GetString("empID");
            if (empName != null && empID != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var empid = HttpContext.Session.GetString("empID");
            var row = _context.auths.Where(d => d.EmpID == empid).FirstOrDefault();
            row.userLogin = 0;
            _context.SaveChanges();
            HttpContext.Session.Clear();
            return View("Login");
        }


        [HttpPost]
        public IActionResult Login(string EmployeeId , string Password)
        {

            if (string.IsNullOrEmpty(EmployeeId) && string.IsNullOrEmpty(Password))
            {
                TempData["Error"] = "Please Enter Login Crendential";
                return View();
            }

            if (_context.auths.Any(d => d.EmpID == EmployeeId))
            {
                var row = _context.auths.Where(d => d.EmpID == EmployeeId).FirstOrDefault();
                bool passwordsMatch = PasswordHelper.VerifyPassword(Password, row.Password, row.PasswordKey);
                if (passwordsMatch)
                {


                    HttpContext.Session.SetString("empName", row.Name);
                    HttpContext.Session.SetString("empID", row.EmpID);
                    HttpContext.Session.SetString("OH", row.OH);
                    HttpContext.Session.SetString("AH", row.AH);
                    HttpContext.Session.SetString("TH", row.TH);
                    HttpContext.Session.SetString("QH", row.QH);
                    HttpContext.Session.SetString("CMID", Convert.ToString(row.CMID));
                    HttpContext.Session.SetString("Department", Convert.ToString(row.Department));
                    HttpContext.Session.SetString("Process", Convert.ToString(row.Process));
                    HttpContext.Session.SetString("Client_Name", Convert.ToString(row.ClientID));
                    HttpContext.Session.SetString("ClientName", Convert.ToString(row.ClientName));
                   //HttpContext.Session.SetString("DOJ", Convert.ToString(row.DOJ));
                    HttpContext.Session.SetString("Gender", Convert.ToString(row.Gender));
                    HttpContext.Session.SetString("Designation", Convert.ToString(row.Designation));
                    HttpContext.Session.SetString("Status", Convert.ToString(row.Status));
                    HttpContext.Session.SetString("DesignationID", Convert.ToString(row.DesignationID));
                    HttpContext.Session.SetString("BatchID", Convert.ToString(row.BatchID));
                    HttpContext.Session.Set("userLogin", System.Text.UTF8Encoding.UTF8.GetBytes(row.EmpID));
                    row.userLogin = 1;
                    _context.SaveChanges();
                    TempData["Success"] = "You are successfully logged in";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Invalid Password or Employee Record Does Not Exists !";
                    return View();
                }

            }

            else
            {
                TempData["Error"] = "Invalid Crendential or Employee Record Does Not Exists !";
                return View();
            }
            return View();
       
        }

        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var list  = _context.auths.ToList();
            return View(list);
        }

        [HttpPost]
      /*  [ValidateAntiForgeryToken]*/
        public async Task<IActionResult> SaveUser(Auth model)
        {

            if(_context.auths.Any(d => d.Email == model.Email))
            {
                return Json(new { message = "User Email ID Already Exists !", status = 320, error = true });
            }


            if (!_context.auths.Any(d => d.EmpID == model.EmpID)) 
            {
                string password = model.Password;
                var (hashedPassword, salt) = PasswordHelper.HashPassword(password);
                Auth auth = new Auth();
                auth.Password = hashedPassword;
                auth.PasswordKey = salt;
                auth.Phone = model.Phone;
                auth.EmpID = model.EmpID;
                auth.Email = model.Email;
                auth.CMID = model.CMID;
                auth.Name = model.Name;
                auth.ClientID = model.ClientID;
                auth.ClientName = model.ClientName;
                auth.QH = model.QH == "Yes" ? model.EmpID : model.QH;
                auth.AH = model.AH == "Yes" ? model.EmpID : model.AH;
                auth.TH = model.TH == "Yes" ? model.EmpID : model.TH;
                auth.OH = model.OH == "Yes" ? model.EmpID : model.OH;
                auth.Department = model.Department;
         
                auth.Gender = model.Gender;
                
                auth.Process = model.Process;
                auth.CreatedBy = HttpContext.Session.GetString("empID");
                auth.CreatedOn = DateTime.Now;
                 _context.Add(auth);
                _context.SaveChanges();
               return Json(new {message = "User Created Successfully !", status = 200 , success=true });
            }
            else
            {
                return Json(new { message = "User Employee ID Already Exists !", status = 320, error = true });
            }
            return Json(new { message = "Something Went Wrong !", status = 320, error = true });

        }


        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var row = _context.auths.Find(id); 
            return new JsonResult(row);
        }


        [HttpPost]
        /*  [ValidateAntiForgeryToken]*/
        public async Task<IActionResult> UpdateUser(Auth model)
        {
            
            if (!_context.auths.Any(d => d.Email == model.Email && d.EmpID == model.EmpID))
            {
                return Json(new { message = "User Email ID Already Exists !", status = 320, error = true });
            }


            if (_context.auths.Any(d => d.EmpID == model.EmpID && d.Id == model.Id))
            {
                var auth = _context.auths.FirstOrDefault(d => d.Id == model.Id);
                string password = model.Password;
                if(auth != null)
                {
                    if(password != null && password != "")
                    {
                        var (hashedPassword, salt) = PasswordHelper.HashPassword(password);
                        auth.Password = hashedPassword;
                        auth.PasswordKey = salt;
                    }

                    auth.Phone = model.Phone;
                    auth.EmpID = model.EmpID;
                    auth.Email = model.Email;
                    auth.CMID = model.CMID;
                    auth.Name = model.Name;
                    auth.ClientID = model.ClientID;
                    auth.ClientName = model.ClientName;
                    auth.QH = model.QH == "Yes" ? model.EmpID : model.QH;
                    auth.AH = model.AH == "Yes" ? model.EmpID : model.AH;
                    auth.TH = model.TH == "Yes" ? model.EmpID : model.TH;
                    auth.OH = model.OH == "Yes" ? model.EmpID : model.OH;
                    auth.Department = model.Department;
                  
                    auth.Gender = model.Gender;
                   
                    auth.Process = model.Process;
                    auth.CreatedBy = HttpContext.Session.GetString("empID");
                    auth.CreatedOn = DateTime.Now;
                    _context.SaveChanges();
                }
                

                return Json(new { message = "User Updated Successfully !", status = 200, success = true });
                //TempData["success"] = "User Created Successfully !";
            }
            else
            {
                return Json(new { message = "User Employee ID Already Exists !", status = 320, error = true });
                //TempData["error"] = "User Already Exists !";
            }
            return Json(new { message = "Something Went Wrong !", status = 320, error = true });

        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id != 0)
            {
                var row = await _context.auths.FirstOrDefaultAsync(x => x.Id == id);
                if (row != null)
                {
                    _context.auths.Remove(row);
                    await _context.SaveChangesAsync();
                    return Json(new { message = "User Deleted Successfully !", success = true, status = 200 });
                }
                return Json(new { message = " User Deleted Failed !", error = true, status = 302 });
            }
            return Json(new { message =" Something Went Wrong !", error=true, status = 200 });
        }
    

    }
}
