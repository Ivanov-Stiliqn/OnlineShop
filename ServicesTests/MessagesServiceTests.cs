using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Models;
using Moq;
using Services;
using Xunit;

namespace ServicesTests
{
    public class MessagesServiceTests
    {
        [Fact]
        public async Task AddMessagesShouldWork()
        {
            var messagesRepo = new Mock<IRepository<Message>>();
            var messages = new List<Message>
            {
                new Message(),
                new Message()
            };

            messagesRepo.Setup(r => r.All()).Returns(messages.AsQueryable);
            messagesRepo.Setup(r => r.AddAsync(It.IsAny<Message>())).Returns<Message>(Task.FromResult)
                .Callback<Message>(m => messages.Add(m));

            var service = new MessagesService(messagesRepo.Object);
            var newMessage = new Message(){Text = "test"};

            await service.AddMessage(newMessage);

            Assert.Equal(3, messages.Count);
            Assert.Contains(messages, m => m.Text == newMessage.Text);
            messagesRepo.Verify(r => r.AddAsync(newMessage), Times.Once);
            messagesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void GetUnredMessagesShouldReturnOnlyCurrentUserUnreadMessages()
        {
            var messagesRepo = new Mock<IRepository<Message>>();
            var messages = new List<Message>
            {
                new Message
                {
                    Text = "test 1",
                    Receiver = new User{UserName = "stamat"},
                    IsRead = false
                },
                new Message
                {
                    Text = "test 2",
                    Receiver = new User{UserName = "stamat"},
                    IsRead = false
                },
                new Message
                {
                    Text = "test 3",
                    Receiver = new User{UserName = "gosho"},
                    IsRead = false
                }

            };

            messagesRepo.Setup(r => r.All()).Returns(messages.AsQueryable);

            var service = new MessagesService(messagesRepo.Object);
            var unReadMessages = service.GetUnReadMessages("stamat");

            Assert.Equal(2, unReadMessages.Count);
            Assert.Contains(messages, m => m.Text == "test 1");
            Assert.Contains(messages, m => m.Text == "test 2");
            messagesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetUnredMessagesShouldReturnOnlyUnreadMessages()
        {
            var messagesRepo = new Mock<IRepository<Message>>();
            var messages = new List<Message>
            {
                new Message
                {
                    Text = "test 1",
                    Receiver = new User{UserName = "stamat"},
                    IsRead = true
                },
                new Message
                {
                    Text = "test 2",
                    Receiver = new User{UserName = "stamat"},
                    IsRead = false
                },
                new Message
                {
                    Text = "test 3",
                    Receiver = new User{UserName = "gosho"},
                    IsRead = false
                }

            };

            messagesRepo.Setup(r => r.All()).Returns(messages.AsQueryable);

            var service = new MessagesService(messagesRepo.Object);
            var unReadMessages = service.GetUnReadMessages("stamat");

            Assert.Equal(1, unReadMessages.Count);
            Assert.Contains(messages, m => m.Text == "test 2");
            messagesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task MarMessagesAsReadShouldWork()
        {
            var messagesRepo = new Mock<IRepository<Message>>();
            var messages = new List<Message>
            {
                new Message
                {
                    Text = "test 1",
                    Receiver = new User{UserName = "stamat"},
                    User = new User{UserName = "pesho"},
                    IsRead = false
                },
                new Message
                {
                    Text = "test 2",
                    Receiver = new User{UserName = "stamat"},
                    User = new User{UserName = "pesho"},
                    IsRead = false
                },
                new Message
                {
                    Text = "test 3",
                    Receiver = new User{UserName = "gosho"},
                    User = new User{UserName = "pesho"},
                    IsRead = false
                }

            };

            messagesRepo.Setup(r => r.All()).Returns(messages.AsQueryable);

            var service = new MessagesService(messagesRepo.Object);
            await service.MarkMessagesAsRead("stamat", "pesho");
            var unReadMessages = service.GetUnReadMessages("stamat");

            Assert.Equal(0, unReadMessages.Count);

            messagesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MarMessagesAsReadShouldSetOnlyForCurrentReceiverAndSender()
        {
            var messagesRepo = new Mock<IRepository<Message>>();
            var messages = new List<Message>
            {
                new Message
                {
                    Text = "test 1",
                    Receiver = new User{UserName = "stamat"},
                    User = new User{UserName = "pesho"},
                    IsRead = false
                },
                new Message
                {
                    Text = "test 2",
                    Receiver = new User{UserName = "stamat"},
                    User = new User{UserName = "pesho"},
                    IsRead = false
                },
                new Message
                {
                    Text = "test 3",
                    Receiver = new User{UserName = "stamat"},
                    User = new User{UserName = "gosho"},
                    IsRead = false
                }

            };

            messagesRepo.Setup(r => r.All()).Returns(messages.AsQueryable);

            var service = new MessagesService(messagesRepo.Object);
            await service.MarkMessagesAsRead("stamat", "pesho");
            var unReadMessages = service.GetUnReadMessages("stamat");

            Assert.Equal(1, unReadMessages.Count);
            Assert.Contains(unReadMessages, m => m.Text == "test 3");
            messagesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
