<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/5.0.13/signalr.min.js"></script>

//var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

//document.getElementById("sendButton").disabled = true;

//connection.on("receiveMessage", function (message) {
//    var li = document.createElement("li");
//    li.innerHTML = `
//        <div class="message @("my_message" if (message.From == currentUser.UserName) else "frnd_message")">
//            <p>${message.Message} <br> <span>${message.Time}</span> </p>
//        </div>
//    `;
//    document.getElementById("messagesBox").appendChild(li);
//});

//connection.start().then(function () {
//    document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err.toString());
//});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var userId = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;

//    connection.invoke("SendMessage", { toUser: userId, message: message }).catch(function (err) {
//        console.error(err.toString());
//    });

//    fetch(`/Messages/SendMessage?recipientId=${userId}&message=${message}`, {
//        method: "POST",
//        headers: {
//            "Content-Type": "application/json"
//        }
//    });

//    event.preventDefault();
//});

//document.querySelectorAll(".block").forEach(function (block) {
//    block.addEventListener("click", async function () {
//        var userId = block.querySelector(".user-id").value;
//        var userName = block.querySelector(".user-name").textContent;

//        document.querySelector(".selected-user").innerHTML = `${userName} <br> <span>online</span>`;
//        await loadUserMessages(userId);
//    });
//});

//async function loadUserMessages(userId) {
//    var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

//    document.getElementById("sendButton").disabled = true;

//    connection.on("receiveMessage", function (message) {
//        var li = document.createElement("li");
//        li.innerHTML = `
//            <div class="message @("my_message" if (message.From == currentUser.UserName) else "frnd_message")">
//                <p>${message.Message} <br> <span>${message.Time}</span> </p>
//            </div>
//        `;
//        document.getElementById("messagesBox").appendChild(li);
//    });

//    try {
//        await connection.start();
//        document.getElementById("sendButton").disabled = false;
//    } catch (err) {
//        console.error(err.toString());
//    }

//    document.getElementById("sendButton").addEventListener("click", function (event) {
//        var message = document.getElementById("messageInput").value;
//        connection.invoke("SendMessage", { toUser: userId, message: message }).catch(function (err) {
//            console.error(err.toString());
//        });
//        event.preventDefault();
//    });
//}


