import fetchMessages from './messages.js';
import { correctButtonPress } from './messages.js';
import { incorrectButtonPress } from './messages.js';

const scrollAmount = 50;
const secondsFunFact = 30;
let rowsShown = 10;

const randomChoice = (arr) => {
    return arr[Math.floor(Math.random() * arr.length)];
}

const hideArrows = (currScroll, width) => {
    if (currScroll == 0) {
        $('#left-arrow').hide();
    } else {
        $('#left-arrow').show();
    }

    if (currScroll >= width) {
        $('#right-arrow').hide();
    } else {
        $('#right-arrow').show();
    }
}

const getUserRequest = () => {
    return $.ajax('/getUser.php', {
        type: 'GET'
    });
}

const getUser = () => {
    return getUserRequest().then(response => {
        return response['user'];
    }).catch(err => {
        const errJSON = err.responseJSON;
        if (errJSON.hasOwnProperty('message')) {
            return {
                'user': randomChoice(['tas127', 'sethl', 'vinish', 'iftekhar']),
                'errorMessage': err.responseJSON['message']
            };
        }
        return {
            'user': randomChoice(['tas127', 'sethl', 'vinish', 'iftekhar'])
        };
    });
}

const scroll = (right) => {
    const currScroll = $('#game-cards').scrollLeft();
    const newScroll = (right) ? currScroll + scrollAmount : currScroll - scrollAmount;
    const width = $('#game-cards').get(0).scrollWidth - $('#game-cards').outerWidth();
    $('#game-cards').scrollLeft(newScroll);
    hideArrows($('#game-cards').scrollLeft(), width);
}

const showMoreHandler = () => {
    const rows = $('#leaderboard tbody>tr:not(.row-info)');
    if (rowsShown > rows.length) {
        return;
    }
    for (let i = rowsShown; i < rowsShown + 5; i++) {
        rows.eq(i).removeClass('hidden');
    }

    rowsShown += 5;

    const firstHidden = $('tr.hidden').eq(0);
    const showMore = $('#showmore-row');
    const ellipsis = $('#ellipsis-row');
    showMore.detach();
    ellipsis.detach();
    if (firstHidden.length == 0) {
        return;
    }
    showMore.insertBefore(firstHidden);
    ellipsis.insertBefore(firstHidden);
}

const getAccountDataRequest = (user) => {
    return $.ajax('/sqlconnect/games/getAccountData.php', {
        type: 'POST',
        data: { 'onyen': user }
    });
}

const getAccountData = (user) => {
    return getAccountDataRequest(user).then(response => {
        return response;
    }).catch(err => {
        console.log(err);
        return {
            'Password Platformer': { 'timesPlayed': 0 },
            'Message Board': { 'timesPlayed': 0 }
        }
    })
}

const getLeaderboardRequest = () => {
    return $.ajax('/sqlconnect/games/getLeaderboard.php', {
        type: 'POST'
    });
}

const getLeaderboard = () => {
    return getLeaderboardRequest().then(response => {
        return response;
    }).catch(err => {
        console.log(err);
        return [{ 'Onyen': 'tas127', 'Password Platformer': 17110, 'Message Board': 100, 'Total': 17120 }, { 'Onyen': 'sethl', 'Password Platformer': 14805, 'Message Board': null, 'Total': 14805 }, { 'Onyen': 'vinish', 'Password Platformer': 14684, 'Message Board': -100, 'Total': 14584 }, { 'Onyen': 'iftekhar', 'Password Platformer': 7739, 'Message Board': null, 'Total': 7739 }];
    })
}

