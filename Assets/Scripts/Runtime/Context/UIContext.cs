using Rich.Base.Runtime.Concrete.Context;
using Rich.Base.Runtime.Extensions;
using Rich.Base.Runtime.Signals;
using Runtime.Mediators.Screen;
using Runtime.Signals;
using Runtime.Views.Screen;

namespace Runtime.Context
{
    public class UIContext : BaseUIContext
    {
        private UISignals _uiSignals;
        private CoreScreenSignals _coreScreenSignals;

        protected override void mapBindings()
        {
            base.mapBindings();

            //Injection Bindings
            _uiSignals = injectionBinder.BindCrossContextSingletonSafely<UISignals>();
            _coreScreenSignals = injectionBinder.BindCrossContextSingletonSafely<CoreScreenSignals>();

            //Mediation Bindings
            mediationBinder.BindView<LevelScreenView>().ToMediator<LevelScreenMediator>();
            mediationBinder.BindView<FailScreenView>().ToMediator<FailScreenMediator>();
            mediationBinder.BindView<WinScreenView>().ToMediator<WinScreenMediator>();
            mediationBinder.BindView<StartScreenView>().ToMediator<StartScreenMediator>();
        }

        public override void Launch()
        {
            _coreScreenSignals.OpenPanel.Dispatch(new OpenNormalPanelArgs()
            {
                IgnoreHistory = false,
                LayerIndex = 0,
                PanelKey = "StartScreen"
            });
        }
    }
}