namespace SymOntoClay.BaseTestLib
{
    public interface INewBehaviorTestEngineInstance: IDisposable
    {
        /// <summary>
        /// Runs testing.
        /// </summary>
        /// <returns>Returns <c>True</c> if success, otherwise <c>False</c>.</returns>
        bool Run();
    }
}
