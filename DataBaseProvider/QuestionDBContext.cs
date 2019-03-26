using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataBaseProvider
{


    public class QuestionDBContext : DbContext
    {
        private DbSet<QuestionModel.Question> Question;
        public QuestionDBContext(DbContextOptions<QuestionDBContext> options)
        : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public Task AddAnswer(AnswerModel.Answer answer)
        {
            return null;
        }

    }
}
