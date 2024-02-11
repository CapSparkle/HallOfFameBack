
using HallOfFame.Controllers;
using HallOfFame.Models;
using HallOfFame.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HallOfFame.Test
{
    [TestFixture]
    public class PersonsControllerTests
    {
        private Mock<IPersonRepository> _mockRepo;
        private PersonsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IPersonRepository>();
            _controller = new PersonsController(_mockRepo.Object);
        }

        [TearDown]
        public void Teardown() { _controller.Dispose(); }   

        [Test]
        public async Task GetPersons()
        {
            var testPersons = GetTestPersons();
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(testPersons);

            var result = await _controller.GetPersons();

            var actionResult = result.Result as OkObjectResult;
            Assert.That(actionResult != null);
            var outputPersons = (actionResult.Value as IEnumerable<Person>).ToList();
            Assert.That(outputPersons != null);
            Assert.That(testPersons.Count == outputPersons.Count);

            for (int i = 0; i < testPersons.Count; i ++)
                Assert.That(testPersons[i] == outputPersons[i]);
        }

        [Test]
        public async Task GetPersonById()
        {
            long testId = 1;
            var testPerson = GetTestPersons()[0];
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId)).ReturnsAsync(testPerson);

            var result = await _controller.GetPerson(testId);

            var actionResult = result.Result as OkObjectResult;
            Assert.That(actionResult != null);
            var outputPerson = actionResult.Value as Person;
            Assert.That(outputPerson == testPerson);

        }

        [Test]
        public async Task GetPersonByWrongId()
        {
            //=== not found ===
            long testId = -1;
            Person testPerson = null;
            _mockRepo.Setup(repo => repo.GetByIdAsync(testId)).ReturnsAsync(testPerson);

            var notFoundResult = await _controller.GetPerson(testId);

            notFoundResult = notFoundResult.Result as NotFoundResult;
            Assert.That(notFoundResult != null);

        }

        [Test]
        public async Task PostPerson()
        {
            var testPerson = new Person { 
                Id = 1, 
                Name = "New Person", 
                DisplayName = "New", 
                Skills = new List<Skill>() 
            };

            _mockRepo.Setup(repo => repo.AddAsync(testPerson))
                .ReturnsAsync(testPerson)
                .Verifiable("AddAsync was called with incorrect data");

            var result = await _controller.PostPerson(testPerson);

            var actionResult = result.Result as CreatedAtActionResult;
            Assert.That(actionResult != null);
            var person = actionResult.Value as Person;
            Assert.That(person != null);

            _mockRepo.Verify(
                repo => repo.AddAsync(It.Is<Person>(
                    p => p.Name == testPerson.Name 
                    && p.DisplayName == testPerson.DisplayName
                    && p.Skills.Equals(testPerson.Skills))),
                Times.Once());
        }

        [Test]
        public async Task PostWrongPerson()
        {
            Person testPerson = null;

            var result = await _controller.PostPerson(testPerson);

            var actionResult = result.Result as BadRequestResult;
            Assert.That(actionResult != null);
        }

        [Test]
        public async Task PutPerson()
        {
            int personId = 1;

            var testPersonFromBody = new Person { 
                Id = -123, Name = "Name", 
                DisplayName = "DisplayName", 
                Skills = new List<Skill>() 
            };

            var updatedPerson = new Person
            {
                Id = personId,
                Name = "Name",
                DisplayName = "DisplayName",
                Skills = new List<Skill>()
            };

            _mockRepo.Setup(repo => repo.UpdateAsync(It.Is<Person>(
                    p => p.Name == testPersonFromBody.Name
                    && p.DisplayName == testPersonFromBody.DisplayName
                    && p.Id == personId
                    && p.Skills.Equals(testPersonFromBody.Skills))))
                .ReturnsAsync(updatedPerson)
                .Verifiable("UpdateAsync was called with incorrect data");

            var result = await _controller.PutPerson(personId, testPersonFromBody);
            var noContentResult = result as NoContentResult; 
            Assert.That(noContentResult != null);

            _mockRepo.Verify(
                repo => repo.UpdateAsync(It.Is<Person>(
                    p => p.Name == testPersonFromBody.Name
                    && p.DisplayName == testPersonFromBody.DisplayName
                    && p.Id == personId
                    && p.Skills.Equals(testPersonFromBody.Skills))),
                Times.Once());
        }

        [Test]
        public async Task PutPersonWrongId()
        {
            int personId = -1;

            var testPersonFromBody = new Person
            {
                Id = -123,
                Name = "Name",
                DisplayName = "DisplayName",
                Skills = new List<Skill>()
            };

            Person updatedPerson = null;

            _mockRepo.Setup(repo => repo.UpdateAsync(It.Is<Person>(p => p.Id == personId)))
                .ReturnsAsync(updatedPerson)
                .Verifiable("UpdateAsync was called with incorrect data");

            var result = await _controller.PutPerson(personId, testPersonFromBody);
            var notFoundResult = result as NotFoundResult;
            Assert.That(notFoundResult != null);

            _mockRepo.Verify(
                repo => repo.UpdateAsync(It.Is<Person>(p => p.Id == personId)),
                Times.Once());
        }

        [Test]
        public async Task DeletePersonWringId()
        {
            int personId = -1;
            _mockRepo.Setup(repo => repo.DeleteAsync(personId)).ReturnsAsync(false);

            var result = await _controller.DeletePerson(personId);

            var notFoundResult = result as NotFoundResult;
            Assert.That(notFoundResult != null);
        }

        [Test]
        public async Task DeletePerson()
        {
            int personId = 1;
            _mockRepo.Setup(repo => repo.DeleteAsync(personId)).ReturnsAsync(true);

            var result = await _controller.DeletePerson(personId);

            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult != null);
        }


        private List<Person> GetTestPersons()
        {
            return new List<Person>
            {
                new Person { Id = 1, Name = "John Doe", DisplayName = "John", Skills = new List<Skill>() },
                new Person { Id = 2, Name = "Jane Doe", DisplayName = "Jane", Skills = new List<Skill>() }
            };
        }
    }
}