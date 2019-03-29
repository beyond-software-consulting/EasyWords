using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;

using Microsoft.AspNetCore.Mvc;
using QuestionManager;
using Questions.EventHandlers;

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


        [HttpGet("GetQuestion/{Dictionary}")]
        public ActionResult<QuestionModel.Question> GetQuestion(string Dictionary)
        {
            var nextQuestion = _manager.GetQuestion(Dictionary);
            _eventBus.Publish(new UserRankChangedIntegrationEvent(Guid.NewGuid(), 5.0f));
            return _manager.GetQuestion(Dictionary);
        }

        [HttpPost("SendAnswer")]
        public ActionResult SendAnswer() {

            return null;
        }




    }
}
