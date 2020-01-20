using System.ServiceProcess;

namespace CydonService
{
    public partial class ReportProcessingService : ServiceBase
    {
        public ReportProcessingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
        }

        protected override void OnStop()
        {

        }
    }
}
