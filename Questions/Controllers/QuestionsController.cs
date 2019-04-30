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
        public ActionResult<QuestionBinding> GetQuestion(int UserClientID,int UserID,int DictionaryID,int QuestionTypeID)
        {
            return  _manager.GetQuestion(UserID, UserClientID, DictionaryID,QuestionTypeID);


        }




        [HttpPost("SaveAnswer")]
        public ActionResult SaveAnswer([FromBody] Answer answer) {

            return _manager.SaveAnswer(answer);

        }




    }
}
