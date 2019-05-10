using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Questions.Interfaces;
using Questions.Models;
using Questions.Providers;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Questions.Helpers;
using Questions.Adapters;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Caching.Memory;

namespace Questions.Managers
{
    public class QuestionManager : IQuestionManager
    {
            

        ICacheProvider CacheProvider { get; set; }
        IMemoryCache memoryCache { get; set; }
        IDatabaseRepository<Question> QuestionDatabaseProvider { get; set; }
        IDatabaseRepository<Pair> PairDatabaseProvider { get; set; }
        IDatabaseRepository<UserPairScore> UserPairScoreDatabaseProvider { get; set; }

        IList<Question> QuestionCache
        {
            get 
            {
                IList<Question> questions = new List<Question>();
                questions = memoryCache.Get<IList<Question>>(CacheKeys.Questions.ToString());
                return questions;

            }
            set 
            {
                memoryCache.Set<IList<Question>>(CacheKeys.Questions.ToString(), value);
            }
        }

        IList<Pair> PairCache
        {
            get
            {
                IList<Pair> pairs = new List<Pair>();
                pairs = memoryCache.Get<IList<Pair>>(CacheKeys.Pairs.ToString());
                return pairs;

            }
            set
            {
                memoryCache.Set<IList<Pair>>(CacheKeys.Pairs.ToString(), value);
            }
        }

        IList<UserPairScore> UserPairScoreCache
        {
            get
            {
                IList<UserPairScore> userPairScores = new List<UserPairScore>();
                userPairScores = memoryCache.Get<IList<UserPairScore>>(CacheKeys.UserScores.ToString());
                return userPairScores;

            }
            set
            {
                memoryCache.Set<IList<UserPairScore>>(CacheKeys.UserScores.ToString(), value);
            }
        }

        TelemetryClient telemetryClient = new TelemetryClient();


        public QuestionManager(IDatabaseRepository<Question> questionDatabaseProvider
            ,IDatabaseRepository<Pair> pairDatabaseProvider
            ,IDatabaseRepository<UserPairScore> userPairDatabaseProvider
            ,ICacheProvider cacheProvider
            ,IMemoryCache memoryCache)
        {
           
        
            CacheProvider = cacheProvider;
            QuestionDatabaseProvider = questionDatabaseProvider;
            PairDatabaseProvider = pairDatabaseProvider;
            UserPairScoreDatabaseProvider = userPairDatabaseProvider;
            this.memoryCache = memoryCache;

            telemetryClient.InstrumentationKey = "86f807bf-88e5-41b1-8bcd-ab8a6ba41909"; 
            RefreshCaches().Wait();
        }

        public async Task<ApiModels.QuestionBinding> GetQuestion(int userId, int userClientId, int dictionaryId, int questionTypeId)
        {


            //var dictionary = CacheProvider.Dictionaries.Where(d => d.Id == dictionaryId).FirstOrDefault();
            //if (dictionary == null)
                //throw new NullReferenceException(string.Format("Dictionary not exist with DictionaryID:{0}", dictionaryId));

            if (questionTypeId < 1 || questionTypeId > 2)
                throw new ArgumentOutOfRangeException(string.Format("Invalid QuestionTypeID {0}", questionTypeId));

            if (CacheProvider.Pairs.Count < 5)
                throw new InvalidOperationException("Not enough pair for dictionary");

            var unansweredQuestion = await GetUnansweredQuestion(userClientId, dictionaryId);



            var rankList = GetAllUserRanksByDictionaryId(userId, dictionaryId);

            var userRank = rankList.FirstOrDefault(r => r.UserId == userId);
            if (unansweredQuestion != null)
            {
                var unansweredPair = PairCache.Where(p => p.Id == unansweredQuestion.PairID).FirstOrDefault();
                var targetSoundEx = unansweredPair.SoundEx1;
                if (unansweredQuestion.QuestionWordNumber == 2)
                {
                    targetSoundEx = unansweredPair.SoundEx2;
                }

                var wrongPairs = await GetWrongPairs(targetSoundEx, unansweredQuestion.QuestionWordNumber, dictionaryId,unansweredPair.Id);
            
                return ModelFactory.ToQuestionBinding(unansweredQuestion, unansweredPair, wrongPairs, null); 
            }

            var communityScore = await GetCommunityScore(userId, dictionaryId);
            var unbindPair = await GenerateUnpairQuestion(communityScore,dictionaryId);
            int numberOfWord = Convert.ToInt32(Math.Floor(new Random().NextDouble() * (3 - 1) + 1));

            var question = AddNewQuestionWithUnbindPair(unbindPair, userClientId, numberOfWord);

            await RefreshCaches(true);

            var wrongPairList = await GetWrongPairs(unbindPair.SoundEx1, numberOfWord, dictionaryId, unbindPair.Id);
            var questionBind = ModelFactory.ToQuestionBinding(question, unbindPair, wrongPairList, null);

            return await Task.FromResult(questionBind);
        }

