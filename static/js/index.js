import fetchMessages from './messages.js';
import { correctButtonPress, incorrectButtonPress } from './messages.js';

// getting started with jQuery - https://api.jquery.com/, https://jquery.com/
// jQuery AJAX - https://api.jquery.com/category/ajax/, https://api.jquery.com/jquery.ajax/, https://www.w3schools.com/xml/ajax_intro.asp

// Promise allSettled - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise/allSettled
// polyfill for allSettled if browser does not implement it
if (!Promise.allSettled) {
    Promise.allSettled = (promises) => {
        let wrappedPromises = promises.map(p => Promise.resolve(p)
            .then(
                val => ({ status: 'fulfilled', value: val }),
                err => ({ status: 'rejected', reason: err })));
        return Promise.all(wrappedPromises);
    }
}

// show a spinner whenever do AJAX request per https://stackoverflow.com/questions/68485/how-to-show-loading-spinner-in-jquery
// spinner generated at https://loading.io
jQuery.ajaxSetup({
    beforeSend: () => {
        $('#spinner').show();
        $('body').css('opacity', '0.5');
    },
    complete: () => {
        $('#spinner').hide();
        $('body').css('opacity', '1.0');
    },
    success: () => { }
})

// number of units to scroll per click
const scrollAmount = 50;
// number of seconds before generating a new fun fact
const secondsFunFact = 30;

const randomChoice = (arr) => {
    // get a random item from an array
    if (!Array.isArray(arr)) return null;
    return arr[Math.floor(Math.random() * arr.length)];
}

const hideArrows = (currScroll, width) => {
    /*
    * hide the left or right arrow on the games carousel according to 
    * how much the carousel is currently scrolled 
    * */
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
    // parameter right = boolean (true for scrolling right, false for scrolling left)
    // scroll the games carousel

    // scroll width - https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollWidth
    // jQuery outer width - https://api.jquery.com/outerWidth/
    // jQuery scrollLeft - https://api.jquery.com/scrollLeft/#scrollLeft
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
        // score is allowed to be null (in which case just not filled in in account data tables)
        return {
            'Password Platformer': { 'timesPlayed': 0 },
            'Message Board': { 'timesPlayed': 0 }
        }
    });
}

const getOverallLeaderboard = () => {
    // get the leaderboard data
    // if fails, sets up an example leaderboard
    return $.ajax('/sqlconnect/games/getLeaderboard.php', {
        type: 'POST'
    }).then(response => {
        return response;
    }).catch(err => {
        console.log(err);
        return [{ 'Onyen': 'tas127', 'Total': 17120 }, { 'Onyen': 'sethl', 'Total': 14805 }, { 'Onyen': 'vinish', 'Total': 14584 }, { 'Onyen': 'iftekhar', 'Total': 7739 }];
    })
}

const getIndividualLeaderboard = (nameBoard) => {
    // return data from a PHP file which corresponds to a given leaderboard
    // these tables always return the structure of (Onyen, Score)

    // map from name of leaderboard to table name
    const tableMap = {
        'Password Platformer': 'PasswordPlatformerScores',
        'Message Board': 'MessageBoardScores'
    }

    // if don't have the the leaderboard in our map, return empty array
    if (!tableMap.hasOwnProperty(nameBoard)) {
        return Promise.resolve([]);
    }

    return $.ajax('/sqlconnect/games/getIndividualLeaderboard.php', {
        type: 'POST',
        data: { 'table': tableMap[nameBoard] }
    }).then(response => {
        return response;
    }).catch(err => {
        console.log(err);
        if (nameBoard == 'Password Platformer') {
            return [{ 'Onyen': 'tas127', 'Score': 17020 }, { 'Onyen': 'sethl', 'Score': 14905 }, { 'Onyen': 'vinish', 'Score': 14584 }, { 'Onyen': 'iftekhar', 'Score': 7739 }];
        } else if (nameBoard == 'Message Board') {
            return [{ 'Onyen': 'tas127', 'Score': 100 }, { 'Onyen': 'sethl', 'Score': -100 }];
        } else {
            return [];
        }
    });
}

