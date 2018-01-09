﻿using System;
using System.Linq;
using Business.ServicesInterfaces.Models.AnswerViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Domain.Entities;
using Data.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize]
    public class AnswersController : Controller
    {
        private readonly IAnswerRepository _repository;

        public AnswersController(IAnswerRepository repository)
        {
            _repository = repository;
        }

        // GET: Answers
        public IActionResult Index(Guid? questionId)
        {
            TempData["Id"] = questionId;

            return View(_repository.GetAllAnswersForGivenQuestion(questionId.Value));
        }
        
        // GET: Answers/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = _repository.GetAnswerById(id.Value);

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // GET: Answers/Create
        public IActionResult Create(Guid? id)
        {
            TempData["Id"] = id;

            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Guid? id, [Bind("UserId,QuestionId,Text")] AnswerCreateModel answerCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(answerCreateModel);
            }
            
            _repository.CreateAnswer(
                Answer.CreateAnswer(
                    answerCreateModel.UserId,
                    id.Value,
                    answerCreateModel.Text
                )
            );
            
           return RedirectToAction("Index","Questions");
        }
        
        // GET: Answers/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = _repository.GetAnswerById(id.Value);

            if (answer == null)
            {
                return NotFound();
            }

            var answerEditModel = new AnswerEditModel(
                answer.UserId,
                answer.QuestionId,
                answer.Text
            );

            return View(answerEditModel);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid? id, [Bind("UserId,QuestionId,AnswerDate,Text")] AnswerEditModel answerEditModel)
        {
            var answerToBeEdited = _repository.GetAnswerById(id.Value);

            if (answerToBeEdited == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(answerEditModel);
            }

            answerToBeEdited.UserId = answerEditModel.UserId;
            answerToBeEdited.QuestionId = answerEditModel.QuestionId;
            answerToBeEdited.AnswerDate = answerEditModel.AnswerDate;
            answerToBeEdited.Text = answerEditModel.Text;

            try
            {
                _repository.EditAnswer(answerToBeEdited);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists(_repository.GetAnswerById(id.Value).Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction("Index", "Questions");
            
        }

        // GET: Answers/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = _repository.GetAnswerById(id.Value);

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var answer = _repository.GetAnswerById(id);

            _repository.DeleteAnswer(answer);

            return RedirectToAction("Index", "Questions");
        }

        private bool AnswerExists(Guid id)
        {
            return _repository.GetAllAnswers().Any(answer => answer.Id == id);
        }

        // GET: Answers/Create
        public IActionResult CreateNew(Guid? qid, Guid? uid, String text)
        {
            TempData["QId"] = qid;
            TempData["UId"] = uid;
            TempData["QText"] = text;

            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNew(Guid? qid, Guid? uid, String text, [Bind("UserId,QuestionId,Text")] AnswerCreateModel answerCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(answerCreateModel);
            }

            _repository.CreateAnswer(
                Answer.CreateAnswer(
                    uid.Value,
                    qid.Value,
                    text
                )
            );

            return RedirectToAction("Index", "Questions");
        }

        [HttpPost]
        public IActionResult CreateNewAnswer(Guid? uid, Guid? qid, string qtext)
        {
            if (qtext != null)
            {
                _repository.CreateAnswer(
                    Answer.CreateAnswer(
                        uid.Value,
                        qid.Value,
                        qtext
                    )
                );
            }

            return RedirectToAction("Index", "Questions");
        }
    }
}