const setUpAccount = (user) => {
    $('#account>h2').text(user);
    getAccountData(user).then((response) => {
        let timesPlayed;
        if (response === undefined) {
            // for debug purposes on server not running PHP
            timesPlayed = {
                'Password Platformer': { 'timesPlayed': 0 },
                'Message Board': { 'timesPlayed': 0 }
            }
        } else {
            timesPlayed = response;
        }

        const tableGamesPlayed = $('#playtime-table>tbody');
        const tableScores = $('#your-score-table>tbody');
        for (let [key, value] of Object.entries(timesPlayed)) {
            const trTimesPlayed = $('<tr></tr>');
            const tdTimesPlayedGame = $('<td>' + key + '</td>');
            const tdTimesPlayedNumber = $('<td>' + value['timesPlayed'] + '</td>');
            trTimesPlayed.append(tdTimesPlayedGame);
            trTimesPlayed.append(tdTimesPlayedNumber);
            tableGamesPlayed.append(trTimesPlayed);

            if (value.hasOwnProperty('score')) {
                const trScores = $('<tr></tr>');
                const tdScoresGame = $('<td>' + key + '</td>');
                const tdScoresNumber = $('<td>' + value['score'] + '</td>');
                trScores.append(tdScoresGame);
                trScores.append(tdScoresNumber);
                tableScores.append(trScores);
            }
        }
    })
}

const clearLeaderboard = () => {
    $('#leaderboard-overall>tbody').empty();
    const leaderboards = $('.leaderboard-table');
    for (let i = 1; i < leaderboards.length; i++) {
        leaderboards.eq(i).remove();
    }
}

const setUpLeaderboard = (user, gameJSON) => {
    clearLeaderboard();

    const games = {};
    for (const game of gameJSON) {
        games[game['name']] = game;
    }

    return getLeaderboard().then(scores => {
        let results;
        if (scores === undefined) {
            // for debug purposes on server not running PHP
            results = [{ 'Onyen': 'tas127', 'Password Platformer': 17110, 'Message Board': 100, 'Total': 17120 }, { 'Onyen': 'sethl', 'Password Platformer': 14805, 'Message Board': null, 'Total': 14805 }, { 'Onyen': 'vinish', 'Password Platformer': 14684, 'Message Board': -100, 'Total': 14584 }, { 'Onyen': 'iftekhar', 'Password Platformer': 7739, 'Message Board': null, 'Total': 7739 }];
        } else {
            results = scores;
        }

        const leaderboard = $('#leaderboard-overall>tbody');

        let potentialBadges = [];
        for (const key of Object.keys(results[0])) {
            if (key == 'Onyen' || key == 'Total') {
                continue;
            }
            if (games.hasOwnProperty(key)) {
                potentialBadges.push(key);
            }
        }

        let badgeList = {};

        const individualLeaderboards = potentialBadges;
        individualLeaderboards.push('Message Board');
        for (const val of potentialBadges) {
            const currVals = results.filter(item => (item[val] != null));
            currVals.sort((itemOne, itemTwo) => itemTwo[val] - itemOne[val]);

            const newDiv = $('<div class="leaderboard-table"></div>');
            newDiv.append($('<h2 class="center xcel">' + val + ' Leaders</h2>'));

            const newTable = $('<table></table>');

            const thead = $('<thead></thead>');
            const theadtr = $('<tr></tr>');
            theadtr.append($('<th>Rank</th>'));
            theadtr.append($('<th>Player</th>'));
            theadtr.append($('<th>Score</th>'));
            thead.append(theadtr);
            newTable.append(thead);

            const tbody = $('<tbody></tbody>');
            let rank = 1;
            for (const row of currVals) {
                const tr = $('<tr></tr>');
                tr.append($('<td>' + rank++ + '</td>'));
                tr.append($('<td>' + row['Onyen'] + '</td>'));
                tr.append($('<td>' + row[val].toLocaleString() + '</td>'));
                tbody.append(tr);
            }
            newTable.append(tbody);
            newDiv.append(newTable);

            newDiv.insertBefore($('#right-arrow-leaderboard'));
            newDiv.hide();

            // don't award badges for spam filtering for now ...
            if (val == 'Message Board') continue;

            let numWithBadge = Math.floor(currVals.length * .15);
            if (numWithBadge == 0) numWithBadge = 1;

            const onyensWithBadge = currVals.map(item => item['Onyen']).slice(0, numWithBadge);
            for (const onyen of onyensWithBadge) {
                if (badgeList.hasOwnProperty(onyen)) {
                    badgeList[onyen].push(val);
                } else {
                    badgeList[onyen] = [val];
                }
            }
        }

        if (badgeList.hasOwnProperty(user)) {
            $('#badges').empty();
            for (const badge of badgeList[user]) {
                $('#badges').append(games[badge]['icon']);
            }
        }

        let rank = 1;
        for (const row of results) {
            const tr = $('<tr></tr>');
            const tdRank = $('<td>' + rank + '</td>');

            let name = row['Onyen'];
            if (badgeList.hasOwnProperty(row['Onyen'])) {
                for (const badge of badgeList[row['Onyen']]) {
                    name += ' ' + games[badge]['icon'];
                }
            }
            const tdName = $('<td>' + name + '</td>');
            const tdScore = $('<td>' + row['Total'].toLocaleString() + '</td>');

            tr.append(tdRank);
            tr.append(tdName);
            tr.append(tdScore);

            leaderboard.append(tr);

            rank++;
        }
    });
}

