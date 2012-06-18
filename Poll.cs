using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SignalR;
using System.Xml.Serialization;
using System.Text;
using System.IO;

namespace VoteR
{
    /// <summary>
    /// Represents a poll.
    /// </summary>
    public class Poll
    {
        private readonly static Lazy<Poll> _instance = new Lazy<Poll>(() => new Poll());
        private readonly static object _pollStateLock = new object();
        private readonly static ConcurrentDictionary<string, VotingOption> _votingOptions = new ConcurrentDictionary<string, VotingOption>();
        private static string _title;
        private PollState _pollState = PollState.Closed;

        private Poll()
        {
            LoadDefaultValues();
        }

        public static Poll Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        /// <summary>
        /// State of the poll
        /// </summary>
        public PollState PollState
        {
            get { return _pollState; }
            private set { _pollState = value; }
        }

        /// <summary>
        /// Gets the current voting options.
        /// </summary>
        /// <returns>Collection of voting options.</returns>
        public IEnumerable<VotingOption> GetVotingOptions()
        {
            return _votingOptions.Values;
        }

        /// <summary>
        /// Poll title.
        /// </summary>
        public string Title 
        { 
            get 
            { 
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// Poll total votes.
        /// </summary>
        public int TotalVotes
        {
            get
            {
                return _votingOptions.Sum(v => v.Value.Votes);
            }
        }

        /// <summary>
        /// Broadcasts the polls state of all clients.
        /// </summary>
        /// <param name="pollState">Poll state to broadcast.</param>
        private static void BroadcastPollStateChange(PollState pollState)
        {
            var clients = GetClients();
            switch (pollState)
            {
                case PollState.Open:
                    clients.pollOpened();
                    break;
                case PollState.Closed:
                    clients.pollClosed();
                    break;
                case PollState.Reset:
                    clients.pollReset();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Opens the poll.
        /// </summary>
        public void Open()
        {
            if (PollState != VoteR.PollState.Open || PollState != VoteR.PollState.Opening)
            {
                lock (_pollStateLock)
                {
                    if (PollState != VoteR.PollState.Open || PollState != VoteR.PollState.Opening)
                    {
                        PollState = VoteR.PollState.Open;
                        BroadcastPollStateChange(VoteR.PollState.Open);
                    }
                }
            }
        }

        /// <summary>
        /// Closes the poll.
        /// </summary>
        public void Close()
        {
            if (PollState == VoteR.PollState.Open || PollState == VoteR.PollState.Opening)
            {
                lock (_pollStateLock)
                {
                    if (PollState == VoteR.PollState.Open || PollState == VoteR.PollState.Opening)
                    {
                        PollState = VoteR.PollState.Closed;
                        BroadcastPollStateChange(VoteR.PollState.Closed);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the poll.
        /// </summary>
        public void Reset()
        {
            lock (_pollStateLock)
            {
                if (PollState != VoteR.PollState.Closed)
                {
                    throw new InvalidOperationException("Poll must be closed before it can be reset.");
                }

                _votingOptions.Clear();
                BroadcastPollStateChange(VoteR.PollState.Reset);
            }
        }

        public void Export()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Poll));
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, this);
            }

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            htmlDoc.LoadHtml(sb.ToString());



            //// ParseErrors is an ArrayList containing any errors from the Load statement
            //if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            //{
            //    // Handle any parse errors as required
            //}
            //else
            //{
            //    if (htmlDoc.DocumentNode != null)
            //    {
            //        HtmlAgilityPack.HtmlNode containerNode = htmlDoc.DocumentNode.SelectSingleNode("//container");

            //        if (containerNode != null)
            //        {
            //            StringBuilder resultsHtml = new StringBuilder();
            //            List<VotingOption> votingOptions = this.GetVotingOptions().ToList();

            //            resultsHtml.AppendLine("<ul>");

            //            foreach (var item in votingOptions)
            //            {
            //                resultsHtml.AppendLine(string.Concat("<li>", item.Name, "</li>"));
            //            }

            //            foreach (var item in votingOptions)
            //            {
            //                resultsHtml.AppendLine(string.Concat("<li>", item.Name, "</li>"));
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Adds a voting option to the poll.
        /// </summary>
        /// <param name="name">Name of the voting option.</param>
        /// <returns>True if the option has been added, otherwise false.</returns>
        public bool AddVotingOption(string name)
        {
            return _votingOptions.TryAdd(name, new VotingOption(name));
        }

        /// <summary>
        /// Makes a vote in the poll.
        /// </summary>
        /// <param name="name">Name of the voting option.</param>
        /// <param name="cid">Connection id of the user.</param>
        /// <returns>True if the vote has been added, otherwise false.</returns>
        public bool PlaceVote(string name, string cid)
        {
            // only open polls can be voted on
            if (PollState != VoteR.PollState.Open)
            {
                return false;
            }

            string votedOptionName;

            if (UserHasVoted(cid, out votedOptionName))
            {
                // check if vote is for same option
                if (votedOptionName == name)
                {
                    return false;
                }

                RemoveUserVote(cid);
            }

            AddUserVote(name, cid);

            return true;
        }

        /// <summary>
        /// Loads default poll options.
        /// </summary>
        private void LoadDefaultValues()
        {
            new List<VotingOption>
            {
                new VotingOption("A"),
                new VotingOption("B")
            }.ForEach(votingOption => _votingOptions.TryAdd(votingOption.Name, votingOption));

            _title = "Poll";
        }

        /// <summary>
        /// Gets the current clients.
        /// </summary>
        /// <returns></returns>
        private static dynamic GetClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<VotingHub>().Clients;
        }

        /// <summary>
        /// Checks if a user has already voted.
        /// </summary>
        /// <param name="cid">Connection id of the user.</param>
        /// <param name="name">Name of the voting option which has been voted for.</param>
        /// <returns>True if user has already voted, otherwise false.</returns>
        private bool UserHasVoted(string cid, out string name)
        {
            name = _votingOptions.SingleOrDefault(v => v.Value.UserHasVoted(cid)).Key;

            return name != null;
        }

        /// <summary>
        /// Removes a users vote.
        /// </summary>
        /// <param name="cid">Connection id of the user.</param>
        private void RemoveUserVote(string cid)
        {
            foreach (var votingOption in _votingOptions.Where(v => v.Value.UserHasVoted(cid)))
            {
                votingOption.Value.RemoveVoter(cid);
            }
        }

        /// <summary>
        /// Adds a user's vote to a voting option.
        /// </summary>
        /// <param name="name">Name of voting option to add vote to.</param>
        /// <param name="cid">Connection id of the user.</param>
        private void AddUserVote(string name, string cid)
        {
            _votingOptions.SingleOrDefault(v => v.Value.Name == name).Value.AddVoter(cid);
        }

        /// <summary>
        /// Checks if a voting option exists.
        /// </summary>
        /// <param name="name">Name of the voting option to check for.</param>
        /// <returns>True if the voting option exists, otherwise false.</returns>
        private bool VotingOptionExists(string name)
        {
            return _votingOptions.Any(v => v.Value.Name == name);
        }
    }

    /// <summary>
    /// State of the poll.
    /// </summary>
    public enum PollState
    {
        Open,
        Opening,
        Closing,
        Closed,
        Reset
    }
}