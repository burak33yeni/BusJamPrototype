using _Game._Scripts._ScriptableObjects;
using Core.ServiceLocator;
using UnityEngine;

namespace _Game._Scripts.LevelEditor
{
    public class LevelEditorMonoInstaller : MonoInstaller
    {
        [SerializeField] private CanvasService canvasService;
        [SerializeField] private ColorScriptableObjectsService colorScriptableObjectsService;
        [SerializeField] private LevelEditor _levelEditor;
        [SerializeField] private HumanButtonBlueForEditor _blueButton;
        [SerializeField] private HumanButtonGreenForEditor _greenButton;
        [SerializeField] private HumanButtonRedForEditor _redButton;
        [SerializeField] private HumanButtonYellowForEditor _yellowButton;
        [SerializeField] private HumanButtonMagentaForEditor _magentaButton;
        [SerializeField] private HumanButtonOrangeForEditor _orangeButton;
        [SerializeField] private LevelEditorSaveLoadService _levelEditorSaveLoadService;
        
        public override void Install(Context context)
        {
            context.Register<CanvasService>().FromInstance(canvasService);
            context.Register<ColorScriptableObjectsService>().FromInstance(colorScriptableObjectsService);
            context.Register<LevelEditor>().FromInstance(_levelEditor).NonLazy();
            context.Register<HumanButtonBlueForEditor>().FromInstance(_blueButton).NonLazy();
            context.Register<HumanButtonGreenForEditor>().FromInstance(_greenButton).NonLazy();
            context.Register<HumanButtonRedForEditor>().FromInstance(_redButton).NonLazy();
            context.Register<HumanButtonYellowForEditor>().FromInstance(_yellowButton).NonLazy();
            context.Register<HumanButtonMagentaForEditor>().FromInstance(_magentaButton).NonLazy();
            context.Register<HumanButtonOrangeForEditor>().FromInstance(_orangeButton).NonLazy();
            context.Register<LevelEditorSaveLoadService>().FromInstance(_levelEditorSaveLoadService).NonLazy();
        }
    }
}