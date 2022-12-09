"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl('/Chatroom/Open')
    .build();

connection.on('ReceiveMessage', function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var div = document.createElement("div");

    var userName = document.getElementById("sender").value;

    var currentdate = new Date();
    var options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
    var date = currentdate.toLocaleDateString("en-UK", options);
    var time = currentdate.toLocaleTimeString("en-US");
    var datetime = date + " " + time;
    
    //= currentdate.getDate() + "/" + (currentdate.getMonth() + 1)
    //    + "/" + currentdate.getFullYear() + " "
    //    + currentdate.getHours() + ":" + currentdate.getMinutes();

    if (userName === user) {
        div.innerHTML += `<div class="d-flex flex-row justify-content-end mb-4"> <div class="p-3 me-3 border" style="border-radius: 15px; background-color: rgba(57, 192, 237,.2);"> <p class="small mb-1 text-muted">${datetime}</p> <p class="small mb-0"> <p><strong>${user}</strong> <img src="https://i.pinimg.com/originals/ff/a0/9a/ffa09aec412db3f54deadf1b3781de2a.png" alt="avatar 1" style="width: 25px; height: 100%;"></p> ${msg} </p> </div> </div>`;
    }
    else {
        div.innerHTML += `<div class="d-flex flex-row justify-content-start mb-4"> <div class="p-3 ms-3" style="border-radius: 15px; background-color: #fbfbfb;"> <p class="small mb-1 text-muted">${datetime}</p> <p class="small mb-0"> <p> <img src="https://i.pinimg.com/originals/ff/a0/9a/ffa09aec412db3f54deadf1b3781de2a.png" alt="avatar 1" style="width: 25px; height: 100%;"> <strong>${user}</strong> </p> ${msg} </p> </div> </div>`;
    }

    var container = document.getElementById("messages");
    container.appendChild(div);
    container.scrollTop = container.scrollHeight;
});

connection.start().catch(function (err) {
    return console.error(err.toString());
})

document.getElementById("sendButton").addEventListener("click", function (e) {
    var user = document.getElementById("sender").value;
    var message = document.getElementById("message").value;
    var group = document.getElementById("groupId").value;
    var userId = document.getElementById("senderId").value;

    if (message != "") {
        connection.invoke("SendMessageToGroup", group, user, message, userId).catch(function (err) {
            return console.error(err.toString());
        });
    }

    var clearMessage = document.getElementById("message");
    clearMessage.value = "";

    var container = document.getElementById("messages");
    container.scrollTop = container.scrollHeight;

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
