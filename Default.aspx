<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VoteR.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .option
        {
            width: 200px;
            height:1px;
            background-color: #ccc;
            border: #333 solid 2px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <% if (IsAdmin)
           { %>
            <fieldset>
                <legend>admin</legend>
                <label for="adminTitle">title</label><input id="adminTitle" type="text" />
                <label for="newVotingOption">new voting option</label><input id="newVotingOption" type="text" /><button id="submitNewVotingOption">submit</button>
            </fieldset>
               
            <fieldset>
                <legend>poll state</legend>
                <button id="openPoll" value="open">open</button>
                <button id="closePoll" value="close">close</button>
                <button id="resetPoll" value="reset">reset</button>
            </fieldset>
        <% } %>
        <h1 id="title"></h1>

        <label>Poll State</label>
        <input type="text" disabled="disabled" id="pollState" />
         
        <div id="pollChart"></div>
    
        <fieldset id="votingOptions">
            <legend>cast your vote</legend>
            <div id="votingButtons"></div>
        </fieldset>

    </form>
    
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/Scripts/jquery-ui-1.8.19.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.signalR-0.5.0.js" type="text/javascript"></script>
    <script src="/signalr/hubs" type="text/javascript"></script>    
    <script src="VoteR.js" type="text/javascript"></script>
</body>
</html>