const setUpGame = (game, user) => {
    const id = game['id'];
    const name = game['name'];

    const tab = $('<button id="' + id + '-tab" class="tab" data-info="' + id + '-info" style="display: none;">' + name + '<i class="far fa-window-close close-tab"></i></button>');
    $('#top-tabs').append(tab);

    const infoDiv = $('<div id="' + id + '-info" class="tab-info"></div>');
    infoDiv.append($('<h2 class="center pixel">' + name + '</h2>'));
    infoDiv.append($('<p class="pixel">' + game['tagline'] + '</p>'));
    const div = $('<div></div>');
    const button = $('<button class="play">Play</button>"');
    button.on('click', function () {
        const newLocation = '/' + game['entrypoint'] + '?user=' + encodeURIComponent(user);
        window.open(newLocation, '_blank');
    });
    div.append(button);
    infoDiv.append(div);
    $('#main-middle').append(infoDiv);

    const classes = "game-card" + ((game['disabled']) ? " disabled" : "");

    const gameCard = $('<div class="' + classes + '" id="' + id + '-card" data-tab="' + id + '-tab"></div>');
    const header = $('<div class="game-card-header"></div>');
    header.append($('<h5>' + name + '</h5>'));
    gameCard.append(header);
    const iconDiv = $('<div class="game-card-icon"></div>');
    iconDiv.append($(game['icon']));
    gameCard.append(iconDiv);
    const cardText = $('<div class="game-card-text"></div>');
    cardText.append($('<p>' + game['card-text'] + '</p>'));
    gameCard.append(cardText);

    if (game['disabled']) {
        gameCard.append($('<p>Coming someday!</p>'));
    }
    $('#game-cards').append(gameCard);
}

