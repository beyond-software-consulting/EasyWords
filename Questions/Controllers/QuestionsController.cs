using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuestionManager;

namespace Questions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        IQuestionManager _manager;

        public QuestionsController(IQuestionManager Manager)
        {
            _manager = Manager; 
        }
        // GET api/values


        [HttpGet("GetQuestion/{Dictionary}")]
        public ActionResult<QuestionModel.Question> GetQuestion(string Dictionary)
        {
            return _manager.GetQuestion(Dictionary);
        }





    }
}
