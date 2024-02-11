using AutoMapper;
using HallOfFame.Controllers;
using HallOfFame.Data;
using HallOfFame.Entities;
using HallOfFame.Mappings;
using HallOfFame.Models;
using HallOfFame.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallOfFameTest
{
    [TestFixture]
    internal class IntegrationTest1
    {
        AppDbContext _dbContext;
        PersonRepository _repository;
        IMapper _mapper;
        PersonsController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new AppDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _repository = new PersonRepository(_dbContext, _mapper);
            _controller = new PersonsController(_repository);
        }

        [Test]
        public async Task PostGetPerson()
        {
            var testPerson = new Person
            {
                Name = "Harper Lee",
                DisplayName = "Harper",
                Id = -18384847,
                Skills = new List<Skill>
                {
                    new Skill(){Level = 84, Name = "writing" },
                }
            };

            var result = await _controller.PostPerson(testPerson);
            var createdAtActionResult = result?.Result as CreatedAtActionResult;
            Assert.That(createdAtActionResult != null);
            Person resultValue = createdAtActionResult.Value as Person;
            Assert.That(
                resultValue.Name == testPerson.Name 
                && resultValue.DisplayName == testPerson.DisplayName
                && resultValue.Skills.Count == 1
                && resultValue.Skills[0].Level == testPerson.Skills[0].Level 
                && resultValue.Skills[0].Name == testPerson.Skills[0].Name
                );


            long assignedId = resultValue.Id;
            result = await _controller.GetPerson(assignedId);
            var okObjectResult = result?.Result as OkObjectResult;
            Assert.That(okObjectResult != null);
            resultValue = createdAtActionResult.Value as Person;
            Assert.That(
                resultValue.Name == testPerson.Name
                && resultValue.DisplayName == testPerson.DisplayName
                && resultValue.Skills.Count == 1
                && resultValue.Id == assignedId
                && resultValue.Skills[0].Level == testPerson.Skills[0].Level
                && resultValue.Skills[0].Name == testPerson.Skills[0].Name
                );
        }

        [Test]
        public async Task UpdatePerson()
        {
            var testPerson = new Person
            {
                Name = "Harper Lee",
                DisplayName = "Harper",
                Id = -18384847,
                Skills = new List<Skill>
                {
                    new Skill(){Level = 84, Name = "writing" },
                }
            };

            var updatedPerson = new Person
            {
                Name = "Ernest Heminguey",
                DisplayName = "Erny",
                Id = -567,
                Skills = new List<Skill>
                {
                    new Skill(){Level = 167, Name = "fishing" },
                }
            };

            var result = await _controller.PostPerson(testPerson);
            var createdAtActionResult = result?.Result as CreatedAtActionResult;
            Assert.That(createdAtActionResult != null);

            Person postResultValue = createdAtActionResult.Value as Person;

            var putResult = await _controller.PutPerson(postResultValue.Id, updatedPerson);
            var noContentResult = putResult as NoContentResult;
            Assert.That(noContentResult != null);

            var getResult = await _controller.GetPerson(postResultValue.Id);
            var okObjectResult = getResult?.Result as OkObjectResult;
            Assert.That(okObjectResult != null);

            Person getResultValue = okObjectResult.Value as Person;
            
            Assert.That(
                getResultValue.Name == updatedPerson.Name
                && getResultValue.DisplayName == updatedPerson.DisplayName
                && getResultValue.Skills.Count == 1
                && getResultValue.Id == postResultValue.Id
                && getResultValue.Skills[0].Level == updatedPerson.Skills[0].Level
                && getResultValue.Skills[0].Name == updatedPerson.Skills[0].Name
                );
        }

        [Test]
        public async Task DeletePerson()
        {
            var testPerson = new Person
            {
                Name = "Harper Lee",
                DisplayName = "Harper",
                Id = -18384847,
                Skills = new List<Skill>
                {
                    new Skill(){Level = 84, Name = "writing" },
                }
            };

            var result = await _controller.PostPerson(testPerson);
            var createdAtActionResult = result?.Result as CreatedAtActionResult;
            Assert.That(createdAtActionResult != null);

            Person postResultValue = createdAtActionResult.Value as Person;

            var putResult = await _controller.DeletePerson(postResultValue.Id);
            var noContentResult = putResult as NoContentResult;
            Assert.That(noContentResult != null);

            var getResult = await _controller.GetPerson(postResultValue.Id);
            var notFoundResult = getResult?.Result as NotFoundResult;
            Assert.That(notFoundResult != null);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();

            _controller.Dispose();
        }
    }
}
