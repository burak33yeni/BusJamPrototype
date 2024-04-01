using UnityEngine;

namespace Core.ServiceLocator
{
    public class SceneContextCreator : MonoBehaviour
    {
        [SerializeField] private Context[] contextPrefabs;

        private void Awake()
        {
            for (int i = 0; i < contextPrefabs.Length; i++)
            {
                if(contextPrefabs[i].gameObject.scene.name != null)
                {
                    throw new ObjectNotPrefabException();
                }
                
                Context context = Instantiate(contextPrefabs[i], transform);
                context.Initialize(ProjectContextCreator.ProjectContext);
            }
        }
    }
}