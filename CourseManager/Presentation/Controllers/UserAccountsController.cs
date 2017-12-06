﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Domain.Entities;
using Data.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class UserAccountsController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserAccountRepository _repository;

        public UserAccountsController(IUserAccountRepository repository,IHostingEnvironment env)
        {
            _env = env;
            _repository = repository;
        }

        // GET: UserAccounts
        public IActionResult Index()
        {
            return View(_repository.GetAllUsers());
        }

        // GET: UserAccounts/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = _repository.GetUserById(id.Value);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // GET: UserAccounts/CreateStudent
        public IActionResult Createstudent()
        {
            return View();
        }

        // POST: UserAccounts/CreateStudent
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStudent([Bind("FirstName,LastName,RegistrationNumber,Group,Password,ConfirmPassword,Email")] UserAccountStudentCreateModel userAccountStudentCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userAccountStudentCreateModel);
            }

            _repository.CreateUser(
                UserAccount.CreateStudentAccount(
                    userAccountStudentCreateModel.FirstName,
                    userAccountStudentCreateModel.LastName,
                    userAccountStudentCreateModel.RegistrationNumber,
                    userAccountStudentCreateModel.Group,
                    userAccountStudentCreateModel.Password,
                    userAccountStudentCreateModel.Email
                )
            );

            return RedirectToAction(nameof(Index));
        }

        // GET: UserAccounts/CreateStudent
        public IActionResult CreateAssistant()
        {
            return View();
        }

        // POST: UserAccounts/CreateAssistant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAssistant([Bind("FirstName,LastName,RegistrationNumber,Group,Password,ConfirmPassword,Email")] UserAccountAssistantCreateModel userAccountAssistantCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userAccountAssistantCreateModel);
            }

            _repository.CreateUser(
                UserAccount.CreateAssistantAccount(
                    userAccountAssistantCreateModel.FirstName,
                    userAccountAssistantCreateModel.LastName,
                    userAccountAssistantCreateModel.Password,
                    userAccountAssistantCreateModel.Email
                )
            );

            return RedirectToAction(nameof(Index));
        }

        // GET: UserAccounts/EditStudent/5
        public IActionResult EditStudent(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = _repository.GetUserById(id.Value);
            if (userAccount == null)
            {
                return NotFound();
            }

            var studentEditModel = new UserAccountStudentEditModel(
                userAccount.FirstName,
                userAccount.LastName,
                userAccount.RegistrationNumber,
                userAccount.Group,
                userAccount.Email
            );

            return View(studentEditModel);
        }

        // POST: UserAccounts/EditStudent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStudent(Guid id, [Bind("FirstName,LastName,RegistrationNumber,Group,Email")] UserAccountStudentEditModel userAccountStudentEditModel)
        {
            var studentToBeEdited = _repository.GetUserById(id);

            if (studentToBeEdited == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(userAccountStudentEditModel);
            }

            studentToBeEdited.FirstName = userAccountStudentEditModel.FirstName;
            studentToBeEdited.LastName = userAccountStudentEditModel.LastName;
            studentToBeEdited.RegistrationNumber = userAccountStudentEditModel.RegistrationNumber;
            studentToBeEdited.Email = userAccountStudentEditModel.Email;
            studentToBeEdited.Group = userAccountStudentEditModel.Group;

            try
            {
                _repository.EditUser(studentToBeEdited);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAccountExists(_repository.GetUserById(id).Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: UserAccounts/EditAdmin/5
        public IActionResult EditAdmin(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = _repository.GetUserById(id.Value);
            if (userAccount == null)
            {
                return NotFound();
            }

            var adminEditModel = new UserAccountAdminEditModel(
                userAccount.FirstName,
                userAccount.LastName,
                userAccount.Email
            );

            return View(adminEditModel);
        }

        // POST: UserAccounts/EditAdmin/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAdmin(Guid id, [Bind("FirstName,LastName,Email")] UserAccountAdminEditModel userAccountAdminEditModel)
        {
            var adminToBeEdited = _repository.GetUserById(id);

            if (id != adminToBeEdited.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(userAccountAdminEditModel);
            }

            adminToBeEdited.FirstName = userAccountAdminEditModel.FirstName;
            adminToBeEdited.LastName = userAccountAdminEditModel.LastName;
            adminToBeEdited.Email = userAccountAdminEditModel.Email;

            try
            {
                _repository.EditUser(adminToBeEdited);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAccountExists(adminToBeEdited.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: UserAccounts/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = _repository.GetUserById(id.Value);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // POST: UserAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var userAccount = _repository.GetUserById(id);

            _repository.DeleteUser(userAccount);

            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(Guid id)
        {
            return _repository.GetAllUsers().Any(e => e.Id == id);
        }

        public IActionResult FileSubmissionPage()
        {
            return View();
        }


        public IActionResult KataSubmit()
        {
            return View();
        }

/*        public IActionResult SeminarSubmit()
        {
            return View();
        }*/
       /* protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (FileUploadControl.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(FileUploadControl.FileName);
                    FileUploadControl.SaveAs(Server.MapPath("~/") + filename);
                    StatusLabel.Text = "Upload status: File uploaded!";
                }
                catch (Exception ex)
                {
                    StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
                }*/

        [HttpPost]
        public async Task<IActionResult> Upload(FileViewModel model)
        {
            var file = model.File;
            if (file.Length > 0)
            {
                string path = Path.Combine(_env.WebRootPath, "Files");
                using (var fs = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
                model.Source = $"/Files{file.FileName}";
                model.Extension = Path.GetExtension(file.FileName).Substring(1);
                return View("~/Views/Home/Index.cshtml");
            }
            return BadRequest();
        }

            }
        }

