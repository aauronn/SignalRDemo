
class ViewModel2 {
    public loginToken: string;
    public error: string;
    public connection: signalR.HubConnection;
    public connectionStarted: boolean;
    public params: string = JSON.stringify({ "userIPAddress": "10.10.10.10", "userMachineName": "MachineName", "username": "lgutie209" });

    public async connect() {
        try {
            //evt.preventDefault();

            //const resp = await fetch('http://localhost:32772/api/account/createToken', {
            const resp = await fetch('http://localhost:5000/api/account/createToken', {
                method: 'POST',
                body: this.params,
                headers: {
                    "Content-Type": "application/json; charset=utf-8",
                    //"Authorization": "Basic QUNjZGJmNzQxZGIyZDA2MTUxMDg5MWYwNTllOWMzMGEyMDo0NGRhOWJkNzA0MzI4NTJhN2M4MzEyMTRmNDhiYzBlZA=="
                    // "Content-Type": "application/x-www-form-urlencoded",
                }
            });
            //    .then(function (response) {
            //    return response.json();
            //}).then(function (data) {
            //    this.loginToken = data["token"];
            //});

            const json = await resp.json();
            debugger
            this.loginToken = json["token"];

            // Connect, using the token we got.
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/chat", {
                    skipNegotiation: true,
                    transport: signalR.HttpTransportType.WebSockets,
                    accessTokenFactory: () => this.loginToken
                })
                .build();

            this.connection.on("ReceiveMessage", (user, message) => {
                const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                const encodedMsg = user + " says " + msg;
                const li = document.createElement("li");
                li.textContent = encodedMsg;
                document.getElementById("messagesList").appendChild(li);
            });

            await this.connection.start();
            this.connectionStarted = true;

        } catch (e) {
            this.error = `Error connecting: ${e}`;
        }

    }

    static async run() {
        const model = new ViewModel2();

        await model.connect();
    }
}

ViewModel2.run();