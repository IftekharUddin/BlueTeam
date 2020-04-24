import fetchMessages from './messages.js';
import { correctButtonPress } from './messages.js';
import { incorrectButtonPress } from './messages.js';

const scrollAmount = 50;
const secondsFunFact = 30;

const randomChoice = (arr) => {
    // get a random item from an array
    return arr[Math.floor(Math.random() * arr.length)];
}

const hideArrows = (currScroll, width) => {
    // hide the left or right arrow on the games carousel according to 
    // how much the carousel is currently scrolled

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
    // get the request to get the user - change this here if you want to move this file around
    return $.ajax('/getUser.php', {
        type: 'GET'
    });
}

const getUser = () => {
    // handle the fetch of the user - give this back to whoever calls it
    return getUserRequest().then(response => {
        // if the request is successful, we'll have a JSON with field "user"
        return response['user'];
    }).catch(err => {
        // if it fails (running locally, the PHP file fails), give the page back a random user
        // and, if have errorMessage, report that
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
    // scroll the games carousel
    const currScroll = $('#game-cards').scrollLeft();
    const newScroll = (right) ? currScroll + scrollAmount : currScroll - scrollAmount;
    const width = $('#game-cards').get(0).scrollWidth - $('#game-cards').outerWidth();
    $('#game-cards').scrollLeft(newScroll);
    hideArrows($('#game-cards').scrollLeft(), width);
}

const getAccountDataRequest = (user) => {
    // get the request to get account data
    // if you want to change the route, file name, or post data this can easily be changed here
    return $.ajax('/sqlconnect/games/getAccountData.php', {
        type: 'POST',
        data: { 'onyen': user }
    });
}

const getAccountData = (user) => {
    // return the account data (times played and score for each game)
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
    // get the request to get leaderboard data
    // if you want to change the route or file name this can easily be changed here
    return $.ajax('/sqlconnect/games/getLeaderboard.php', {
        type: 'POST'
    });
}

const getLeaderboard = () => {
    // get the leaderboard data
    // if fails, sets up an example leaderboard
    return getLeaderboardRequest().then(response => {
        return response;
    }).catch(err => {
        console.log(err);
        return [{ 'Onyen': 'tas127', 'Password Platformer': 17110, 'Message Board': 100, 'Total': 17120 }, { 'Onyen': 'sethl', 'Password Platformer': 14805, 'Message Board': null, 'Total': 14805 }, { 'Onyen': 'vinish', 'Password Platformer': 14684, 'Message Board': -100, 'Total': 14584 }, { 'Onyen': 'iftekhar', 'Password Platformer': 7739, 'Message Board': null, 'Total': 7739 }];
    })
}

const addToTable = (table, data, classes = "") => {
    // add an array of data to a table 
    // this function cannot handle more complex table data yet (e.g. classes and such), but gets the job done
    let tr;
    if (classes.length > 0) {
        tr = $('<tr class="' + classes + '"></tr>');
    } else {
        tr = $('<tr></tr>');
    }

    for (const currData of data) {
        const td = $('<td>' + currData + '</td>');
        tr.append(td);
    }

    table.append(tr);
}

const setUpAccount = (user) => {
    // populate the Account page with user's data
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
            addToTable(tableGamesPlayed, [key, value['timesPlayed']]);

            if (value.hasOwnProperty('score')) {
                // user may not have a score for every game - in this case they do have a 
                // times played (0), but they do not have a score
                addToTable(tableScores, [key, value['score']]);
            }
        }
    })
}

const clearLeaderboard = () => {
    // clear the leaderboard
    // when we have the leaderboard load again, this can be used
    $('#leaderboard-overall>tbody').empty();

    // remove leaderboard tables that are not the overall one (which is always first in the DOM)
    const leaderboards = $('.leaderboard-table');
    for (let i = 1; i < leaderboards.length; i++) {
        leaderboards.eq(1).remove();
    }
}

const makeNewTable = (headers) => {
    // make a new table with an array of headers
    const newTable = $('<table></table>');

    const thead = $('<thead></thead>');
    const theadtr = $('<tr></tr>');
    for (const th of headers) {
        theadtr.append($('<th>' + th + '</th>'));
    }
    thead.append(theadtr);
    newTable.append(thead);

    const tbody = $('<tbody></tbody>');
    newTable.append(tbody);

    return [newTable, tbody];
}

