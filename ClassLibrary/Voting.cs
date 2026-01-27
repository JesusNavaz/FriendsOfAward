using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    public class Voting
    {
        public int VotingId { get; }
        public string QrId { get; }
        public int DaId { get; }

        public Voting(int votingId, string qrId, int daId)
        {
            VotingId = votingId;
            QrId = qrId;
            DaId = daId;
        }
    }
}
