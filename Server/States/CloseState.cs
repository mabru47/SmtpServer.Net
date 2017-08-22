using System;
using System.Threading.Tasks;

namespace Tireless.Net.Mail.States
{
    class CloseState : StateBase
    {
        private String errorText = null;

        public CloseState(Connection connection) : base(connection)
        {
        }

        public CloseState(String errorText, Connection connection) : base(connection)
        {
            this.errorText = errorText;
        }

        public override async Task<StateBase> HandleStateAsync()
        {
            if (!String.IsNullOrEmpty(this.errorText))
                await base.Connection.WriteLineAsync(this.errorText);
            else
                await base.Connection.WriteLineAsync("221 Service closing transmission channel");

            return null;
        }
    }
}