const hideRows = (table) => {
    // hide some number of rows (by default show first 10)
    const numRows = $(table).find('tr').length;
    if (numRows > 10) {
        $(table).find('tr').each((index, element) => {
            if (index < 10) return;
            $(element).addClass('hidden');
        });

        const showMoreRow = $('<tr class="row-info showmore-row"></tr>');
        const showmoretd = $('<td colspan="3"><button class="showmoreButton">Show More <i class="fas fa-plus-circle"></i></button></td>');
        showMoreRow.append(showmoretd);
        showMoreRow.insertBefore($(table).find('tr.hidden').eq(0));
        const ellipsisrow = $('<tr class="row-info ellipsis-row"><td colspan="3"><i class="fas fa-ellipsis-h"></i></td></tr>');
        ellipsisrow.insertAfter(showMoreRow);
    }
}

const setUpLeaderboard = (user, gameJSON) => {
    // set up the leaderboard(s)
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
            if (key == 'Onyen' || key == 'Total') continue;

            if (games.hasOwnProperty(key)) {
                potentialBadges.push(key);
            }
        }

        let badgeList = {};

        const individualLeaderboards = potentialBadges;
        individualLeaderboards.push('Message Board');
        for (const val of individualLeaderboards) {
            // find users which have a score for the current leaderboard we're looking at
            const currVals = results.filter(item => (item[val] != null));
            // sort the items descending by score for current leaderboard
            currVals.sort((itemOne, itemTwo) => itemTwo[val] - itemOne[val]);

            const newDiv = $('<div class="leaderboard-table"></div>');
            newDiv.append($('<h2 class="center xcel">' + val + ' Leaders</h2>'));

            const [newTable, tbody] = makeNewTable(['Rank', 'Player', 'Score']);
            let rank = 1;
            for (const row of currVals) {
                let name = row['Onyen'];
                let classes = "";

                if (name == user) {
                    classes += 'you';
                    name = 'You';
                }

                addToTable(tbody, [rank++, name, row[val].toLocaleString()], classes);
            }
            newDiv.append(newTable);

            hideRows(tbody);

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

        // if the user has any badges, put them on the account page
        if (badgeList.hasOwnProperty(user)) {
            $('#badges').empty();
            for (const badge of badgeList[user]) {
                $('#badges').append(games[badge]['icon']);
            }
        }

        let rank = 1;
        for (const row of results) {
            let classes = (row['Onyen'] == user) ? 'you' : "";
            let name = (row['Onyen'] == user) ? 'You' : row['Onyen'];

            if (badgeList.hasOwnProperty(row['Onyen'])) {
                for (const badge of badgeList[row['Onyen']]) {
                    name += ' ' + games[badge]['icon'];
                }
            }

            addToTable(leaderboard, [rank, name, row['Total'].toLocaleString()], classes);

            rank++;
        }

        hideRows(leaderboard);
    });
}

const setUpGame = (game, user) => {
    // add a single game to the carousel and a tab to the page
    const id = game['id'];
    const name = game['name'];

    const tab = $('<button id="' + id + '-tab" class="tab" data-info="' + id + '-info" style="display: none;">' + name + '<i class="far fa-window-close close-tab"></i></button>');
    $('#top-tabs').append(tab);

    const infoDiv = $('<div id="' + id + '-info" class="tab-info"></div>');
    infoDiv.append($('<h2 class="center pixel">' + name + '</h2>'));
    infoDiv.append($('<p class="pixel">' + game['overview'] + '</p><br>'));
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

    $('form').on('submit', function (eve) {
        eve.preventDefault();
    });

    $('.showmoreButton').each((index, element) => {
        const tbody = $(element).closest('tbody');
        $(element).on('click', () => {
            const rows = tbody.find('tr').not('.row-info');
            const rowsShown = tbody.find('tr').not('.row-info').not('.hidden').length;

            if (rowsShown > rows.length) {
                return;
            }
            for (let i = rowsShown; i < rowsShown + 5; i++) {
                rows.eq(i).removeClass('hidden');
            }

            const firstHidden = rows.find('tr.hidden').eq(0);
            const showMore = tbody.find('.showmore-row');
            const ellipsis = tbody.find('.ellipsis-row');
            showMore.detach();
            ellipsis.detach();
            if (firstHidden.length == 0) return;
            showMore.insertBefore(firstHidden);
            ellipsis.insertBefore(firstHidden);

        })
    });
}

const fetchGames = (user) => {
    // fetch and set up all the games from the JSON file
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
    // run as soon as the page loads

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
                    correctButtonPress(user).then(() => {
                        window.location.href = '/goodMessage.html';
                    });
                    //add hide functionality here?
                    //$(this).hide()
                });
            });

            $('.incorrectButton').each((index, element) => {
                $(element).on('click', () => {
                    incorrectButtonPress(user).then(() => {
                        window.location.href = '/message.html';
                    }).catch(() => {
                        window.location.href = '/message.html';
                    });
                });
            });
        });

        setUpAccount(user);
    });
});