const setUpIndividualLeaderboard = (nameBoard, user) => {
    /*
    * Set up an individual leaderboard with the following parameters:
    * nameBoard - the name of the game (e.g. Password Platformer, Message Board)
    * user      - the onyen of the current user (used to style that row) 
    */
    return getIndividualLeaderboard(nameBoard).then(scores => {
        let results;
        if (scores === undefined) {
            if (nameBoard == 'Password Platformer') {
                results = [{ 'Onyen': 'tas127', 'Score': 17020 }, { 'Onyen': 'sethl', 'Score': 14905 }, { 'Onyen': 'vinish', 'Score': 14584 }, { 'Onyen': 'iftekhar', 'Score': 7739 }];
            } else if (nameBoard == 'Message Board') {
                results = [{ 'Onyen': 'tas127', 'Score': 100 }, { 'Onyen': 'sethl', 'Score': -100 }];
            } else {
                results = [];
            }
        } else {
            results = scores;
        }

        // need this class "leaderboard-table" to properly make the arrow handlers work!
        const newDiv = $('<div class="leaderboard-table"></div>');
        newDiv.append($('<h2 class="center xcel">' + nameBoard + ' Leaders</h2>'));

        const [newTable, tbody] = makeNewTable(['Rank', 'Player', 'Score']);
        if (results.length > 0) {
            let rank = 1;
            for (const row of results) {
                const [name, classes] = (row['Onyen'] == user) ? ['You', 'you'] : [row['Onyen'], ''];

                addToTable(tbody, [rank++, name, row['Score'].toLocaleString()], classes);
            }
        } else {
            const row = $('<tr class="none"><td colspan="3">No data to show!</td></tr>');
            tbody.append(row);
        }
        newDiv.append(newTable);

        hideRows(tbody, user);

        newDiv.insertBefore($('#right-arrow-leaderboard'));
        newDiv.hide();

        // don't award badges for spam filtering for now ...
        if (nameBoard == 'Message Board') return {};

        // award a badge to the top 15%
        let numWithBadge = Math.floor(results.length * .15);
        if (numWithBadge == 0) numWithBadge = 1;

        const badgeList = {};

        // map - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/map
        // slice - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/slice
        const onyensWithBadge = results.map(item => item['Onyen']).slice(0, numWithBadge);
        for (const onyen of onyensWithBadge) {
            badgeList[onyen] = nameBoard;
        }

        // give the badge list to the overall leaderboard
        return badgeList;
    });
}

const addToTable = (table, data, classes = "") => {
    // add an array of data to a table

    // if passed tbody, use it; otherwise, get it from the table
    const tagName = table.prop('tagName').toLowerCase();
    if (tagName != 'table' && tagName != 'tbody') return;
    const element = (tagName == 'table') ? table.find('tbody') : table;

    const tr = (classes.length > 0) ? $('<tr class="' + classes + '"></tr>') : $('<tr></tr>');

    for (const currData of data) {
        const td = $('<td>' + currData + '</td>');
        tr.append(td);
    }

    element.append(tr);
}