        public Question AddNewQuestionWithUnbindPair(Pair unbindPair,int userClientId,int numberOfWord)
        {


            Models.Question question = new Question()
            {
                PairID = unbindPair.Id,
                QuestionWordNumber = numberOfWord,
                DateOfCreation = DateTime.UtcNow,
                UserClientID = userClientId,
                Type = 1,
            };

            question = QuestionDatabaseProvider.Add(question);

            return question;
        }

        public async Task<ApiModels.AnswerResult> SaveAnswer(ApiModels.Answer answer)
        {
            if (string.IsNullOrEmpty(answer.AnswerText))
                throw new ArgumentNullException("Answer text is null");

            var question = QuestionCache.Where(q => q.Id == answer.QuestionID).FirstOrDefault();

            var result = CheckAnswer(question,answer);


            UpdateUserPairScoreRepository(question, true, answer.UserId, answer.DictionaryId);

            if (result)
            {
                await UpdateQuestionRepository(question, answer);
            }

            var answerResult = new ApiModels.AnswerResult() { IsCorrect = result };

            await RefreshCaches(true);

            return answerResult;

            
        }

        #region Private Methods

        private IList<UserRank> GetAllUserRanksByDictionaryId(int dictionaryId,int? userId = null)
        {

            var userPairScores = UserPairScoreCache;
            var list = (from up in userPairScores
                        where up.DictionaryId == dictionaryId
                        group up by new { up.UserId }
                        into temp
                        select new
                        {

                            temp.Key.UserId,
                            TotalWrongAnswer = (from s in temp select s.TotalWrong).Sum(),
                            TotalCorrectAnswer = (from s in temp select s.TotalCorrect).Sum(),
                            TotalAnswer = (from s in temp select s.TotalWrong + s.TotalCorrect).Sum()
                        }
             ).OrderByDescending(a => a.TotalCorrectAnswer)
             .Select((item, Rank) => new UserRank
             {
                 Rank = Rank,
                 UserId = item.UserId,
                 TotalAnswer = item.TotalAnswer,
                 TotalCorrectAnswer = item.TotalCorrectAnswer,
                 TotalWrongAnswer = item.TotalWrongAnswer
             }).ToList();
            return list;

        }
        
        private async Task<Question> UpdateQuestionRepository(Question question,ApiModels.Answer answer)
        {
            question.Answer = answer.AnswerText;
            question.DateOfAnswer = DateTime.UtcNow;
            question.WasAnswerCorrect = true;

            var result = QuestionDatabaseProvider.Update(question);
            return await Task.FromResult(result);
        }

