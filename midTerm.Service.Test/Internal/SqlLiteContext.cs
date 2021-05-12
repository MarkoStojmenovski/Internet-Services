using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using midTerm.Data;
using midTerm.Data.Entities;

namespace midTerm.Service.Test.Internal
{
    public abstract class SqlLiteContext : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        protected readonly MidTermDbContext DbContext;

        protected SqlLiteContext(bool withData = false)
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            DbContext = new MidTermDbContext(CreateOptions());
            _connection.Open();
            DbContext.Database.EnsureCreated();
            if (withData)
                SeedData(DbContext);
        }

        protected DbContextOptions<MidTermDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<MidTermDbContext>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlite(_connection)
                .Options;
        }
        private void SeedData(MidTermDbContext dbContext)
        {
            var answers = new List<Answers>
                {
                    new Answers
                    {
                        Id = 1,
                        UserId = 1,
                        OptionId = 1
                    },
                    new Answers
                    {
                        Id = 1,
                        UserId = 2,
                        OptionId = 2
                    },
                    new Answers
                    {
                        Id = 1,
                        UserId = 3,
                        OptionId = 1
                    },
                };
            var option = new List<Option>
                {
                    new Option
                    {
                        Id = 1,
                        Text = "SomeOption1",
                        Order = 1,
                        QuestionId = 1
                    },
                    new Option
                    {
                        Id = 2,
                        Text = "SomeOption2",
                        Order = 2,
                        QuestionId = 1
                    },
                    new Option
                    {
                        Id = 3,
                        Text = "SomeOption3",
                        Order = 2,
                        QuestionId = 2
                    },
                    new Option
                    {
                        Id = 4,
                        Text = "SomeOption4",
                        Order = 1,
                        QuestionId = 2
                    },
                };
            var question = new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        Text = "A Question 1",
                        Description = "Something interesting",
                    },
                    new Question
                    {
                        Id = 2,
                        Text = "A Question 2",
                        Description = "Something interesting",
                    },
                    new Question
                    {
                        Id = 3,
                        Text = "A Question 3",
                        Description = "Something interesting",
                    },
                    new Question
                    {
                        Id = 4,
                        Text = "A Question 4",
                        Description = "Something interesting",
                    },
                    new Question
                    {
                        Id = 5,
                        Text = "A Question 5",
                        Description = "Something interesting",
                    },
                };
            var user = new List<SurveyUser>
                {
                    new SurveyUser
                    {
                        Id = 1,
                        FirstName = "Marko",
                        LastName = "Stojmenovski",
                        DoB = DateTime.Today.AddYears(-20),
                        Gender = (Data.Enums.Gender)1,
                        Country = "North Macedonia",
                    },
                    new SurveyUser
                    {
                        Id = 2,
                        FirstName = "John",
                        LastName = "Smith",
                        DoB = DateTime.Today.AddYears(-23),
                        Gender = (Data.Enums.Gender)1,
                        Country = "United States",
                    },
                    new SurveyUser
                    {
                        Id = 3,
                        FirstName = "Jane",
                        LastName = "Doe",
                        DoB = DateTime.Today.AddYears(-26),
                        Gender = (Data.Enums.Gender)2,
                        Country = "United Kingdom",
                    },
                };

            dbContext.AddRange(answers);
            dbContext.AddRange(option);
            dbContext.AddRange(question);
            dbContext.AddRange(user);
            dbContext.SaveChanges();
        }
        public void Dispose()
        {
            _connection.Close();
            _connection?.Dispose();
            DbContext?.Dispose();
        }
    }
}
