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

namespace Questions.Managers
{
    public class QuestionManager : IQuestionManager
    {
            

        ICacheProvider CacheProvider { get; set; }
        IDatabaseRepository<Question> QuestionDatabaseProvider { get; set; }
        IDatabaseRepository<Pair> PairDatabaseProvider { get; set; }
        IDatabaseRepository<UserPairScore> UserPairScoreDatabaseProvider { get; set; }

        TelemetryClient telemetryClient = new TelemetryClient();


        public QuestionManager(IDatabaseRepository<Question> questionDatabaseProvider
            ,IDatabaseRepository<Pair> pairDatabaseProvider
            ,IDatabaseRepository<UserPairScore> userPairDatabaseProvider
            ,ICacheProvider cacheProvider)
        {
           
        
            CacheProvider = cacheProvider;
            QuestionDatabaseProvider = questionDatabaseProvider;
            PairDatabaseProvider = pairDatabaseProvider;
            UserPairScoreDatabaseProvider = userPairDatabaseProvider;

            telemetryClient.InstrumentationKey = "86f807bf-88e5-41b1-8bcd-ab8a6ba41909"; 
            RefreshCaches().Wait();
        }

        public  string AddQuestion(Question question)
        {
            throw new NotImplementedException();
        }


        public async Task<ActionResult<ApiModels.QuestionBinding>> GetQuestion(int userId, int userClientId, int dictionaryId, int questionTypeId)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>();
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            var totalTime = System.Diagnostics.Stopwatch.StartNew();
            

            metrics.Add("Initial Refresh Cache", stopWatch.ElapsedMilliseconds);
            stopWatch.Restart();
            var dictionary = CacheProvider.Pairs.Where(d => d.DictionaryId == dictionaryId).FirstOrDefault();
            if (dictionary == null)
                throw new NullReferenceException(string.Format("Dictionary not exist with DictionaryID:{0}", dictionaryId));

            if (questionTypeId < 1 || questionTypeId > 2)
                throw new ArgumentOutOfRangeException(string.Format("Invalid QuestionTypeID {0}", questionTypeId));

            if (CacheProvider.Pairs.Count < 5)
                throw new InvalidOperationException("Not enough pair for dictionary");
            metrics.Add("Validation Complete", stopWatch.ElapsedMilliseconds);
            stopWatch.Restart();
            var unansweredQuestion = GetUnansweredQuestion(userClientId, dictionaryId);
            metrics.Add("GetUnAnsweredQuestion Completed", stopWatch.ElapsedMilliseconds);
            stopWatch.Restart();
            var rankList = GetAllUserRanksByDictionaryId(userId, dictionaryId);

            var userRank = rankList.FirstOrDefault(r => r.UserId == userId);
            metrics.Add("Calculate Rank Completed", stopWatch.ElapsedMilliseconds);
            stopWatch.Restart();
            if (unansweredQuestion != null)
            {
                metrics.Add("UnAnsweredQuestion InProgress", 0);
                stopWatch.Restart();
                var unansweredPair = CacheProvider.Pairs.Where(p => p.Id == unansweredQuestion.PairID).FirstOrDefault();
                var targetSoundEx = unansweredPair.SoundEx1;
                if (unansweredQuestion.QuestionWordNumber == 2)
                    targetSoundEx = unansweredPair.SoundEx2;
                var wrongPairs = GetWrongPairs(targetSoundEx, unansweredQuestion.QuestionWordNumber, dictionaryId,unansweredPair.Id);
                metrics.Add("GetWrongPairs Completed", stopWatch.ElapsedMilliseconds);
                stopWatch.Restart();
                metrics.Add("Total Executing time", totalTime.ElapsedMilliseconds);
                telemetryClient.TrackEvent("GetQuestions", null, metrics);
                return ModelFactory.ToQuestionBinding(unansweredQuestion, unansweredPair, wrongPairs, userRank); 
            }

