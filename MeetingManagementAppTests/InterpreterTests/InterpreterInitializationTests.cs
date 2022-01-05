using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeetingManagementApp;

namespace MeetingManagementAppTests.InterpreterTests
{
    [TestClass]
    public class InterpreterInitializationTests
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
            "Command service is null.")]
        public void NullUsedInConstructor_ThrowsArgumentNullException()
        {
            Interpreter interpreter = new Interpreter(null);
        }

        [TestMethod]
        public void InterpretEmptyString_ReturnsTrue()
        {
            ICommandService commandService = new CommandService(UserRepository.Instance, MeetingRepository.Instance);
            Interpreter interpreter = new Interpreter(commandService);

            bool result = interpreter.Interpret("");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InterpretNullString_ReturnsTrue()
        {
            ICommandService commandService = new CommandService(UserRepository.Instance, MeetingRepository.Instance);
            Interpreter interpreter = new Interpreter(commandService);

            bool result = interpreter.Interpret(null);
            Assert.IsTrue(result);
        }

    }
}