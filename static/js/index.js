const funFacts = ['Fun Fact 1',
    'Fun Fact 2',
    'Fun Fact 3'];

const scrollAmount = 50;
const secondsFunFact = 30;
let rowsShown = 10;

const randomChoice = (arr) => {
    return arr[Math.floor(Math.random() * arr.length)];
}

const genFunFact = () => {
    $('#fun-fact').text(randomChoice(funFacts));
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

const setup = () => {
    $('.tab-info').hide();
    $('#leaderboard').show();

    $('.message-content').hide();
    const showMessage = $('.message.active').attr('data-message');
    $('#' + showMessage).show();

    $('#left-arrow').on('click', function () {
        scroll(false);
    });

    $('#right-arrow').on('click', function () {
        scroll(true);
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

$(document).ready(function () {
    hideArrows($('#game-cards').scrollLeft(), $('#game-cards').width());

    genFunFact();
    window.setInterval(genFunFact, secondsFunFact * 1000);

    fetch('games.json')
        .then(response => {
            if (!response.ok) {
                throw new Error("HTTP error " + response.status);
            }
            return response.json();
        }).then(json => {
            for (let game of json) {
                const id = game['id'];
                const name = game['name'];

                const tab = $('<button id="' + id + '-tab" class="tab" data-info="' + id + '-info" style="display: none;">' + name + '<i class="far fa-window-close close-tab"></i></button>');
                $('#tabs').append(tab);

                const infoDiv = $('<div id="' + id + '-info" class="tab-info"></div>');
                infoDiv.append($('<h2 class="center pixel">' + name + '</h2>'));
                infoDiv.append($('<p class="pixel">' + game['tagline'] + '</p>'));
                const div = $('<div></div>');
                const button = $('<button class="play">Play</button>"');
                button.on('click', function () {
                    const newLocation = '/' + game['entrypoint'] + '?user=tas127';
                    window.location.href = newLocation;
                });
                div.append(button);
                infoDiv.append(div);
                $('#info').append(infoDiv);

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
                    gameCard.append($('<p>Coming soon!</p>'));
                }
                $('#game-cards').append(gameCard);
            }
        }).then(() => {
            setup();
        });
});