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
        type: 'POST'
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
    return getMessages().then(messages => {
        let idx = 0;
        for (const message of messages) {
            setupMessage(message, idx++);
        }
        $('.message').eq(0).addClass('active');
    }).catch(err => {
        console.log(err);
        setupMessage({
            'Message': '<p>No messages to display yet! Stay tuned!</p>',
            'Title': 'Keep checking these!',
            'Onyen': 'dbarker'
        }, 0);
        setupMessage({
            'Message': '<p>Click <a href="./message.html">here</a> to receive your 100 points!</p><button class="correctButton">Report</button><button class="incorrectButton">Reply</button>',
            'Title': 'Example',
            'Onyen': 'tas127'
        }, 1);
        $('.message').eq(0).addClass('active');
    });
}

//THE FOLLOWIWNG TWO FUNCTIONS are exported so that the main js page, index.js call listen for the button presses that will call these functions - this will be sent to /sqlconnect/games/updateMessagesScore.php - this will update the db

// number of points below can easily be changed!
export const correctButtonPress = (user) => {
    return $.ajax('/sqlconnect/games/updateMessagesScore.php', {
        type: 'POST',
        data: {
            'onyen': user,
            'data': 500
        }
    });
}

export const incorrectButtonPress = (user) => {
    return $.ajax('/sqlconnect/games/updateMessagesScore.php', {
        type: 'POST',
        data: {
            'onyen': user,
            'data': -500
        }
    });
}

export { fetchMessages as default };