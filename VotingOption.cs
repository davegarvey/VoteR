using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VoteR
{
    public class VotingOption
    {
        public string Name { get; set; }
        
        public int Votes 
        {
            get
            {
                return Voters.Count();
            }
        }

        /// <summary>
        /// List of connection ids that have voted for this option.
        /// </summary>
        private List<string> Voters { get; set; }

        public VotingOption(string name)
        {
            this.Name = name;
            this.Voters = new List<string>();
        }

        public bool UserHasVoted(string cid)
        {
            return Voters.Contains(cid);
        }

        public bool RemoveVoter(string cid)
        {
            return Voters.Remove(cid);
        }

        public void AddVoter(string cid)
        {
            Voters.Add(cid);
        }
    }
}