using Tireless.Net.Mail.Commands;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.States
{
    class StartTlsState : StateBase
    {
        private StartTlsState(Connection connection) : base(connection)
        {
        }

        public StartTlsState(StartTlsCommand cmd, Connection connection) : this(connection)
        {
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            if (this.Server.X509Certificate != null)
            {
                await base.Connection.WriteLineAsync("220 Go ahead");
                await base.Connection.UpgradeConnectionAsync();
            }
            else
                await base.Connection.WriteLineAsync("500 Command not recognized: No certificate specified.");
            return new BeginState(fromSslUpgrade: true, connection: base.Connection);
        }
    }
}
