using System;
using ModelBase;

namespace AnswerModel
{
    public interface IAnswerModel:IModelBase
    {
        int QuestionID { get; set; }
        string AnswerText { get; set; }

    }
}
