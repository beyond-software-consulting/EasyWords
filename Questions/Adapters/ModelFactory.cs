using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Questions.Interfaces;
using Questions.Models;

namespace Questions.Adapters
{
    public class ModelFactory
    {
        public ModelFactory() { }

        public static ApiModels.QuestionBinding ToQuestionBinding(Question question,Pair pair
            , IList<Pair> wrongPairs, UserRank rank)
        {
            ApiModels.QuestionBinding binding = new ApiModels.QuestionBinding();
            binding.Question = new ApiModels.Question()
            {
                PairID = question.PairID,
                DictionaryID = pair.DictionaryId,
                QuestionID = question.Id,
                InLanguage1 = pair.InLanguage1,
                InLanguage2 = pair.InLanguage2,
                Conjugation = pair.Conjugation,
                SoundEx1 = pair.SoundEx1,
                SoundEx2 = pair.SoundEx2,
                QuestionWordNumber = question.QuestionWordNumber
            };

            binding.Rank = new ApiModels.UserRank()
            {
                Rank = rank == null ? 0 : rank.Rank,
                TotalWrong = rank == null ? 0 : rank.TotalWrongAnswer,
                TotalCorrect = rank == null ? 0 : rank.TotalCorrectAnswer
            };
            var wrongPairList = new List<ApiModels.QuestionPair>();
            foreach (var item in wrongPairs)
            {
                wrongPairList.Add(new ApiModels.QuestionPair()
                {
                    PairID = item.Id,
                    InLanguage1 = item.InLanguage1,
                    InLanguage2 = item.InLanguage2,
                    Conjugation = item.Conjugation,
                    SoundEx1 = item.SoundEx1,
                    SoundEx2 = item.SoundEx2
                });
            }
            binding.WrongPairs = wrongPairList;

            return binding; 
        } 
    }
}
