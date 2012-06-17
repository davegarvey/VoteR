/// <reference path="../Scripts/jquery-1.7.2.js" />
/// <reference path="../Scripts/jquery-ui-1.8.19.js" />
/// <reference path="../Scripts/jquery.signalR-0.5.0.js" />

$(function () {
    var votingHub = $.connection.votingHub,
        $vote = $("#vote"),
        $adminTitle = $("#adminTitle"),
        $title = $("#title"),
        $pollState = $("#pollState"),
        $votingOptions = $("#votingOptions"),
        $pollChart = $("#pollChart #container"),
        $newVotingOption = $("#newVotingOption"),
        $votingButtons = $("#votingButtons");

    $.extend(votingHub, {
        titleChanged: function (cid, title) {
            $title.text(title);
        },
        pollOpened: function () {
            $pollState.text("Open");
            $votingOptions.show();
            $("#openPoll").prop("disabled", true);
            $("#closePoll").prop("disabled", false);
            $("#resetPoll").prop("disabled", true);
        },
        pollClosed: function () {
            $pollState.text("Closed");
            $votingOptions.hide();
            $("#openPoll").prop("disabled", false);
            $("#closePoll").prop("disabled", true);
            $("#resetPoll").prop("disabled", false);
        },
        pollReset: function () {
            updateChart();
        },
        votingOptionAdded: function () {
            updateChart();
        },
        votePlaced: function () {
            updateChart();
        }
    });

    function init() {
        return updateChart();
    }

    function updateChart() {
        return votingHub.getVotingOptions()
            .done(function (votingOptions) {
                var votes,
                    total,
                    height1,
                    height2;
                    
                // add titles
                $pollChart.empty();
                $pollChart.append("<ul>");
                $.each(votingOptions, function (idx) {
                    var btnClass = (idx === 0) ? " class='first'" : "";
                    $pollChart.append("<li" + btnClass + ">" + this.Name + "</li>");
                });
                
                //calculate the chart values
                votes = this.Votes;
                total = votes[0] + votes[1];
                height1 = ((votes[0] / total) * 100);
                height2 = ((votes[1] / total) * 100);
                
                // add charts
                $pollChart.append("<div id='pollChart'>" +
                                    "<div id='container'>" +
                                        "<div id='result1'><div class='count' style='height:" + height1 + "%'></div><div class='bar' style='height:" + height1 + "%'></div></div>" +
                                        "<div id='result2'><div class='count' style='height:" + height1 + "%'></div><div class='bar' style='height:" + height2 + "%'></div></div>" +
                                    "</div>" +
                                "</div>");
                
                // add vote buttons
                $votingButtons.empty();
                $votingButtons.append("<ul>");
                $.each(votingOptions, function (idx) {
                    var btnClass = (idx === 0) ? " class='first'" : "";
                    $votingButtons.append("<li " + btnClass + "><button class='button button-1'>" + this.Name + "<span></span></button></li>");
                });

                $(".votingButton").click(function () {
                    votingHub.placeVote(this.innerText);
                    return false;
                });
            });
    }

    $.connection.hub.start().done(function () {
        // wire up actions
        $adminTitle.change(function () {
            votingHub.changeTitle($adminTitle.val());
        });

        $("#openPoll").click(function () {
            votingHub.openPoll();
            return false;
        });

        $("#closePoll").click(function () {
            votingHub.closePoll();
            return false;
        });

        $("#resetPoll").click(function () {
            votingHub.resetPoll();
            return false;
        });

        $("#submitNewVotingOption").click(function () {
            votingHub.addVotingOption($newVotingOption.val());
            return false;
        });

        // initialise poll
        init().done(function () {
            votingHub.getPollState().done(function (state) {
                $pollState.text(state);
                if (state == "Open") {
                    votingHub.pollOpened();
                } else {
                    votingHub.pollClosed();
                }
            });

            votingHub.getPollTitle().done(function (title) {
                $title.text(title);
            });
        });
    });
}); 