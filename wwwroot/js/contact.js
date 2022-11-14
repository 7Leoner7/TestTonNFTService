let Storage0 = window.localStorage;
let item0 = Storage0.getItem("PersonLog");
let address0 = JSON.parse(item0).address;
var req = new XMLHttpRequest();
// Получаем элемент чата
let chat = document.querySelector("#divMessages");
req.open("POST", "/Second/GetClientsDialog?vm.address=" + address, false);
req.send(null);
let str = req.responseText.split('\\');
for (let i = 0; i < str.length; i++)
    chat.innerHTML += '<div class="msg">' + str[i].replace('|u', '<u>').replace('u|', '</u>') + '</div>';
    // Получаем строку ввода сообщения
let input = document.querySelector("#inputMessage");
    // Получаем кнопку для ввода сообщения
let btnSubmit = document.querySelector("#btnSend");
    // Отслеживаем нажатие мыши
btnSubmit.addEventListener("click", () => {
    // Получаем текст из формы для ввода сообщения
    message = 'Me: ' + input.value;
    // Отправка сообщения
    chat.innerHTML += '<div class="msg">' + message.replace('|u', '<u>').replace('u|', '</u>') + '</div>';
    req = new XMLHttpRequest();
    req.open("POST", "/Second/SendClientMessage?vm.messages=" + message + "&vm.address=" + address0, false);
    req.send(null);
    // Очищаем поле для ввода текста
    input.value = '';
    chat.scrollTo({ top: chat.scrollHeight });
});

    
