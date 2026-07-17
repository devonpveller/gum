using Gum.InnerThoughts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Gum.Tests
{
    /// <summary>
    /// Error-path coverage for the parser: malformed input must surface as a graceful
    /// diagnostic (a null script), never as a hang, an unhandled exception, or a process abort.
    /// </summary>
    [TestClass]
    public class BungeeErrors
    {
        private CharacterScript? Read(string input)
        {
            string[] lines = Regex.Split(input, @"\r?\n|\r");
            Parser parser = new("Test", lines);

            return parser.Start();
        }

        [TestMethod]
        public void MalformedColonLine_DoesNotThrow()
        {
            // A dialogue line whose first word starts with ':' used to index speakerText[-1]
            // and throw IndexOutOfRangeException with no line information.
            const string situationText = @"
=Encounter
    : hello there";

            CharacterScript? script = Read(situationText);

            // The line is simply treated as text with no speaker; parsing must succeed gracefully.
            Assert.IsNotNull(script);
        }

        [TestMethod]
        public void ElseConditionWithoutPriorCondition_ReturnsNullInsteadOfAborting()
        {
            // An '(...)' else block with no prior condition used to hit Debug.Fail("Empty edge?"),
            // aborting the whole process in a Debug build.
            const string situationText = @"
=a
    (...)
        hi";

            CharacterScript? script = Read(situationText);

            Assert.IsNull(script);
        }
    }
}
