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
    
    if (userName === user) {
        div.innerHTML += `<div class="d-flex flex-row justify-content-end mb-4"> <div class="p-3 me-3 border" style="border-radius: 15px; background-color: rgba(57, 192, 237,.2);"> <p class="small mb-1 text-muted">${datetime}</p> <p class="small mb-0"> <p><strong>${user}</strong> <img src="https://i.pinimg.com/originals/ff/a0/9a/ffa09aec412db3f54deadf1b3781de2a.png" alt="avatar 1" style="width: 25px; height: 100%;"></p> <p style="width: 350px">${msg}</p> </div> </div>`;
    }
    else {
        div.innerHTML += `<div class="d-flex flex-row justify-content-start mb-4"> <div class="p-3 ms-3" style="border-radius: 15px; background-color: #fbfbfb;"> <p class="small mb-1 text-muted">${datetime}</p> <p class="small mb-0"> <p> <img src="https://i.pinimg.com/originals/ff/a0/9a/ffa09aec412db3f54deadf1b3781de2a.png" alt="avatar 1" style="width: 25px; height: 100%;"> <strong>${user}</strong> </p> <p style="width: 350px">${msg}</p> </div> </div>`;
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