const setUpAccount = (user) => {
    // populate the Account page with user's data
    $('#account>h2').text(user);

    getAccountData(user).then((response) => {
        let accountData;

        if (response === undefined) {
            // for debug purposes on server not running PHP
            accountData = {
                'Password Platformer': { 'timesPlayed': 0 },
                'Message Board': { 'timesPlayed': 0 }
            }
        } else {
            accountData = response;
        }

        const tableGamesPlayed = $('#playtime-table>tbody');
        const tableScores = $('#your-score-table>tbody');

        // Object.values - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_objects/Object/values
        const numScores = Object.values(accountData).filter(item => item.hasOwnProperty('score')).length;
        if (numScores == 0) {
            const row = $('<tr class="none"><td colspan="2">No scores yet!</td></tr>');
            tableScores.append(row);
        }

        // Object.entries - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Object/entries
        // like dict.items() in Python
        for (let [key, value] of Object.entries(accountData)) {
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
    // jQuery slice - https://api.jquery.com/slice/
    $('.leaderboard-table').slice(1).remove();
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

const hideRows = (table, user, numRowsShown = 10) => {
    // hide some number of rows (by default show first 10)
    if (numRowsShown <= 0) numRowsShown = 10;

    // if passed tbody, use it; otherwise, get it from the table
    const tagName = table.prop('tagName').toLowerCase();
    if (tagName != 'table' && tagName != 'tbody') return;
    const element = (tagName == 'table') ? table.find('tbody') : table;

    const numRows = $(element).find('tr').length;
    if (numRows > numRowsShown) {
        $(element).find('tr').slice(numRowsShown).addClass('hidden');

        const userRow = $(element).find('td').filter((idx, elem) => $(elem).text() == 'You' || $(elem).text() == user).parent();
        userRow.removeClass('hidden');

        const numHidden = $(table).find('tr.hidden').length;

        if (numHidden == 0) return;

        const showMoreRow = $('<tr class="row-info showmore-row"></tr>');
        const showmoretd = $('<td colspan="3"><button class="showmoreButton">Show More <i class="fas fa-plus-circle"></i></button></td>');
        showMoreRow.append(showmoretd);
        showMoreRow.insertBefore($(element).find('tr.hidden').eq(0));
        const ellipsisrow = $('<tr class="row-info ellipsis-row"><td colspan="3"><i class="fas fa-ellipsis-h"></i></td></tr>');
        ellipsisrow.insertAfter(showMoreRow);
    }
}

const setUpLeaderboard = (user, gameJSON) => {
    // set up the overall and individual leaderboards
    clearLeaderboard();

    // build a dictionary from game name => game representation from games.json
    const games = {};
    for (const game of gameJSON) {
        games[game['name']] = game;
    }

    // change here when we have more individual leaderboards to manage
    const individualLeaderboards = ['Password Platformer', 'Message Board'];
    const leaderboardPromises = individualLeaderboards.map(item => setUpIndividualLeaderboard(item, user));

    return Promise.allSettled(leaderboardPromises).then((results) => {
        /* 
        * current structure is that each leaderboard gives back an object with keys of onyens and 
        * value pointing to the name of the leaderboard (which can then be used to get its icon)
        * this structure can be changed
        */
        const reducer = (accumulator, currentValue) => {
            for (const [key, value] of Object.entries(currentValue)) {
                if (accumulator.hasOwnProperty(key)) {
                    accumulator[key].push(value);
                } else {
                    accumulator[key] = [value];
                }
            }
            return accumulator;
        }
        // get the promises that succeeded => get the values => combine them together
        let badgeList = results.filter(item => item['status'] == 'fulfilled').map(item => item['value']).reduce(reducer, {});

        if (badgeList == null || badgeList == undefined) badgeList = {};
        // if the user has any badges, put them on the account page
        if (badgeList.hasOwnProperty(user)) {
            $('#badges').empty();
            for (const badge of badgeList[user]) {
                $('#badges').append(games[badge]['icon']);
            }
        }

        return badgeList;
    }).then(badgeList => {
        // use this badge list in thee overall leaderboard
        return getOverallLeaderboard().then(scores => {
            let results;

            if (scores === undefined) {
                // for debug purposes on server not running PHP
                results = [{ 'Onyen': 'tas127', 'Total': 17120 }, { 'Onyen': 'sethl', 'Total': 14805 }, { 'Onyen': 'vinish', 'Total': 14584 }, { 'Onyen': 'iftekhar', 'Total': 7739 }];
            } else {
                results = scores;
            }

            const leaderboard = $('#leaderboard-overall>tbody');

            const getBadgeList = (onyen) => {
                // let's define a quick function which allows us to either get a user's badgelist 
                // or nicely handle cases where the user is not in the badge list
                return (badgeList.hasOwnProperty(onyen)) ? badgeList[onyen] : [];
            }

            let rank = 1;
            for (const row of results) {
                let currBadges = getBadgeList(row['Onyen']).map(item => games[item]['icon']).join(' ');
                // add leading space to badges for spacing
                if (currBadges.length > 0) currBadges = ' ' + currBadges;
                const [name, classes] = (row['Onyen'] == user) ? ['You' + currBadges, 'you'] : [row['Onyen'] + currBadges, ''];

                addToTable(leaderboard, [rank, name, row['Total'].toLocaleString()], classes);

                rank++;
            }

            hideRows(leaderboard, user);
        });
    });
}

const setUpGame = (game, user) => {
    // add a single game to the carousel and a tab to the page
    const id = game['id'];
    const name = game['name'];

    const tab = $('<button id="' + id + '-tab" class="tab" data-info="' + id + '-info" style="display: none;">' + name + ' <i class="far fa-window-close close-tab"></i></button>');
    $('#top-tabs').append(tab);

    const infoDiv = $('<div id="' + id + '-info" class="tab-info"></div>');
    infoDiv.append($('<h2 class="center pixel">' + name + '</h2>'));
    infoDiv.append($('<p class="pixel">' + game['overview'] + '</p>'));
    infoDiv.append($('<p class="pixel">' + game['tagline'] + '</p>'));
    const div = $('<div></div>');
    const button = $('<button class="play">Play</button>"');
    button.on('click', function () {
        const newLocation = '/' + game['entrypoint'] + '?user=' + encodeURIComponent(user);

        // this code opens the link in the same tab
        location.href = newLocation;

        // this code opens the link in a new tab - we have chosen to instead use the Quit button in the game to send you 
        // back to the games.fo.unc.edu main screen
        // window.open(newLocation, '_blank');
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
    // fetch API - https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API
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

            // https://www.w3schools.com/jsref/met_win_setinterval.asp
            // https://developer.mozilla.org/en-US/docs/Web/API/WindowOrWorkerGlobalScope/setInterval
            window.setInterval(genFunFact, secondsFunFact * 1000);
        }).catch(error => {
            console.log('Error: ' + error);
        });

    $('.tab-info').hide();
    const currDiv = $('.tab.active').attr('data-info');
    $('#' + currDiv).show();

    $('.message-content').hide();
    const showMessage = $('.message.active').attr('data-message');
    $('#' + showMessage).show();

    $('#left-arrow').on('click', () => scroll(false));

    $('#right-arrow').on('click', () => scroll(true));

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

            console.log(rows.length, rowsShown);
            if (rowsShown > rows.length) {
                return;
            }

            const showMore = tbody.find('.showmore-row');
            const ellipsis = tbody.find('.ellipsis-row');
            showMore.detach();
            ellipsis.detach();

            const indexFirstHidden = tbody.find('tr.hidden').eq(0).index();
            if (indexFirstHidden == -1) return;
            for (let i = indexFirstHidden; i < rowsShown + 5; i++) {
                rows.eq(i).removeClass('hidden');
            }

            const firstHidden = rows.find('tr.hidden').eq(0);

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

        // console.log('User: ' + user);

        hideArrows($('#game-cards').scrollLeft(), $('#game-cards').width());

        fetchGames(user);

        fetchMessages().then(() => {
            $('.correctButton').each((index, element) => {
                $(element).on('click', () => {
                    correctButtonPress(user).then(() => {
                        window.location.href = '/goodMessage.html';
                    }).catch(() => {
                        window.location.href = '/goodMessage.html';
                    });
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