        private void UpdateUserPairScoreRepository(Question question,bool wasAnswerIsCorrect,int userId,int dictionaryId)
        {
            var userPairScore = UserPairScoreCache.Where(q => q.PairId == question.PairID).FirstOrDefault();
            if(userPairScore==null)
            {
                userPairScore = new UserPairScore()
                {
                    UserId = userId,
                    DictionaryId = dictionaryId,
                    TotalCorrect = wasAnswerIsCorrect ? 1 : 0,
                    TotalWrong = wasAnswerIsCorrect ? 0 : 1
                };

                UserPairScoreDatabaseProvider.Add(userPairScore);
            }
            else
            {
                userPairScore.TotalCorrect += wasAnswerIsCorrect ? 1 : 0;
                userPairScore.TotalWrong += wasAnswerIsCorrect ? 0 : 1;
                UserPairScoreDatabaseProvider.Update(userPairScore); 
            }

        }
        private async Task<bool> RefreshCaches(bool ForceRefresh=false)
        {

            if (PairCache == null || ForceRefresh)
                PairCache = PairDatabaseProvider.GetAll();
            if (QuestionCache == null || ForceRefresh)
                QuestionCache = QuestionDatabaseProvider.GetAll();
            if (UserPairScoreCache == null || ForceRefresh) UserPairScoreCache = UserPairScoreDatabaseProvider.GetAll();


            return await Task.FromResult(true);

        }


        private bool CheckAnswer(Question question,ApiModels.Answer answer)
        {
            var pair = PairCache.Where(p => p.Id == question.PairID).FirstOrDefault();
            if(question.QuestionWordNumber==1)
            {

                return string.Compare(pair.InLanguage2, answer.AnswerText, true) == 0; 
            }
            else
            {
                return string.Compare(pair.InLanguage1, answer.AnswerText, true) == 0;
            }

        }

        private async Task<Question> GetUnansweredQuestion(int userClientId,int dictionaryId)
        {
            if (QuestionCache == null) return null;
            var userClientQuestions = (from q in QuestionCache where q.DateOfAnswer == null && q.UserClientID == userClientId select q);
            var dictionaryPairs = (from p in PairCache where p.DictionaryId == dictionaryId select p);

            var result = (from q in userClientQuestions
                           join p in dictionaryPairs on q.PairID equals p.Id
                           select q).FirstOrDefault();

            return await Task.FromResult(result);


        }
        private async Task<IList<CommunityScore>> GetCommunityScore(int userId,int dictionaryId)
        {

            var results = (from s in UserPairScoreCache
                           where s.UserId == userId && s.DictionaryId == dictionaryId
                           group s by new { s.PairId, s.DictionaryId }
                           into temp
                           select new CommunityScore()
                           {

                               DictionaryID = temp.Key.DictionaryId,
                               PairID = temp.Key.PairId,
                               TotalCorrect = (from s2 in temp select s2.TotalCorrect).Sum(),
                               TotalWrong = (from s2 in temp select s2.TotalWrong).Sum(),
                               TotalCommunityScore = (from s2 in temp select s2.TotalCorrect - s2.TotalWrong).Sum()
                           }
                           ).ToList();

            return await Task.FromResult(results);
        }


        private async Task<Pair> GenerateUnpairQuestion(IList<CommunityScore> communityScores,int dictionaryId)
        {
            var pairs = GetMinimizedSetOfPair(communityScores,dictionaryId);

            var pair = GetBestQuestinPairByScore(pairs);

            return await Task.FromResult(pair); 
        }

        private IList<UserPairScoreNormalization> GetMinimizedSetOfPair(IList<CommunityScore> communityScores,int dictionaryId)
        {
            var unbindedPairs = (from p in PairCache
                                 join c in communityScores on p.Id equals c.PairID into temp
                                 from c in temp.DefaultIfEmpty()
                                 where
                                 p.Active == 1 && p.DictionaryId == dictionaryId
                                 select new UserPairScoreNormalization()
                                 {
                                     PairID = p.Id,
                                     UserScoreNormalization = c == null ? 0 : NormalizeUserScore(c.TotalCorrect, c.TotalWrong),
                                     TotalCommunityScore = c == null ? 0 : c.TotalCommunityScore,
                                     RandomValue = new Random().NextDouble()

                                 }).OrderBy(a => Guid.NewGuid()).Take(10).ToList();
            return unbindedPairs;
        }
        private Pair GetBestQuestinPairByScore(IList<UserPairScoreNormalization> pairs)
        {
            var unbindedPair = pairs.OrderBy(p => p.UserScoreNormalization)
                                .ThenByDescending(p => p.TotalCommunityScore)
                                .ThenBy(p => p.RandomValue).FirstOrDefault();

            return PairCache.Where(p => p.Id == unbindedPair.PairID).FirstOrDefault(); 
        }


