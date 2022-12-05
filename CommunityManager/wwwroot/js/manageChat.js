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
    var group = document.getElementById("groupJoinId").value;
    connection.invoke("JoinGroup", group).catch(function (err) {
        return console.error(err.toString());
    });
    e.preventDefault;
});

document.getElementById("leaveButton").addEventListener("click", function (e) {
    var group = document.getElementById("groupLeaveId").value;
    connection.invoke("LeaveGroup", group).catch(function (err) {
        return console.error(err.toString());
    });
    e.preventDefault;
});