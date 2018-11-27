var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ViewModel2 {
    constructor() {
        this.params = JSON.stringify({ "userIPAddress": "10.10.10.10", "userMachineName": "MachineName", "username": "lgutie209" });
    }
    connect() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                //evt.preventDefault();
                //const resp = await fetch('http://localhost:32772/api/account/createToken', {
                const resp = yield fetch('http://localhost:5000/api/account/createToken', {
                    method: 'POST',
                    body: this.params,
                    headers: {
                        "Content-Type": "application/json; charset=utf-8",
                    }
                });
                //    .then(function (response) {
                //    return response.json();
                //}).then(function (data) {
                //    this.loginToken = data["token"];
                //});
                const json = yield resp.json();
                debugger;
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
                yield this.connection.start();
                this.connectionStarted = true;
            }
            catch (e) {
                this.error = `Error connecting: ${e}`;
            }
        });
    }
    static run() {
        return __awaiter(this, void 0, void 0, function* () {
            const model = new ViewModel2();
            yield model.connect();
        });
    }
}
ViewModel2.run();