const setup = () => {
    //funFacts imported using a txt file thanks to this video https://www.youtube.com/watch?time_continue=319&v=OWxVjS3yD1c&feature=emb_title
    fetch('static/funFacts/funFacts.txt')
        .then(response => {
            if (!response.ok) {
                throw new Error("HTTP error " + response.status);
            }
            return response.text();
        }).then(data => {
            const funFacts = data.split("\n");

            const genFunFact = () => {
                $('#fun-fact').text(randomChoice(funFacts));
            }

            genFunFact();

            window.setInterval(genFunFact, secondsFunFact * 1000);
        })
        .catch(error => {
            console.log('Error: ' + error);
        });


    $('.tab-info').hide();
    const currDiv = $('.tab.active').attr('data-info');
    $('#' + currDiv).show();

    $('.message-content').hide();
    const showMessage = $('.message.active').attr('data-message');
    $('#' + showMessage).show();

    $('#left-arrow').on('click', function () {
        scroll(false);
    });

    $('#right-arrow').on('click', function () {
        scroll(true);
    });

    let idx = 0;
    const leaderboards = $('.leaderboard-table');
    $('#left-arrow-leaderboard').on('click', function () {
        leaderboards.eq(idx).hide();
        idx++;
        if (idx >= leaderboards.length) {
            idx = 0;
        }
        leaderboards.eq(idx).show();
    });

    $('#right-arrow-leaderboard').on('click', function () {
        leaderboards.eq(idx).hide();
        idx--;
        if (idx < 0) {
            idx = leaderboards.length - 1;
        }
        leaderboards.eq(idx).show();
    });

    $('#main-button').on('click', function () {
        $('#messages').hide();
        $('#info').show();
    });

    $('#messages-button').on('click', function () {
        $('#info').hide();
        $('#messages').show();
    });

    $('.game-card:not(.disabled)').on('click', function () {
        const tab = $('#' + $(this).attr('data-tab'));
        tab.show();
        $('.tab').removeClass('active');
        tab.addClass('active');
        const show = tab.attr('data-info');
        $('.tab-info').hide();
        $('#' + show).show();
    });

    $('.tab').on('click', function () {
        const show = $(this).attr('data-info');
        $('.tab-info').hide();
        $('#' + show).show();
        $('.tab').removeClass('active');
        $(this).addClass('active');
    });

    $('.close-tab').on('click', function (eve) {
        $(this).parent().hide();
        $('.tab').removeClass('active');
        $('.tab-info').hide();
        const tabShow = $('.tab:visible').eq(0);
        tabShow.addClass('active');
        $('#' + tabShow.attr('data-info')).show();
        eve.stopPropagation();
    });

    $('.message').on('click', function () {
        $('.message').removeClass('active');
        $(this).addClass('active');
        $('.message-content').hide();
        const content = $(this).attr('data-message');
        $('#' + content).show();
    });

    $('#showmore').on('click', function () {
        showMoreHandler();
    });

    $('form').on('submit', function (eve) {
        eve.preventDefault();
    });
}

const fetchGames = (user) => {
    fetch('games.json')
        .then(response => {
            if (!response.ok) {
                throw new Error("HTTP error " + response.status);
            }
            return response.json();
        }).then(json => {
            for (let game of json) {
                setUpGame(game, user);
            }
            setUpLeaderboard(user, json).then(() => {
                setup();
            });
        });
}

$(document).ready(function () {
    getUser().then((response) => {
        let user;
        if (response === undefined) {
            // for debug purposes on server not running PHP
            user = randomChoice(['tas127', 'sethl', 'vinish', 'iftekhar']);
        } else {
            if (response.hasOwnProperty('errorMessage')) {
                // handle the error case when not able to get user
                console.log(response['errorMessage']);
            }

            if (response.hasOwnProperty('user')) {
                user = response['user'];
            } else {
                user = response;
            }

        }
        console.log('User: ' + user);

        hideArrows($('#game-cards').scrollLeft(), $('#game-cards').width());

        fetchGames(user);

        fetchMessages().then(() => {
            $('.correctButton').each((index, element) => {
                $(element).on('click', () => {
                    correctButtonPress(user);
                });
            });

            $('.incorrectButton').each((index, element) => {
                $(element).on('click', () => {
                    incorrectButtonPress(user).then(() => {
                        console.log('hello!');
                        window.location.href = '/message.html';
                    }).catch(() => {
                        window.location.href = '/message.html';
                    });
                });
            });
        });

        setUpAccount(user);
    });

    //messages on button press - this will trigger the php call that will update the db
    $('.correctButton').on('click', function () {
        correctButtonPress;
    })
    $('.incorrectButton').on('click', function () {
        incorrectButtonPress;
    })
});