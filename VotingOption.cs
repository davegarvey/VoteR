namespace VoteR
{
    using System.Collections.Generic;
    using System.Linq;

    public class VotingOption
    {
        public VotingOption(string name)
        {
            this.Name = name;
            this.Voters = new HashSet<string>();
        }

        /// <summary>
        /// Voting option name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets the number of votes for this option.
        /// </summary>
        public int Votes 
        {
            get
            {
                return Voters.Count();
            }
        }

        /// <summary>
        /// HashSet of connection ids that have voted for this option.
        /// </summary>
        private HashSet<string> Voters { get; set; }

        public bool UserHasVoted(string cid)
        {
            return Voters.Contains(cid);
        }

        /// <summary>
        /// Removes a votes from the option.
        /// </summary>
        /// <param name="cid">Id of the voter.</param>
        /// <returns>True if the voter was removed, otherwise false.</returns>
        public bool RemoveVoter(string cid)
        {
            return Voters.Remove(cid);
        }

        /// <summary>
        /// Adds a voter to the option.
        /// </summary>
        /// <param name="cid">Id of the voter.</param>
        public void AddVoter(string cid)
        {
            Voters.Add(cid);
        }
    }
}