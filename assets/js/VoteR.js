﻿/// <reference path="../Scripts/jquery-1.7.2.js" />
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
        $votingButtons = $("#votingButtons"),
        $chartTitles,
        $buttonList;

    $.extend(votingHub, {
        titleChanged: function (cid, title) {
            $title.text(title);
        },
        pollOpened: function () {
            $pollState.text("Open");
            $votingOptions.show();
            $("h2#castVote").show();
            $("#openPoll").prop("disabled", true);
            $("#closePoll").prop("disabled", false);
            $("#resetPoll").prop("disabled", true);
        },
        pollClosed: function () {
            $pollState.text("Closed");
            $votingOptions.hide();
            $("h2#castVote").hide();
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
                var //votes = [],
                    total = 0,
                    containerWidth = $pollChart.width(),
                    barWidth,
                    barMarginWidth;

                // prep titles area
                $pollChart.empty();
                $pollChart.append("<ul id='chart-titles'>");
                $chartTitles = $("#chart-titles");

                // prep vote buttons area
                $votingButtons.empty();
                $votingButtons.append("<ul id='button-list'>");
                $buttonList = $("#button-list");

                $.each(votingOptions, function (idx) {
                    //calculate the chart values
                    total += this.Votes;
                });

                // get the percentage of 10px in the current width
                barMarginWidth = 10 / (containerWidth / 100);
                barWidth = (100 - ((votingOptions.length - 1) * barMarginWidth)) / votingOptions.length;

                // add a chart bar and a voting button for each vote type
                $.each(votingOptions, function (idx) {
                    var //vote = votes[idx],
                        chartHeight,
                        cssIndex = (idx + 1),
                        btnClass = ((idx === 0) || ((idx % 2) === 0)) ? " class='first'" : "",
                        leftPos = (barMarginWidth * idx) + (barWidth * idx);

                    if (this.Votes > 0) {
                        chartHeight = ((this.Votes / total) * 100);
                    } else {
                        chartHeight = 0;
                    }

                    // add chart titles                   
                    $chartTitles.append("<li style='width:" + barWidth + "%; margin-left:" + ((idx > 0) ? barMarginWidth : 0) + "%'>" + this.Name + "</li>");

                    // add charts
                    $pollChart.append("<div class='result result" + cssIndex + "' style='width:" + barWidth + "%; left:" + leftPos + "%'><div class='count' style='bottom:" + chartHeight + "%'>" + this.Votes + "</div><div class='bar' style='height:" + chartHeight + "%'></div></div>");

                    // add buttons
                    $buttonList.append("<li " + btnClass + "><button class='votingButton button button-" + cssIndex + "'>" + this.Name + "<span></span></button></li>");
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

        $("#exportPoll").click(function () {
            votingHub.exportPoll();
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