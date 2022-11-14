let Storage = window.localStorage;
let item = Storage.getItem("PersonLog");
let pkey = "";
let skey = "";
let address = "";
var req = new XMLHttpRequest();
if (item != null) {
    pkey = JSON.parse(item).public_key;
    skey = JSON.parse(item).secret_key;
    address = JSON.parse(item).address;   
    req.open("POST", "/Second/GetBalance?vm.public_key=" + pkey + "&vm.secret_key=" + skey + "&vm.address=" + address, false);
    req.send(null);
    document.getElementById("balance").innerHTML = req.responseText;
}

document.addEventListener("click", auth_function = (e) => {
    if (e.target.id == "authorsCollects") {
        window.location.assign("/Main/PersonArea?vm.ToAddress=" + document.getElementById("author").innerHTML + "&vm.address=" + address);
    }
    if (e.target.id == "report") {
        let image = document.getElementById("MainImage");
        let jsonImg = JSON.parse(image.alt);
        if (confirm("Вы уверены, что хотите подать жалобу?")) {
            message = 'Me: Я нашёл похожее изображение с адресом коллекции |u' + jsonImg.IdColl + ':' + jsonImg.idNFT + "u|";
            req = new XMLHttpRequest();
            req.open("POST", "/Second/SendClientMessage?vm.messages=" + message + "&vm.address=" + address, false);
            req.send(null);
            alert("Жалоба отправлена");
        }
    }
    if (e.target.id == "buy") {
        if (confirm("Вы уверены, что хотите купить данное NFT?")) {
            let image = document.getElementById("MainImage");
            req = new XMLHttpRequest();
            req.open("POST", "/Second/Transaction?vm.nft=" + image.alt + "&vm.address=" + address + "&vm.public_key=" + pkey + "&vm.secret_key=" + skey, false);
            req.send(null);
            if (req.responseText == "True") alert("Транзакция совершена успешно");
            else alert("Транзакция провалена");
        }
    }
});