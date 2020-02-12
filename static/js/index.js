$(document).ready(function () {
    const scrollAmount = 50;
    $('#left-arrow').on('click', function () {
        const currScroll = $('#game-cards').scrollLeft();
        const newScroll = (currScroll > scrollAmount) ? currScroll - scrollAmount : 0;
        $('#game-cards').scrollLeft(newScroll);
    });

    $('#right-arrow').on('click', function () {
        const currScroll = $('#game-cards').scrollLeft();
        const width = $('#game-cards').width();
        const newScroll = (currScroll < width - scrollAmount) ? currScroll + scrollAmount : width + scrollAmount;
        $('#game-cards').scrollLeft(newScroll);
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
        $('#back-button').on('click', function () {
            $('#password-game').hide();
            $('#info').show();
        });
        $('#info').hide();
        $('#password-game').show();
    })
});