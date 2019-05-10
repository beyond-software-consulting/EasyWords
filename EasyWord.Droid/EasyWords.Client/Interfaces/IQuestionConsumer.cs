using System;
using System.Threading.Tasks;
using EasyWords.Client.Models;

namespace EasyWords.Client.Interfaces
{
    public interface IQuestionConsumer
    {
        Task<QuestionBinding> GetQuestion(int userId, int userClientId, int dictionaryId, int questionTypeId);

        Task<AnswerResult> SendAnswer(AnswerModel answer);
    }
}
