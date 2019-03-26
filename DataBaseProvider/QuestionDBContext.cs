using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QuestionModel;
namespace DataBaseProvider
{
    public class QuestionDBContext : DbContext
    {
        private DbSet<QuestionModel.Question> Questions { get; set; }
        private DbSet<AnswerModel.Answer> Answers { get; set; }
        public QuestionDBContext(DbContextOptions<QuestionDBContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public Task AddAnswer(AnswerModel.Answer item)
        {

             return this.SaveChangesAsync();

        }

        public async Task<IList<QuestionModel.Question>> GetDictionaries(string Language)
        {

            var data = await Questions.Where(q => q.Dictionary == Language).ToListAsync();

            return data;
        }

    }
}
