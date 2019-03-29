using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QuestionModel;
namespace DataBaseProvider
{
    public class QuestionDBContext 
    {

        public QuestionDBContext()        
        {

        }

        public void AddAnswer(AnswerModel.Answer item)
        {

              

        }

        public IList<QuestionModel.Question> GetQuestions() { return null; }

}
}
