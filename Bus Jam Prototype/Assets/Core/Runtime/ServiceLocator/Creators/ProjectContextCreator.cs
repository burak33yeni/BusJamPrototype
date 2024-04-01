using UnityEngine;

namespace Core.ServiceLocator
{
    internal static class ProjectContextCreator
    {
        private static readonly string _contextName = "ProjectContext";
        private static Context _projectContext;
        internal static Context ProjectContext => _projectContext;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadProjectContext()
        {
            if (_projectContext != null) return;
            Context projectContextPrefab = Resources.Load<Context>(_contextName);
            _projectContext = Object.Instantiate(projectContextPrefab);
            _projectContext.Initialize();
            _projectContext.CheckPersistence();
            Object.DontDestroyOnLoad(_projectContext);
        }
    }
}