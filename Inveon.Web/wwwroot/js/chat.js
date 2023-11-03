const chatButton = document.querySelector("#chat_button");
const chatBox = document.querySelector("#chat_box");
const closeChat = document.querySelector("#close_chat");
const chatSend = document.querySelector("#chat_send"); 
const messageInput = document.querySelector("#message_input");
const userIdentity = document.querySelector("#user_identity");
const chatContent = document.querySelector("#chat_content");


/*Gets New Messages From Queue*/
(function setupConnection() {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    //chatButton.disabled = true;
    connection.on("lastMessage", function (message) {

        createMessage(message);

    });
    connection.start().then(function () {
    //chatButton.disabled = false;
    }).catch(err => console.error(err.toString())).then(response => console.log("connected"));
})();

chatButton.addEventListener("click", function() {
    chatBox.classList.remove("d-none");
})

closeChat.addEventListener("click", function() {
    chatBox.classList.add("d-none");
})



    /*Sends Message to Queue*/
function sendMessage(username) {
    let text = messageInput.value;

    if (!text.trim().length) {
        return;
    }
    let time = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    fetch("/Message", {
        method: "POST",
        body: JSON.stringify({
            text,username,time
        }),
        headers: {
            'content-type': 'application/json'
        }
    }).then(response => console.log("message created"));

    messageInput.value = "";
}


function createMessage (message){

    // Create the main container
    var container = document.createElement("div");
    userIdentity.textContent === message.username ? container.className = "message-container right" : container.className = "message-container left"

    // Create the message time span
    var messageTime = document.createElement("span");
    messageTime.className = "message-time";
    messageTime.textContent = message.time;

    // Create the message container
    var messageContainer = document.createElement("div");
    messageContainer.className = "message";

    // Create the message text span
    var messageText = document.createElement("span");
    messageText.className = "message-text";
    messageText.textContent = message.text;

    // Append elements to their respective parents
    messageContainer.appendChild(messageText);
    container.appendChild(messageTime);
    container.appendChild(messageContainer);

    // Append the div element to a parent element (e.g., the body of the document)
    chatContent.appendChild(container);


}



