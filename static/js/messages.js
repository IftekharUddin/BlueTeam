// messages.js
// Created with the help of https://www.w3schools.com/php/php_ajax_database.asp

const setupMessage = (message, idx) => {
    const messageText = $(message['Message']);

    const messageDiv = $('<div class="message" data-message="message-' + idx + '"></div>');

    const from = $('<h5 class="message-from">' + message['Onyen'] + '</h5>');
    messageDiv.append(from);

    const title = $('<h4 class="message-title">' + message['Title'] + '</h4>');
    messageDiv.append(title);

    const preview = messageText.text().substring(0, 10) + ' ...';
    const messagePreview = $('<p class="message-preview">' + preview + '</p>');
    messageDiv.append(messagePreview);

    $('#messages-choose').append(messageDiv);

    const messageContent = $('<div class="message-content" id="message-' + idx + '"></div>');
    messageContent.append(messageText);

    $('#messages-container').append(messageContent);
}

const getMessageRequest = () => {
    return $.ajax('/sqlconnect/games/getMessages.php', {
        type: 'GET'
    });
}

const getMessages = () => {
    return getMessageRequest().then(response => {
        return response;
    }).catch(err => {
        return err;
    });
}

const fetchMessages = () => {
    getMessages().then(messages => {
        let idx = 0;
        for (const message of messages) {
            setupMessage(message, idx++);
        }
        console.log(messages);
        $('.message').eq(0).addClass('active');
    }).catch(err => {
        console.log(err);
    });
}

//THE FOLLOWIWNG TWO FUNCTIONS are exported so that the main js page, index.js call listen for the button presses that will call these functions - this will be sent to /sqlconnect/games/updateMessagesScore.php - this will update the db
export const correctButtonPress = () => {
    return $.ajax('/sqlconnect/games/updateMessagesScore.php', {
        type: 'GET',
        data: 100,
        processData: false
    });
}

export const incorrectButtonPress = () => {
    return $.ajax('/sqlconnect/games/updateMessagesScore.php', {
        type: 'GET',
        data: -100,
        processData: false
    });
}

export { fetchMessages as default };