            var communityScore = GetCommunityScore(userId, dictionaryId);
            metrics.Add("CommunityScore Completed", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();
            var unbindPair = GenerateUnpairQuestion(communityScore,dictionaryId);
            metrics.Add("GenerateUnpair Completed", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();
            int numberOfWord = Convert.ToInt32(Math.Floor(new Random().NextDouble() * (3 - 1) + 1));

            var question = AddNewQuestionWithUnbindPair(unbindPair, userClientId, numberOfWord);

            RefreshCaches(true);

            var wrongPairList = GetWrongPairs(unbindPair.SoundEx1, numberOfWord, dictionaryId, unbindPair.Id);
            var questionBind = ModelFactory.ToQuestionBinding(question, unbindPair, wrongPairList, userRank);
            metrics.Add("Total Executing time", totalTime.ElapsedMilliseconds);

            telemetryClient.TrackEvent("GetQuestions", null, metrics);

            return questionBind;
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

        public async Task<ActionResult> SaveAnswer(ApiModels.Answer answer)
        {
            if (string.IsNullOrEmpty(answer.AnswerText))
                throw new ArgumentNullException("Answer text is null");

            var question = CacheProvider.Questions.Where(q => q.Id == answer.QuestionID).FirstOrDefault();

            var result = CheckAnswer(question,answer);

            UpdateUserPairScoreRepository(question, true, answer.UserId, answer.DictionaryId);

            if (result)
            {
                UpdateQuestionRepository(question, answer);
            }

            RefreshCaches(true);

            if (result)
                return new OkResult();
            else
                return new BadRequestResult();

            
        }

        #region Private Methods

        private IList<UserRank> GetAllUserRanksByDictionaryId(int dictionaryId,int? userId = null)
        {

            var userPairScores = CacheProvider.UserScores;
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
            var userPairScore = CacheProvider.UserScores.Where(q => q.PairId == question.PairID).FirstOrDefault();
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
        private async Task RefreshCaches(bool ForceRefresh=false)
        {

            if (CacheProvider.Pairs == null || ForceRefresh)
                CacheProvider.Pairs = PairDatabaseProvider.GetAll();
            if (CacheProvider.Questions == null || ForceRefresh)
                CacheProvider.Questions = QuestionDatabaseProvider.GetAll();
            if (CacheProvider.UserScores == null || ForceRefresh) CacheProvider.UserScores = UserPairScoreDatabaseProvider.GetAll();
        }


        private bool CheckAnswer(Question question,ApiModels.Answer answer)
        {
            var pair = PairDatabaseProvider.GetById(question.PairID);
            return ((question.QuestionWordNumber == 1 ? pair.InLanguage1 : pair.InLanguage2) == answer.AnswerText);                

        }
        private Question GetUnansweredQuestion(int userClientId,int dictionaryId)
        {
            if (CacheProvider.Questions == null) return null;
            var userClientQuestions = (from q in CacheProvider.Questions where q.DateOfAnswer == null && q.UserClientID == userClientId select q);
            var dictionaryPairs = (from p in CacheProvider.Pairs where p.DictionaryId == dictionaryId select p);

            var result = (from q in userClientQuestions
                           join p in dictionaryPairs on q.PairID equals p.Id
                           select q).FirstOrDefault();

            return result;


        }
        private IList<CommunityScore> GetCommunityScore(int userId,int dictionaryId)
        {

            var results = (from s in CacheProvider.UserScores
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

            return results;
        }


        private Pair GenerateUnpairQuestion(IList<CommunityScore> communityScores,int dictionaryId)
        {
            var pairs = GetMinimizedSetOfPair(communityScores,dictionaryId);

            var pair = GetBestQuestinPairByScore(pairs);

            return pair; 
        }

        private IList<UserPairScoreNormalization> GetMinimizedSetOfPair(IList<CommunityScore> communityScores,int dictionaryId)
        {
            var unbindedPairs = (from p in CacheProvider.Pairs
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

            return CacheProvider.Pairs.Where(p => p.Id == unbindedPair.PairID).FirstOrDefault(); 
        }


        private int NormalizeUserScore(int CorrectCount, int WrongCount)
        {
            var calculationResult = CorrectCount - WrongCount;

            return calculationResult >= 0 && calculationResult < 3 ? 0 : calculationResult;
        }


        private IList<Pair> GetWrongPairs(string soundEx,int questionWordNumber,int dictionaryId,int originalPairId)
        {
            if(questionWordNumber == 1) 
            {
                return GetWrongPairsBySoundEx1(soundEx, dictionaryId, originalPairId);
            }
            else
            {
                return GetWrongPairsBySoundEx2(soundEx, dictionaryId, originalPairId);
            }

        }
        public IList<Pair> GetWrongPairsBySoundEx1(string soundEx,int dictionaryId, int originalPairId)
        {
            var letter = soundEx[0].ToString();
            var soundExNumber = int.Parse(soundEx.Substring(1));
            var result = (from p in CacheProvider.Pairs
                          where
                          p.DictionaryId == dictionaryId
                          && p.Id != originalPairId
                          && p.SoundEx1[0].ToString() == letter
                          && Math.Abs(int.Parse(p.SoundEx1.Substring(1)) - soundExNumber) < 40
                          select p)
                             .OrderByDescending(x => Math.Abs(int.Parse(x.SoundEx1.Substring(1)) - soundExNumber))
                             .Take(5).ToList();
            return result;
        }

        public IList<Pair> GetWrongPairsBySoundEx2(string soundEx, int dictionaryId, int originalPairId)
        {
            var letter = soundEx[0].ToString();
            var soundExNumber = int.Parse(soundEx.Substring(1));
            var result = (from p in CacheProvider.Pairs
                          where
                          p.DictionaryId == dictionaryId
                          && p.Id != originalPairId
                          && p.SoundEx2[0].ToString() == letter
                          && Math.Abs(int.Parse(p.SoundEx1.Substring(1)) - soundExNumber) < 40
                          select p)
                             .OrderByDescending(x => Math.Abs(int.Parse(x.SoundEx2.Substring(1)) - soundExNumber))
                             .Take(5).ToList();
            return result;
        }
        public async Task<ActionResult> GenerateTestData()
        {

             var pairs = CacheProvider.Pairs.Take(20000);
            IList<Question> questions = new List<Question>();
            foreach (var pair in pairs)
            {
                int numberOfWord = Convert.ToInt32(Math.Floor(new Random().NextDouble() * (3 - 1) + 1));


                Models.Question question = new Question()
                {
                    PairID = pair.Id,
                    QuestionWordNumber = numberOfWord,
                    DateOfCreation = DateTime.UtcNow,
                    UserClientID = 1,
                    Type = 1,
                    DateOfAnswer = DateTime.UtcNow
                };
                questions.Add(question);
            }
            QuestionDatabaseProvider.AddRange(questions);
            return new OkResult();
        }
        #endregion
    }
}
