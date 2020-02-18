const funFacts = ['Fun Fact 1',
    'Fun Fact 2',
    'Fun Fact 3'];

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

const showMoreHandler = () => {
    const rows = $('#leaderboard tbody>tr:not(.row-info)');
    if (rowsShown > rows.length) {
        return;
    }
    for (let i = rowsShown; i < rowsShown + 5; i++) {
        rows.eq(i).show();
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

$(document).ready(function () {
    const scrollAmount = 50;
    hideArrows($('#game-cards').scrollLeft(), $('#game-cards').width());

    genFunFact();
    window.setInterval(genFunFact, secondsFunFact * 1000);

    $('.tab-info').hide();
    $('#leaderboard').show();

    $('.message-content').hide();
    const showMessage = $('.message.active').attr('data-message');
    $('#' + showMessage).show();

    $('#left-arrow').on('click', function () {
        const currScroll = $('#game-cards').scrollLeft();
        const newScroll = currScroll - scrollAmount;
        const width = $('#game-cards').get(0).scrollWidth - $('#game-cards').outerWidth();
        $('#game-cards').scrollLeft(newScroll);
        hideArrows($('#game-cards').scrollLeft(), width);
    });

    $('#right-arrow').on('click', function () {
        const currScroll = $('#game-cards').scrollLeft();
        const width = $('#game-cards').outerWidth();
        const scrollWidth = $('#game-cards').get(0).scrollWidth;
        const newScroll = currScroll + scrollAmount;
        $('#game-cards').scrollLeft(newScroll);
        hideArrows($('#game-cards').scrollLeft(), scrollWidth - width);
    });

    $('#main-button').on('click', function () {
        $('#messages').hide();
        $('#info').show();
    });

    $('#messages-button').on('click', function () {
        $('#info').hide();
        $('#messages').show();
    });

    $('#password-card').on('click', function () {
        $('.tab-info').hide();
        $('#password-game').show();
        $('.tab').removeClass('active');
        $('#password-tab').show();
        $('#password-tab').addClass('active');
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
});