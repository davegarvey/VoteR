<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VoteR.Default" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title></title>
    <link href="assets/css/style.css" rel="stylesheet">
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
           <h2>Poll detail</h2>
            <fieldset class="admin admin-title">                
                <label for="adminTitle">title</label><input id="adminTitle" type="text" />
                <label for="newVotingOption">new voting option</label><input id="newVotingOption" type="text" /><button class="button button-1" id="submitNewVotingOption">submit<span></span></button>
            </fieldset>
            
            <h2>Poll state</h2>
            <fieldset class="admin admin-status">                
                <button id="openPoll" class="button button-1" value="open">open</button>
                <button id="closePoll" class="button button-3" value="close">close</button>
                <button id="resetPoll" class="button button-4" value="reset">reset</button>
            </fieldset>

            <h2>Export</h2>
            <fieldset class="admin admin-export">
                <button id="exportPoll" class="button button-1" value="export">export</button>
            </fieldset>
        <% } %>
        <h1 id="title"></h1>

        <p class="heading">Poll is <span id="pollState"></span></p>
         
        <div id="results">
            <div id="pollChart">
                <div id="container">
                    <!--
                
                    <div id="result1"><div class="count" style="bottom:75%;">3</div><div class="bar" style="height:75%;"></div></div>
                    <div id="result2"><div class="count" style="bottom:25%;">1</div><div class="bar" style="height:25%;"></div></div>
        
                    -->
                </div>
            </div>
        </div>
    
        <h2 id="castVote">Cast your vote</h2>
        <fieldset id="votingOptions">
            <div id="votingButtons">
                <!--
                
                <li class="first"><button class="button button-1">Poll 1<span></span></button></li>
                <li><button class="button button-2">Poll 2<span></span></button></li>
                
                -->
            </div>
        </fieldset>

    </form>
    
    <script src="/assets/js/libs/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/assets/js/libs/jquery-ui-1.8.19.js" type="text/javascript"></script>
    <script src="/assets/js/libs/jquery.signalR-0.5.0.js" type="text/javascript"></script>
    <script src="/signalr/hubs" type="text/javascript"></script>    
    <script src="/assets/js/VoteR.js" type="text/javascript"></script>
</body>
</html>