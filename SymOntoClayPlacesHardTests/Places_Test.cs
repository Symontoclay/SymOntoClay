using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.HostListeners;

namespace SymOntoClayPlacesHardTests
{
    public class Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("0", message);
                            break;

                        case 3:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 4:
                            Assert.AreEqual(string.Empty, message);
                            break;

                        case 5:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                var hostListener = new Exec_Tests_HostListener4();

                throw new NotImplementedException();
            }
        }
    }
}