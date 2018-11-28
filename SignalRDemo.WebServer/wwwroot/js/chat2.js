// The following sample code uses modern ECMAScript 6 features 
// that aren't supported in Internet Explorer 11.
// To convert the sample for environments that do not support ECMAScript 6, 
// such as Internet Explorer 11, use a transpiler such as 
// Babel at http://babeljs.io/. 
//
// See Es5-chat.js for a Babel transpiled version of the following code:
var params = JSON.stringify({ "userIPAddress": "10.10.10.10", "userMachineName": "MachineName", "username": "lgutie209" });
var token = "";
var ServerUrl = "http://localhost:5005";

function init() {
    var userIPAddress = "10.10.10.10";
    var userMachineName = "MachineName";
    var userName = document.getElementById("UserNameInput0").value;

    params = JSON.stringify({ "userIPAddress": userIPAddress, "userMachineName": userMachineName, "username": userName });

    GetToken();
}

async function GetToken() {
    return await fetch(`${ServerUrl}/api/account/createToken`, {
        method: 'POST',
        body: params,
        headers: {
            "Content-Type": "application/json; charset=utf-8",
            //"Authorization": "Basic QUNjZGJmNzQxZGIyZDA2MTUxMDg5MWYwNTllOWMzMGEyMDo0NGRhOWJkNzA0MzI4NTJhN2M4MzEyMTRmNDhiYzBlZA=="
            // "Content-Type": "application/x-www-form-urlencoded",
        }
    })
        .then(function (response) {
            return response.json();
        })
        .then((data) => {
            token = data.token;
        })
        .then(() => {
            connect();
        })
        .catch((error) => {
            var errorMessage = "Invalid Authentication"
        });
}


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: () => token
    })
    .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
    .build();

function connect() {
    //const connection = new signalR.HubConnectionBuilder()
    //    .withUrl("/chatHub", {
    //        skipNegotiation: true,
    //        transport: signalR.HttpTransportType.WebSockets,
    //        accessTokenFactory: () => token
    //    })
    //    .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
    //    .build();

    connection.on("ReceiveMessage", (user, message) => {
        const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        const encodedMsg = user + " says " + msg;
        const li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    });

    connection.on("ReceiveChatMessage", (message) => {
        const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        const encodedMsg = msg;
        const li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    });

    connection.on("ReceiveDirectMessage", (user, message) => {
        const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        const encodedMsg = "Private Message From: " + user + " says " + msg;
        const li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    });

    

    connection.start()
              .catch(err => console.error(err.toString()));

    document.getElementById("sendButton").addEventListener("click", event => {
        const user = document.getElementById("userInput").value;
        const message = document.getElementById("messageInput").value;
        connection.invoke("SendToUser", user, message).catch(err => console.error(err.toString()));
        event.preventDefault();
    });

    document.getElementById("sendButton0").addEventListener("click", event => {
        const message = document.getElementById("messageInput0").value;
        connection.invoke("Send", message).catch(err => console.error(err.toString()));
        event.preventDefault();
    });

    document.getElementById("getUsersButton").addEventListener("click", event => {

        //connection.stream("Counter", 10, 500)
        //    .subscribe({
        //        next: (item) => {
        //            var li = document.createElement("li");
        //            li.textContent = item;
        //            document.getElementById("messagesList").appendChild(li);
        //        },
        //        complete: () => {
        //            var li = document.createElement("li");
        //            li.textContent = "Stream completed";
        //            document.getElementById("messagesList").appendChild(li);
        //        },
        //        error: (err) => {
        //            var li = document.createElement("li");
        //            li.textContent = err;
        //            document.getElementById("messagesList").appendChild(li);
        //        },
        //    });

        connection.stream("Counter", 1000)
            .subscribe({
                next: (item) => {
                    var li = document.createElement("li");
                    li.textContent = item;
                    document.getElementById("messagesList").appendChild(li);
                },
                complete: () => {
                    var li = document.createElement("li");
                    li.textContent = "Stream completed";
                    document.getElementById("messagesList").appendChild(li);
                },
                error: (err) => {
                    var li = document.createElement("li");
                    li.textContent = err;
                    document.getElementById("messagesList").appendChild(li);
                },
            });

    });
}

