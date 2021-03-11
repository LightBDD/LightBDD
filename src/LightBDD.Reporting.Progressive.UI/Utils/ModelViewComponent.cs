using Microsoft.AspNetCore.Components;

namespace LightBDD.Reporting.Progressive.UI.Utils
{
    public class ModelViewComponent : ComponentBase
    {
        private bool _modelChanged;
        protected int ChangesCounter { get; private set; }
        protected void ModelHasChanged()
        {
            _modelChanged = true;
            ++ChangesCounter;
            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _modelChanged = false;
        }

        protected override bool ShouldRender()
        {
            return _modelChanged;
        }

    }
}