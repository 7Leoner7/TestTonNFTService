var req = new XMLHttpRequest();
req.open("POST", "/Second/GetMessagesAddress?vm.address=" + address, false);
req.send(null);
let QuestsClient = document.getElementById("QuestsClient");
let str = req.responseText.split('\\');
let listHTML = "";
for (let i = 0; i < str.length; i++)
    listHTML += '<li><button id="addr" value='+str[i]+'>'+str[i]+'</button></li>';
QuestsClient.innerHTML = '<ul>' + listHTML + '</ul>';

document.addEventListener("click", (e) => {
    if (e.target.id == "addr") {
        let block = `<div align="center">
            <form style="margin-top: 2%; width: 1200px;">
                <div align="left" id="divMessages" style="display: inline-block; width: 100%; overflow-y: auto; height: 80vh; background-color: #e7e7e7;"></div>
                <div class="container">
                    <div id="blockSendMessage">
                        <input align="left" id="inputMessage" type="text" placeholder="Введите сообщение..." style="width: 900px; box-sizing: border-box; padding: 8px 8px;">
                            <input align="right" id="btnSend" type="button" class="buttonscript buttonPA" value="Отправить" />
                        </div>
                    </div>
                </form>
            </div>`;
        document.getElementById("AdminPanel").innerHTML = block;
        req = new XMLHttpRequest();
        // Получаем элемент чата
        let chat = document.querySelector("#divMessages");
        req.open("POST", "/Second/GetClientsDialog?vm.address=" + e.target.value, false);
        req.send(null);
        str = req.responseText.split('\\');
        for (let i = 0; i < str.length; i++)
            chat.innerHTML += '<div class="msg">' + str[i].replace('|u', '<u>').replace('u|', '</u>') + '</div>';
            // Получаем строку ввода сообщения
        let input = document.querySelector("#inputMessage");
            // Получаем кнопку для ввода сообщения
        let btnSubmit = document.querySelector("#btnSend");
            // Отслеживаем нажатие мыши
        btnSubmit.addEventListener("click", () => {
            // Получаем текст из формы для ввода сообщения
            message = 'Admin: ' + input.value;
            // Отправка сообщения
            chat.innerHTML += '<div class="msg">' + message.replace('|u', '<u>').replace('u|', '</u>') + '</div>';
            req = new XMLHttpRequest();
            req.open("POST", "/Second/SendClientMessage?vm.messages=" + message + "&vm.address=" + e.target.value, false);
            req.send(null);
            // Очищаем поле для ввода текста
            input.value = '';
            chat.scrollTo({ top: chat.scrollHeight });
        });
    }
    
})


    
