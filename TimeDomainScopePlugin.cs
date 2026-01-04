using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;

namespace SDRSharp.TimeDomainScope
{
    public class TimeDomainScopePlugin : ISharpPlugin, ICanLazyLoadGui, ISupportStatus, IExtendedNameProvider
    {
        private ControlPanel _gui;
        private ISharpControl _control;
        private TimeDomainProcessor _processor;

        public string DisplayName => "Time Domain Scope";

        public string Category => "Analysis";

        public string MenuItemName => DisplayName;

        public bool IsActive => _gui != null && _gui.Visible;

        public UserControl Gui
        {
            get
            {
                LoadGui();
                return _gui;
            }
        }

        public void LoadGui()
        {
            if (_gui == null)
            {
                _processor = new TimeDomainProcessor();
                _processor.Enabled = true;
                _gui = new ControlPanel(_control, _processor);

                // Use DecimatedAndFilteredIQ for OOK signal detection
                _control.RegisterStreamHook(_processor, ProcessorType.DecimatedAndFilteredIQ);
            }
        }

        public void Initialize(ISharpControl control)
        {
            _control = control;
        }

        public void Close()
        {
            if (_processor != null)
            {
                _processor.Enabled = false;
                _control.UnregisterStreamHook(_processor);
            }
        }
    }
}