"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl('/Chatroom/Open')
    .build();

connection.on('ReceiveMessage', function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var div = document.createElement("div");
    div.innerHTML += `<div class="d-flex flex-row justify-content-start mb-4"> <img src="https://i.pinimg.com/originals/ff/a0/9a/ffa09aec412db3f54deadf1b3781de2a.png" alt="avatar 1" style="width: 45px; height: 100%;"> <div class="p-3 ms-3" style="border-radius: 15px; background-color: #fbfbfb;"> <p class="small mb-0"> <p><strong>${user}</strong></p> ${msg} </p></div></div>`;
    
    document.getElementById("messages").appendChild(div);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
})

document.getElementById("sendButton").addEventListener("click", function (e) {
    var user = document.getElementById("sender").value;
    var message = document.getElementById("message").value;
    var group = document.getElementById("groupId").value;
    var userId = document.getElementById("senderId").value;
    connection.invoke("SendMessageToGroup", group, user, message, userId).catch(function (err) {
        return console.error(err.toString());
    });
    e.preventDefault;
});






















//class Message {
//    constructor(username, text, when) {
//        this.userName = username;
//        this.text = text;
//        this.when = when;
//    }
//}

//// userName is declared in razor page.
//const username = userName;
//const textInput = document.getElementById('messageText');
//const whenInput = document.getElementById('when');
//const chat = document.getElementById('chat');
//const messagesQueue = [];

//document.getElementById('submitButton').addEventListener('click', () => {
//    var currentDate = new Date();
//    whenInput.value = currentDate.getDate();
//});

//function clearInputField() {
//    messagesQueue.push(textInput.value);
//    textInput.value = "";
//}

//function sendMessage() {
//    let text = messagesQueue.shift() || "";
//    if (text.trim() === "") return;

//    let when = new Date();
//    let message = new Message(username, text, when);
//    sendMessageToHub(message);
//}

//function addMessageToChat(message) {
//    let isCurrentUserMessage = message.userName === username;

//    let container = document.createElement('div');
//    container.className = isCurrentUserMessage ? "container darker" : "container";

//    let sender = document.createElement('p');
//    sender.className = "sender";
//    sender.innerHTML = message.userName;
//    console.log(sender);
//    let text = document.createElement('p');
//    text.innerHTML = message.text;

//    let when = document.createElement('span');
//    when.className = isCurrentUserMessage ? "time-left" : "time-right";
//    var currentDate = new Date();
//    when.innerHTML = currentDate.getDate();

//    container.appendChild(sender);
//    container.appendChild(text);
//    container.appendChild(when);
//    chat.appendChild(container);
//}