        private int NormalizeUserScore(int CorrectCount, int WrongCount)
        {
            var calculationResult = CorrectCount - WrongCount;

            return calculationResult >= 0 && calculationResult < 3 ? 0 : calculationResult;
        }


        private async  Task<IList<Pair>> GetWrongPairs(string soundEx,int questionWordNumber,int dictionaryId,int originalPairId)
        {
            if(questionWordNumber == 1) 
            {
                return await Task.FromResult(GetWrongPairsBySoundEx1(soundEx, dictionaryId, originalPairId));
            }
            else
            {
                return await Task.FromResult(GetWrongPairsBySoundEx2(soundEx, dictionaryId, originalPairId));
            }

        }
        public IList<Pair> GetWrongPairsBySoundEx1(string soundEx,int dictionaryId, int originalPairId)
        {
            var letter = soundEx[0].ToString();
            var soundExNumber = int.Parse(soundEx.Substring(1));
            var result = (from p in PairCache
                          where
                          (p.DictionaryId == dictionaryId
                          && p.SoundEx1[0].ToString() == letter
                          && Math.Abs(int.Parse(p.SoundEx1.Substring(1)) - soundExNumber) <= 48) || p.Id == originalPairId
                          select p)
                             .OrderByDescending(x => x.Id == originalPairId ? 9999 : Math.Abs(int.Parse(x.SoundEx1.Substring(1)) - soundExNumber))
                             .Take(5).ToList().OrderBy(xx => Guid.NewGuid()).ToList();
            return result;
        }

        public  IList<Pair> GetWrongPairsBySoundEx2(string soundEx, int dictionaryId, int originalPairId)
        {
            var letter = soundEx[0].ToString();
            var soundExNumber = int.Parse(soundEx.Substring(1));
            var result = (from p in PairCache
                          where
                          (p.DictionaryId == dictionaryId
                          && p.SoundEx2[0].ToString() == letter
                          && Math.Abs(int.Parse(p.SoundEx2.Substring(1)) - soundExNumber) <= 48) || p.Id == originalPairId
                          select p)
                             .OrderByDescending(x => x.Id == originalPairId ? 9999 : Math.Abs(int.Parse(x.SoundEx2.Substring(1)) - soundExNumber))
                             .Take(5).ToList().OrderBy(xx => Guid.NewGuid()).ToList();
            return result;
        }
        public async Task<IActionResult> GenerateTestData()
        {
            await Task.Run(() =>
            {
                var pairs = CacheProvider.Pairs.Take(20000);
               
                for (int i = 0; i < 10; i++)
                {
                    IList<Question> questions = new List<Question>();
                    var clientId = new Random().Next(50);
                    foreach (var pair in pairs)
                    {
                        int numberOfWord = Convert.ToInt32(Math.Floor(new Random().NextDouble() * (3 - 1) + 1));


                        Models.Question question = new Question()
                        {
                            PairID = pair.Id,
                            QuestionWordNumber = numberOfWord,
                            DateOfCreation = DateTime.UtcNow,
                            UserClientID = clientId,
                            Type = 1,
                            DateOfAnswer = DateTime.UtcNow
                        };
                        questions.Add(question);
                    }
                    QuestionDatabaseProvider.AddRange(questions);
                }

            });

            return await Task.FromResult(new OkResult());
        }
        #endregion
    }
}
