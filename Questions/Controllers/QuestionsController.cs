using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.AspNetCore.Mvc;
using Questions.ApiModels;
using Questions.Interfaces;
using Questions.Models;

namespace Questions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        IQuestionManager _manager;
        IEventBus _eventBus;
        public QuestionsController(IQuestionManager Manager,IEventBus eventBus)
        {
            _manager = Manager;
            _eventBus = eventBus;

        }
        // GET api/values


        [HttpGet("GetQuestion/{UserClientID}/{UserID}/{DictionaryID}/{QuestionTypeID}")]
        public async Task<ActionResult<QuestionBinding>> GetQuestion(int UserClientID,int UserID,int DictionaryID,int QuestionTypeID)
        {
            var value =  _manager.GetQuestion(UserID, UserClientID, DictionaryID,QuestionTypeID);
            return await value;

        }

        [HttpGet("GenerateTestData")]
        public async Task<ActionResult> CreateTestData()
        {
            return (await _manager.GenerateTestData()) as ActionResult;
        }


        [HttpPost("SaveAnswer")]
        public async Task<ActionResult<ApiModels.AnswerResult>> SaveAnswer([FromBody] ApiModels.Answer answer) {

            var value =  _manager.SaveAnswer(answer);

            return await value;
        }




    }
}
