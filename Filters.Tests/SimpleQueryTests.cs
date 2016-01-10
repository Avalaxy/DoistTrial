using System;
using System.Collections.Generic;
using Filters.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.Linq;

namespace Filters.Tests
{
    [TestClass]
    public class SimpleQueryTests
    {
        private static TaskFilter _taskFilter;
        private Fixture _fixture;

        private const int ItemsToGenerate = 10000;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _taskFilter = new TaskFilter();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void FilterItems_WithValidFutureDate_ReturnsFilteredItems()
        {
            // Arrange
            IEnumerable<Item> items = _fixture.CreateMany<Item>(ItemsToGenerate).ToArray();
            DateTime date = items.First().DueDate.Date;
            string query = date.ToString("yyyy-MM-dd");

            // Act
            Section section = _taskFilter.FilterItems(items, query);

            // Assert
            section.Title.Should().Be(date.ToString("MMM d"));
            section.Items.Should().OnlyContain(x => x.DueDate.Date == date);
            section.DueDate.Should().Be(date);
        }

        [TestMethod]
        public void FilterItems_WithValidPriority_ReturnsFilteredItems()
        {
            // Arrange
            var priority = _fixture.Create<Priority>();
            string query = $"p{priority}";
            IEnumerable<Item> items = _fixture.CreateMany<Item>(ItemsToGenerate).ToArray();

            // Act
            Section section = _taskFilter.FilterItems(items, query);

            // Assert
            section.Title.Should().Be($"Priority {priority}");
            section.Items.Should().OnlyContain(x => x.Priority == priority);
            section.Priority.Should().Be(priority);
        }

        [TestMethod]
        public void FilterItems_WithValidText_ReturnsFilteredItems()
        {
            // Arrange
            IEnumerable<Item> items = _fixture.CreateMany<Item>(ItemsToGenerate).ToArray();
            string text = items.First().Content.Substring(0, 5);
            string query = text;

            // Act
            Section section = _taskFilter.FilterItems(items, query);

            // Assert
            section.Title.Should().Be($"Search for: '{text}'");
            section.Items.Should().OnlyContain(x => x.Content.Contains(text));
        }
    }
}
