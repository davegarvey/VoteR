using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace VoteR
{
    [HubName("votingHub")]
    public class VotingHub : Hub
    {
        private readonly Poll _poll;

        public VotingHub() : this(Poll.Instance) { }

        public VotingHub(Poll poll)
        {
            _poll = poll;
        }

        public IEnumerable<VotingOption> GetVotingOptions()
        {
            return _poll.GetVotingOptions();
        }

        public string GetPollTitle()
        {
            return _poll.Title;
        }

        public int GetTotleVotes()
        {
            return _poll.TotalVotes;
        }

        public string GetPollState()
        {
            return _poll.PollState.ToString();
        }

        public void OpenPoll()
        {
            _poll.Open();
        }

        public void ClosePoll()
        {
            _poll.Close();
        }

        public void ResetPoll()
        {
            _poll.Reset();
        }

        public void ExportPoll()
        {
            _poll.Export();
        }

        /// <summary>
        /// Places a vote in the poll.
        /// </summary>
        /// <param name="name">Name of voting option to vote for.</param>
        public void PlaceVote(string name)
        {
            // only inform clients of vote if it is successfully placed
            if (_poll.PlaceVote(name, Context.ConnectionId))
            {
                Clients.votePlaced();
            }
        }

        /// <summary>
        /// Changes title of the poll.
        /// </summary>
        /// <param name="title">New title for poll.</param>
        public void ChangeTitle(string title)
        {
            // only inform clients of title change if title is different to existing title
            if (title != _poll.Title )
            {
                _poll.Title = title;
                Clients.titleChanged(Context.ConnectionId, title);
            }
        }

        /// <summary>
        /// Adds a voting option to the poll.
        /// </summary>
        /// <param name="name">Name of voting option.</param>
        public void AddVotingOption(string name)
        {
            // only inform clients of new voting option if it is successfully added
            if (!string.IsNullOrWhiteSpace(name) &&
                _poll.AddVotingOption(name))
            {
                Clients.votingOptionAdded(Context.ConnectionId, name);
            }
        }
    }
}